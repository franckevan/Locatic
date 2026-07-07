using Locatic.Application.DTOs;
using Locatic.Application.Interfaces;
using Locatic.Domain.Entities;
using Locatic.Domain.Interfaces;
namespace Locatic.Application.Services{
    public class CarService : ICarService
    {
        private readonly ICarRepository _repository;
        public CarService(ICarRepository repository)
        {
            _repository = repository;
        }
        public async Task<IEnumerable<CarDto>> GetAllAsync()
        {
            var cars = await _repository.GetAllAsync();
            return cars.Select(c => new CarDto
            {
                Id = c.Id,
                LicensePlate = c.LicensePlate,
                Year = c.Year,
                DailyRate = c.DailyRate,
                Seats = c.Seats,
                FuelType = c.FuelType,
                Mileage = c.Mileage,
                Color = c.Color,
                ImageUrl = c.ImageUrl,
                Status = c.Status,
                CarModelId = c.CarModelId,
                ModelName = c.CarModel.Name,
                BrandName = c.CarModel.Brand.Name
            });
        }
        public async Task<CarDto?> GetByIdAsync(int id)
        {
            var c = await _repository.GetByIdAsync(id);
            if (c == null) return null;
            return new CarDto
            {
                Id = c.Id,
                LicensePlate = c.LicensePlate,
                Year = c.Year,
                DailyRate = c.DailyRate,
                Seats = c.Seats,
                FuelType = c.FuelType,
                Mileage = c.Mileage,
                Color = c.Color,
                ImageUrl = c.ImageUrl,
                Status = c.Status,
                CarModelId = c.CarModelId,
                ModelName = c.CarModel.Name,
                BrandName = c.CarModel.Brand.Name
            };
        }
        public async Task CreateAsync(CreateCarDto dto)
        {
            var car = new Car
            {
                LicensePlate = dto.LicensePlate,
                Year = dto.Year,
                DailyRate = dto.DailyRate,
                Seats = dto.Seats,
                FuelType = dto.FuelType,
                Mileage = dto.Mileage,
                Color = dto.Color,
                ImageUrl = dto.ImageUrl ?? string.Empty,
                Status = dto.Status,
                CarModelId = dto.CarModelId
            };
            await _repository.AddAsync(car);
        }
        public async Task UpdateAsync(int id, CreateCarDto dto)
        {
            var car = await _repository.GetByIdAsync(id);
            if (car == null) return;
            car.LicensePlate = dto.LicensePlate;
            car.Year = dto.Year;
            car.DailyRate = dto.DailyRate;
            car.Seats = dto.Seats;
            car.FuelType = dto.FuelType;
            car.Mileage = dto.Mileage;
            car.Color = dto.Color;
            car.ImageUrl = dto.ImageUrl ?? string.Empty;
            car.Status = dto.Status;
            car.CarModelId = dto.CarModelId;
            await _repository.UpdateAsync(car);
        }
        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}