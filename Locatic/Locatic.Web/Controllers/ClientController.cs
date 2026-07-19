using Locatic.Application.DTOs;
using Locatic.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Locatic.Web.Controllers
{

    public class ClientController : Controller
    {
        private readonly IClientService _service;

        public ClientController(IClientService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            var clients = await _service.GetAllAsync();
            return View(clients);
        }

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(CreateClientDto dto)
        {
            if (!ModelState.IsValid) return View(dto);
            await _service.CreateAsync(dto);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }


}
