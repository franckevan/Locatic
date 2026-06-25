namespace Locatic.Application.DTOs{

    public class CarModelDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int BrandId { get; set; }
        public string BrandName { get; set; } = string.Empty;
    }


}