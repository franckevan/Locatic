using Locatic.Domain.Enums;

namespace Locatic.Domain.Entities
{

    public class Car
    {
        public int Id { get; set; }

        public string LicensePlate { get; set;} = string.Empty ;

        public int Year { get; set; }

        public decimal DailyRate { get ; set; }

        public int Seats { get; set;}

        public string FuelType { get; set; } = string.Empty ;

        public int Mileage { get; set; }

        public string Color { get; set; } = string.Empty;

        public string ImageUrl { get; set; } = string.Empty;

        public CarStatus Status { get; set; } = CarStatus.Available;

        public int CarModelId { get; set;}

        public CarModel CarModel { get; set; } = null! ;

        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}