using Locatic.Application.DTOs;
using Locatic.Application.Interfaces;
using Locatic.Domain.Entities;
using Locatic.Domain.Interfaces;

namespace Locatic.Application.Services{

    public class ReservationService : IReservationService
    {
        private readonly IReservationRepository _repository;


        public ReservationService(IReservationRepository repository)
        {
            _repository = repository;
        }



        public async Task<IEnumerable<ReservationDto>> GetAllAsync()
        {
            var reservations = await _repository.GetAllAsync();
            return reservations.Select(r => new ReservationDto
            {
                Id = r.Id,
                StartDate = r.StartDate,
                EndDate = r.EndDate,
                CarId = r.CarId,
                LicensePlate = r.Car.LicensePlate,
                ClientId = r.ClientId,
                ClientName = r.Client.FirstName + " " + r.Client.LastName
            });
        }



        public async Task CreateAsync(CreateReservationDto dto)
        {
            if (dto.EndDate <= dto.StartDate)
                throw new Exception("La date de fin doit être après la date de début.");

            var available = await _repository.IsCarAvailableAsync(dto.CarId, dto.StartDate, dto.EndDate);
            if (!available)
                throw new Exception("Cette voiture est déjà réservée sur cette période.");

            var reservation = new Reservation
            {
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                CarId = dto.CarId,
                ClientId = dto.ClientId
            };
            await _repository.AddAsync(reservation);
        }



        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }


}
