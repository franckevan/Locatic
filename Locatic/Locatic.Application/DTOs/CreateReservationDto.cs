namespace Locatic.Application.DTOs{




    public class CreateReservationDto
    {
        public DateTime StartDate { get; set; }


        public DateTime EndDate { get; set; }
        
        public int CarId { get; set; }
        public int ClientId { get; set; }


    }


}
