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

        public int CarModelId { get; set;} 

        public CarModel CarModel { get; set; } = null! ;

        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}