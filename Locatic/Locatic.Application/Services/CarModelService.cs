using Locatic.Application.DTOs;
using Locatic.Application.Interfaces;
using Locatic.Domain.Entities;
using Locatic.Domain.Interfaces;

namespace Locatic.Application.Services{

    public class CarModelService : ICarModelService
    {
        private readonly ICarModelRepository _repository;

        public CarModelService(ICarModelRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<CarModelDto>> GetAllAsync()
        {
            var models = await _repository.GetAllAsync();
            return models.Select(m => new CarModelDto { Id = m.Id, Name = m.Name, BrandId = m.BrandId, BrandName = m.Brand.Name });
        }

        public async Task<CarModelDto?> GetByIdAsync(int id)
        {
            var model = await _repository.GetByIdAsync(id);
            if (model == null) return null;
            return new CarModelDto { Id = model.Id, Name = model.Name, BrandId = model.BrandId, BrandName = model.Brand.Name };
        }

        public async Task CreateAsync(CreateCarModelDto dto)
        {
            var model = new CarModel { Name = dto.Name, BrandId = dto.BrandId };
            await _repository.AddAsync(model);
        }

        public async Task UpdateAsync(int id, CreateCarModelDto dto)
        {
            var model = await _repository.GetByIdAsync(id);
            if (model == null) return;
            model.Name = dto.Name;
            model.BrandId = dto.BrandId;
            await _repository.UpdateAsync(model);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }


}
