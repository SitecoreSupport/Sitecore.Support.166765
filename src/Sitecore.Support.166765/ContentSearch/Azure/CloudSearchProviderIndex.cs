namespace Sitecore.Support.ContentSearch.Azure
{
  using System;
  using Sitecore.ContentSearch;
  using Sitecore.ContentSearch.Azure;
  using Sitecore.ContentSearch.Maintenance;
  using Sitecore.ContentSearch.Security;
  public class CloudSearchProviderIndex : Sitecore.ContentSearch.Azure.CloudSearchProviderIndex
  {
    public CloudSearchProviderIndex(string name, string connectionStringName, string totalParallelServices, IIndexPropertyStore propertyStore) : base(name, connectionStringName, totalParallelServices, propertyStore)
    {
    }

    public CloudSearchProviderIndex(string name, string connectionStringName, string totalParallelServices, IIndexPropertyStore propertyStore, string @group) : base(name, connectionStringName, totalParallelServices, propertyStore, @group)
    {
    }

    public override IProviderSearchContext CreateSearchContext(SearchSecurityOptions options = SearchSecurityOptions.EnableSecurityCheck)
    {
      this.EnsureInitialized();
      return new Sitecore.Support.ContentSearch.Azure.CloudSearchSearchContext(this, options);
    }

    private void EnsureInitialized()
    {
      if (!this.initialized)
      {
        throw new InvalidOperationException("Index has not been initialized.");
      }
    }
  }
}