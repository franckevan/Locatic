using Locatic.Application.DTOs;
using Locatic.Application.Services;
using Locatic.Domain.Entities;
using Locatic.Domain.Enums;
using Locatic.Domain.Interfaces;
using Moq;
using Xunit;

namespace Locatic.Tests.Services
{
    public class CarServiceTests
    {
        private static Car BuildCar(int id = 1)
        {
            return new Car
            {
                Id = id,
                LicensePlate = "AA-111-BB",
                Year = 2023,
                DailyRate = 35m,
                Seats = 5,
                FuelType = "Essence",
                Mileage = 12000,
                Color = "Blanc",
                ImageUrl = string.Empty,
                Status = CarStatus.Available,
                CarModelId = 1,
                CarModel = new CarModel
                {
                    Id = 1,
                    Name = "Yaris",
                    BrandId = 1,
                    Brand = new Brand { Id = 1, Name = "Toyota", Country = "Japon" }
                }
            };
        }

        [Fact]
        public async Task GetAllAsync_MapsBrandAndModelNames()
        {
            var repository = new Mock<ICarRepository>();
            repository.Setup(r => r.GetAllAsync()).ReturnsAsync(new[] { BuildCar() });
            var service = new CarService(repository.Object);

            var result = (await service.GetAllAsync()).Single();

            Assert.Equal("Toyota", result.BrandName);
            Assert.Equal("Yaris", result.ModelName);
            Assert.Equal("AA-111-BB", result.LicensePlate);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenCarDoesNotExist()
        {
            var repository = new Mock<ICarRepository>();
            repository.Setup(r => r.GetByIdAsync(42)).ReturnsAsync((Car?)null);
            var service = new CarService(repository.Object);

            var result = await service.GetByIdAsync(42);

            Assert.Null(result);
        }

        [Fact]
        public async Task CreateAsync_PersistsCarWithDtoValues()
        {
            var repository = new Mock<ICarRepository>();
            Car? savedCar = null;
            repository.Setup(r => r.AddAsync(It.IsAny<Car>()))
                .Callback<Car>(c => savedCar = c)
                .Returns(Task.CompletedTask);
            var service = new CarService(repository.Object);

            var dto = new CreateCarDto
            {
                LicensePlate = "CC-222-DD",
                Year = 2022,
                DailyRate = 28m,
                Seats = 5,
                FuelType = "Diesel",
                Mileage = 25000,
                Color = "Gris",
                ImageUrl = null,
                Status = CarStatus.Maintenance,
                CarModelId = 1
            };

            await service.CreateAsync(dto);

            repository.Verify(r => r.AddAsync(It.IsAny<Car>()), Times.Once);
            Assert.NotNull(savedCar);
            Assert.Equal("CC-222-DD", savedCar!.LicensePlate);
            Assert.Equal(CarStatus.Maintenance, savedCar.Status);
            Assert.Equal(string.Empty, savedCar.ImageUrl);
        }

        [Fact]
        public async Task DeleteAsync_DelegatesToRepository()
        {
            var repository = new Mock<ICarRepository>();
            var service = new CarService(repository.Object);

            await service.DeleteAsync(7);

            repository.Verify(r => r.DeleteAsync(7), Times.Once);
        }
    }
}
