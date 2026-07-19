namespace Locatic.Domain.Entities
{

    public class CarModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int BrandId { get; set; }

        public Brand Brand { get; set; } = null!;

        public ICollection<Car> Cars { get; set; } = new List<Car>();


    }
}