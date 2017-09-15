namespace Sitecore.Support.ContentSearch.Azure
{
  using Sitecore.ContentSearch;
  using Sitecore.ContentSearch.Maintenance;
  using Sitecore.ContentSearch.Security;
  using System.Reflection;
  public class CloudSearchProviderIndex : Sitecore.ContentSearch.Azure.CloudSearchProviderIndex
  {
    private static readonly MethodInfo EnsureInitializedMethodInfo =
      typeof(Sitecore.ContentSearch.Azure.CloudSearchProviderIndex).GetMethod("EnsureInitialized",
        BindingFlags.Instance | BindingFlags.NonPublic);
    public CloudSearchProviderIndex(string name, string connectionStringName, string totalParallelServices, IIndexPropertyStore propertyStore) : base(name, connectionStringName, totalParallelServices, propertyStore)
    {
    }

    public CloudSearchProviderIndex(string name, string connectionStringName, string totalParallelServices, IIndexPropertyStore propertyStore, string @group) : base(name, connectionStringName, totalParallelServices, propertyStore, @group)
    {
    }

    public override IProviderSearchContext CreateSearchContext(SearchSecurityOptions options = SearchSecurityOptions.EnableSecurityCheck)
    {
      EnsureInitializedMethodInfo.Invoke(this, new object[0]);
      return new Sitecore.Support.ContentSearch.Azure.CloudSearchSearchContext(this, options);
    }
  }
}