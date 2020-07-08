using System.Collections.Generic;
using System.Threading.Tasks;
using Catalog.API.Data.Interfaces;
using Catalog.API.Entities;
using Catalog.API.Repositories.Interfaces;
using MongoDB.Driver;

namespace Catalog.API.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ICatalogContext _catalogContext;
        public ProductRepository(ICatalogContext catalogContext)
        {
            _catalogContext = catalogContext;
        }

        public async Task<IEnumerable<Product>> GetProducts() =>
            await _catalogContext.Products.Find(_ => true).ToListAsync();


        public async Task<Product> GetProduct(string id) =>
            await _catalogContext.Products.Find(p => p.Id == id).FirstOrDefaultAsync();


        public async Task<IEnumerable<Product>> GetProductbyName(string name)
        {
            var filter = Builders<Product>.Filter.ElemMatch(p => p.Name, name);
            return await _catalogContext.Products.Find(filter).ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductByCategory(string categoryName)
        {
            var filter = Builders<Product>.Filter.Where(p => p.Category == categoryName);
            return await _catalogContext.Products.Find(filter).ToListAsync();
        }

        public async Task Create(Product product) =>
            await _catalogContext.Products.InsertOneAsync(product);

        public async Task<bool> Delete(string id)
        {
            var deleteProd = await GetProduct(id);
            var result = false;
            if (deleteProd != null)
            {
                var deleteResult = await _catalogContext.Products.DeleteOneAsync(p => p.Id == id);
                result = deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;
            }
            return result;
        }

        public async Task<bool> Update(Product product)
        {
            var updateProd = await GetProduct(product.Id);
            var result = false;
            if (updateProd != null)
            {
                var replaceResult = await _catalogContext.Products.ReplaceOneAsync(p => p.Id == product.Id, product);
                result = replaceResult.IsAcknowledged && replaceResult.ModifiedCount > 0;
            }
            return result;
        }
    }
}