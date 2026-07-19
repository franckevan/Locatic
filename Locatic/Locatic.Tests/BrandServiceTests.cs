using Locatic.Application.DTOs;
using Locatic.Application.Services;
using Locatic.Domain.Entities;
using Locatic.Domain.Interfaces;
using Moq;
using Xunit;

namespace Locatic.Tests
{
    public class BrandServiceTests
    {
        [Fact]
        public async Task GetAllAsync_ReturnsAllBrandsAsDto()
        {
            var repoMock = new Mock<IBrandRepository>();
            repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Brand>
            {
                new Brand { Id = 1, Name = "Peugeot", Country = "France" },
                new Brand { Id = 2, Name = "Toyota", Country = "Japon" }
            });
            var service = new BrandService(repoMock.Object);

            var result = await service.GetAllAsync();

            Assert.Equal(2, result.Count());
            Assert.Contains(result, b => b.Name == "Peugeot");
        }

        [Fact]
        public async Task GetByIdAsync_ExistingId_ReturnsDto()
        {
            var repoMock = new Mock<IBrandRepository>();
            repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Brand { Id = 1, Name = "Renault", Country = "France" });
            var service = new BrandService(repoMock.Object);

            var result = await service.GetByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal("Renault", result!.Name);
        }

        [Fact]
        public async Task GetByIdAsync_UnknownId_ReturnsNull()
        {
            var repoMock = new Mock<IBrandRepository>();
            repoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Brand?)null);
            var service = new BrandService(repoMock.Object);

            var result = await service.GetByIdAsync(99);

            Assert.Null(result);
        }

        [Fact]
        public async Task CreateAsync_CallsRepositoryAddAsync()
        {
            var repoMock = new Mock<IBrandRepository>();
            var service = new BrandService(repoMock.Object);
            var dto = new CreateBrandDto { Name = "Fiat", Country = "Italie" };

            await service.CreateAsync(dto);

            repoMock.Verify(r => r.AddAsync(It.Is<Brand>(b => b.Name == "Fiat" && b.Country == "Italie")), Times.Once);
        }
    }
}
