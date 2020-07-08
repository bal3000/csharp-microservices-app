using Catalog.API.Data.Interfaces;
using Catalog.API.Entities;
using Catalog.API.Settings;
using MongoDB.Driver;

namespace Catalog.API.Data
{
    public class CatalogContext : ICatalogContext
    {
        public CatalogContext(ICatalogDatabaseSettings catalogDatabaseSettings)
        {
            var client = new MongoClient(catalogDatabaseSettings.ConnectionString);
            var database = client.GetDatabase(catalogDatabaseSettings.DatabaseName);
            this.Products = database.GetCollection<Product>(catalogDatabaseSettings.CollectionName);

            CatalogContextSeed.SeedData(this.Products);
        }

        public IMongoCollection<Product> Products { get; private set; }
    }
}
