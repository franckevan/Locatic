using Locatic.Domain.Enums;

namespace Locatic.Application.DTOs{
    public class CarDto
    {
        public int Id { get; set; }
        public string LicensePlate { get; set; } = string.Empty;
        public int Year { get; set; }
        public decimal DailyRate { get; set; }
        public int Seats { get; set; }
        public string FuelType { get; set; } = string.Empty;
        public int Mileage { get; set; }
        public string Color { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public CarStatus Status { get; set; }
        public int CarModelId { get; set; }
        public string ModelName { get; set; } = string.Empty;
        public string BrandName { get; set; } = string.Empty;
    }
}
