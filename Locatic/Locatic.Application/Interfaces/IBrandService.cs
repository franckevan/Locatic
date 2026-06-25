using Locatic.Application.DTOs;

namespace Locatic.Application.Interfaces{

    public interface IBrandService
    {
        Task<IEnumerable<BrandDto>> GetAllAsync();

        Task<BrandDto?> GetByIdAsync(int id);

        Task CreateAsync(CreateBrandDto dto);

        Task UpdateAsync(int id, CreateBrandDto dto);


        Task DeleteAsync(int id);
    }

}