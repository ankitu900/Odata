using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business.Repositories;
using Odata.Entities; // Replace with the actual namespace for your Product entity

namespace Business.Services
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllAsync();
        IQueryable<Product> All();
        Task<Product?> GetByIdAsync(int id);
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(int id);
    }

    public class ProductService : IProductService
    {
        private readonly IRepository<Product> _productRepository;

        public ProductService(IRepository<Product> productRepository)
        {
            _productRepository = productRepository;
        }

        public IQueryable<Product> All()
        {
            // This method returns an IQueryable for further querying
            return _productRepository.All();
        }
        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _productRepository.AllAsync();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            // Assuming Id is the key property
            return await _productRepository.FindSingleAsync(p => p.Id == id);
        }

        public async Task AddAsync(Product product)
        {
            _productRepository.Add(product);
            await _productRepository.SaveChangesAsync();
        }

        public async Task UpdateAsync(Product product)
        {
            _productRepository.Update(product);
            await _productRepository.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _productRepository.FindSingleAsync(p => p.Id == id);
            if (product != null)
            {
                _productRepository.Remove(product);
                // Remove already calls SaveChangesAsync internally, so no need to call again
            }
        }
    }
}
