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
      if (queryMapperFieldInfos.TryGetValue(this.GetType(), out queryMapperFieldInfo))
      {
        queryMapperFieldInfo.SetValue(this, new Sitecore.Support.ContentSearch.Azure.Query.CloudQueryMapper(this.Parameters));        
      }
      else
      {
        var type = this.GetType();
        queryMapperFieldInfos.TryAdd(type, type.GetField("queryMapper", BindingFlags.Instance | BindingFlags.NonPublic));
      }
    }
  }
}