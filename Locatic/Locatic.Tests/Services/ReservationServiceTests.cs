using Locatic.Application.DTOs;
using Locatic.Application.Services;
using Locatic.Domain.Interfaces;
using Moq;
using Xunit;

namespace Locatic.Tests.Services
{
    public class ReservationServiceTests
    {
        [Fact]
        public async Task CreateAsync_Throws_WhenEndDateIsNotAfterStartDate()
        {
            var repository = new Mock<IReservationRepository>();
            var service = new ReservationService(repository.Object);
            var dto = new CreateReservationDto
            {
                StartDate = new DateTime(2026, 7, 10),
                EndDate = new DateTime(2026, 7, 10),
                CarId = 1,
                ClientId = 1
            };

            await Assert.ThrowsAsync<Exception>(() => service.CreateAsync(dto));
            repository.Verify(r => r.AddAsync(It.IsAny<Locatic.Domain.Entities.Reservation>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_Throws_WhenCarIsAlreadyReservedOnPeriod()
        {
            var repository = new Mock<IReservationRepository>();
            repository.Setup(r => r.IsCarAvailableAsync(1, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(false);
            var service = new ReservationService(repository.Object);
            var dto = new CreateReservationDto
            {
                StartDate = new DateTime(2026, 7, 10),
                EndDate = new DateTime(2026, 7, 12),
                CarId = 1,
                ClientId = 1
            };

            await Assert.ThrowsAsync<Exception>(() => service.CreateAsync(dto));
            repository.Verify(r => r.AddAsync(It.IsAny<Locatic.Domain.Entities.Reservation>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_PersistsReservation_WhenCarIsAvailable()
        {
            var repository = new Mock<IReservationRepository>();
            repository.Setup(r => r.IsCarAvailableAsync(1, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(true);
            var service = new ReservationService(repository.Object);
            var dto = new CreateReservationDto
            {
                StartDate = new DateTime(2026, 7, 10),
                EndDate = new DateTime(2026, 7, 12),
                CarId = 1,
                ClientId = 1
            };

            await service.CreateAsync(dto);

            repository.Verify(r => r.AddAsync(It.Is<Locatic.Domain.Entities.Reservation>(
                res => res.CarId == 1 && res.ClientId == 1)), Times.Once);
        }
    }
}
