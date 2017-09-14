using Sitecore.ContentSearch.Azure.Query;
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

      var wrap = CloudQueryBuilder.ShouldWrap.None;
      if (node.SourceNode.NodeType == QueryNodeType.Or)
      {
        wrap = CloudQueryBuilder.ShouldWrap.Left;
      }
      else if (node.PredicateNode.NodeType == QueryNodeType.Or)
      {
        wrap = CloudQueryBuilder.ShouldWrap.Right;
      }
      else if (node.SourceNode.NodeType == QueryNodeType.Or && node.PredicateNode.NodeType == QueryNodeType.Or)
      {
        wrap = CloudQueryBuilder.ShouldWrap.Both;
      }

      return CloudQueryBuilder.Merge(left, right, "and", wrap);
    }
  }
}