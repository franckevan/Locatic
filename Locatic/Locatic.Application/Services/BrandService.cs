using Locatic.Application.DTOs;
using Locatic.Application.Interfaces;
using Locatic.Domain.Entities;
using Locatic.Domain.Interfaces;

namespace Locatic.Application.Services{

    public class BrandService : IBrandService
    {
        private readonly IBrandRepository _repository;

        public BrandService(IBrandRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<BrandDto>> GetAllAsync()
        {
            var brands = await _repository.GetAllAsync();

            return brands.Select(b => new BrandDto { Id = b.Id, Name = b.Name, Country = b.Country });
        }

        public async Task<BrandDto?> GetByIdAsync(int id)
        {
            var brand = await _repository.GetByIdAsync(id);
            if (brand == null) return null;

            return new BrandDto { Id = brand.Id, Name = brand.Name, Country = brand.Country };
        }

        public async Task CreateAsync(CreateBrandDto dto)
        {
            var brand = new Brand { Name = dto.Name, Country = dto.Country };
            await _repository.AddAsync(brand);
        }

        public async Task UpdateAsync(int id, CreateBrandDto dto)
        {
            var brand = await _repository.GetByIdAsync(id);
            if (brand == null) return;
            
            brand.Name = dto.Name;
            brand.Country = dto.Country;

            await _repository.UpdateAsync(brand);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }


}