using Locatic.Application.DTOs;
using Locatic.Application.Interfaces;
using Locatic.Domain.Entities;
using Locatic.Domain.Interfaces;

namespace Locatic.Application.Services
{

    public class ClientService : IClientService
    {
        private readonly IClientRepository _repository;

        public ClientService(IClientRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<ClientDto>> GetAllAsync()
        {
            var clients = await _repository.GetAllAsync();
            return clients.Select(c => new ClientDto
            {
                Id = c.Id,
                FirstName = c.FirstName,
                LastName = c.LastName,
                Email = c.Email,
                PhoneNumber = c.PhoneNumber
            });
        }

        public async Task<ClientDto?> GetByIdAsync(int id)
        {
            var c = await _repository.GetByIdAsync(id);
            if (c == null) return null;
            return new ClientDto { Id = c.Id, FirstName = c.FirstName, LastName = c.LastName, Email = c.Email, PhoneNumber = c.PhoneNumber };
        }

        public async Task CreateAsync(CreateClientDto dto)
        {
            var client = new Client { FirstName = dto.FirstName, LastName = dto.LastName, Email = dto.Email, PhoneNumber = dto.PhoneNumber };
            await _repository.AddAsync(client);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }


}
