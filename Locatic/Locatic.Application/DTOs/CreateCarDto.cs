using Locatic.Domain.Enums;

namespace Locatic.Application.DTOs{


    public class CreateCarDto
    {
        public string LicensePlate { get; set; } = string.Empty;

        public int Year { get; set; }

        public decimal DailyRate { get; set; }

        public int Seats { get; set; }

        public string FuelType { get; set; } = string.Empty;

        public int Mileage { get; set; }

        public string Color { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }

        public CarStatus Status { get; set; } = CarStatus.Available;

        public int CarModelId { get; set; }
    }


}