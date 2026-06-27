namespace Locatic.Application.DTOs{


    public class CreateCarDto
    {
        public string LicensePlate { get; set; } = string.Empty;

        public int Year { get; set; }

        public decimal DailyRate { get; set; }

        public int Seats { get; set; }

        public string FuelType { get; set; } = string.Empty;
        
        public int CarModelId { get; set; }
    }


}
