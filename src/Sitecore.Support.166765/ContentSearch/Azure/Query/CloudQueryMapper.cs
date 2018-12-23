﻿using Sitecore.ContentSearch.Azure.Http;
using Sitecore.ContentSearch.Azure.Query;
using Sitecore.ContentSearch.Azure.Schema;
using Sitecore.ContentSearch.Linq.Common;
using Sitecore.ContentSearch.Linq.Helpers;
using Sitecore.ContentSearch.Linq.Nodes;

namespace Sitecore.Support.ContentSearch.Azure.Query
{
  public class CloudQueryMapper : Sitecore.ContentSearch.Azure.Query.CloudQueryMapper
  {
    public CloudQueryMapper(CloudIndexParameters parameters) : base(parameters)
    {
    }

    protected override string HandleWhere(WhereNode node, CloudQueryMapperState mappingState)
    {
      var left = this.HandleCloudQuery(node.SourceNode, mappingState);
      var right = this.HandleCloudQuery(node.PredicateNode, mappingState);

      var wrap = Sitecore.ContentSearch.Azure.Query.CloudQueryBuilder.ShouldWrap.None;
      if (node.SourceNode.NodeType == QueryNodeType.Or)
      {
        wrap = Sitecore.ContentSearch.Azure.Query.CloudQueryBuilder.ShouldWrap.Left;
      }
      else if (node.PredicateNode.NodeType == QueryNodeType.Or)
      {
        wrap = Sitecore.ContentSearch.Azure.Query.CloudQueryBuilder.ShouldWrap.Right;
      }
      else if (node.SourceNode.NodeType == QueryNodeType.Or && node.PredicateNode.NodeType == QueryNodeType.Or)
      {
        wrap = Sitecore.ContentSearch.Azure.Query.CloudQueryBuilder.ShouldWrap.Both;
      }

      return Sitecore.ContentSearch.Azure.Query.CloudQueryBuilder.Merge(left, right, "and", wrap);
    }

    private ICloudSearchIndexSchema Schema
    {
      get
      {
        return this.Parameters.Schema as ICloudSearchIndexSchema;
      }
    }

    protected override string HandleEqual(EqualNode node, CloudQueryMapperState state)
    {
      var onlyConstants = node.LeftNode is ConstantNode && node.RightNode is ConstantNode;

      if (onlyConstants)
      {
        var comparison = ((ConstantNode)node.LeftNode).Value.Equals(((ConstantNode)node.RightNode).Value);

        var expression = comparison
                   ? Sitecore.ContentSearch.Azure.Query.CloudQueryBuilder.Search.SearchForEverything
                   : Sitecore.ContentSearch.Azure.Query.CloudQueryBuilder.Search.SearchForNothing;

        return Sitecore.Support.ContentSearch.Azure.Query.CloudQueryBuilder.Search.Operations.Equal(null, expression, node.Boost);
      }

      var fieldNode = QueryHelper.GetFieldNode(node);
      var valueNode = QueryHelper.GetValueNode(node, fieldNode.FieldType);

      string query = null;
      if (this.ProcessAsVirtualField(fieldNode.FieldKey, valueNode.Value, node.Boost, ComparisonType.Equal, state, out query))
      {
        return query;
      }

      return this.HandleEqual(fieldNode.FieldKey, valueNode.Value, node.Boost);
    }

    private string HandleEqual(string initFieldName, object fieldValue, float boost)
    {
      var fieldName = this.Parameters.FieldNameTranslator.GetIndexFieldName(initFieldName, this.Parameters.IndexedFieldType);
      var fieldSchema = this.Schema.GetFieldByCloudName(fieldName);
      if (fieldSchema == null)
      {
        var expression = fieldValue == null ?
          Sitecore.ContentSearch.Azure.Query.CloudQueryBuilder.Search.SearchForEverything :
          Sitecore.ContentSearch.Azure.Query.CloudQueryBuilder.Search.SearchForNothing;

        return Sitecore.Support.ContentSearch.Azure.Query.CloudQueryBuilder.Search.Operations.Equal(null, expression, boost);
      }

      var formattedValue = this.ValueFormatter.FormatValueForIndexStorage(fieldValue, fieldName);

      if (fieldSchema.Type == EdmTypes.StringCollection)
      {
        return Sitecore.ContentSearch.Azure.Query.CloudQueryBuilder.Filter.Operations.Collections.Any(fieldName, formattedValue, fieldSchema.Type);
      }

      if (formattedValue == null)
      {
        return $"&$filter={fieldName} eq null";
      }

      if (formattedValue is string)
      {
        if (formattedValue.ToString().Trim() == string.Empty)
        {
          return Sitecore.ContentSearch.Azure.Query.CloudQueryBuilder.Filter.Operations.Equal(fieldName, formattedValue, fieldSchema.Type, boost);
        }

        return Sitecore.Support.ContentSearch.Azure.Query.CloudQueryBuilder.Search.Operations.Equal(fieldName, formattedValue, boost);
      }

      return Sitecore.ContentSearch.Azure.Query.CloudQueryBuilder.Filter.Operations.Equal(fieldName, formattedValue, fieldSchema.Type, boost);
    }
  }
}