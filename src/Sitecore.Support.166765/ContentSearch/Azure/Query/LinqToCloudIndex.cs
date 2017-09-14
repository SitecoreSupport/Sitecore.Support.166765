namespace Sitecore.Support.ContentSearch.Azure.Query
{
  using System;
  using System.Collections.Concurrent;
  using System.Reflection;
  using Sitecore.ContentSearch.Linq.Common;
  public class LinqToCloudIndex<TItem> : Sitecore.ContentSearch.Azure.Query.LinqToCloudIndex<TItem>
  {
    private static ConcurrentDictionary<Type, FieldInfo> queryMapperFieldInfos = new ConcurrentDictionary<Type, FieldInfo>();
    public LinqToCloudIndex(Sitecore.ContentSearch.Azure.CloudSearchSearchContext context, IExecutionContext executionContext) : base(context, executionContext)
    {
    }

    public LinqToCloudIndex(Sitecore.ContentSearch.Azure.CloudSearchSearchContext context, IExecutionContext[] executionContexts) : base(context, executionContexts)
    {
      FieldInfo queryMapperFieldInfo;
      var type = this.GetType().BaseType.BaseType;
      if (queryMapperFieldInfos.TryGetValue(type, out queryMapperFieldInfo))
      {
        queryMapperFieldInfo.SetValue(this, new Sitecore.Support.ContentSearch.Azure.Query.CloudQueryMapper(this.Parameters));        
      }
      else
      {
        queryMapperFieldInfo = type.GetField("queryMapper", BindingFlags.Instance | BindingFlags.NonPublic);
        queryMapperFieldInfos.TryAdd(type, queryMapperFieldInfo);
        queryMapperFieldInfo.SetValue(this, new Sitecore.Support.ContentSearch.Azure.Query.CloudQueryMapper(this.Parameters));
      }
    }
  }
}