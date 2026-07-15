using Locatic.Application.DTOs;

namespace Locatic.Application.Interfaces
{

    public interface IClientService
    {
        Task<IEnumerable<ClientDto>> GetAllAsync();

        Task<ClientDto?> GetByIdAsync(int id);

        Task CreateAsync(CreateClientDto dto);

        Task DeleteAsync(int id);
    }


}
