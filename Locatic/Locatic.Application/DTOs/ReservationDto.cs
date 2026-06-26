namespace Locatic.Application.DTOs{

    public class ReservationDto
    {
        public int Id { get; set; }


        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int CarId { get; set; }

        public string LicensePlate { get; set; } = string.Empty;

        
        public int ClientId { get; set; }

        public string ClientName { get; set; } = string.Empty;

    }


}
