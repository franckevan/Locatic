using Locatic.Application.DTOs;
using Locatic.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Locatic.Web.Controllers{

    public class ReservationController : Controller
    {
        private readonly IReservationService _service;
        private readonly ICarService _carService;
        private readonly IClientService _clientService;

        public ReservationController(IReservationService service, ICarService carService, IClientService clientService)
        {
            _service = service;
            _carService = carService;
            _clientService = clientService;
        }

        public async Task<IActionResult> Index()
        {
            var reservations = await _service.GetAllAsync();
            return View(reservations);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Cars = await _carService.GetAllAsync();
            ViewBag.Clients = await _clientService.GetAllAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateReservationDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Cars = await _carService.GetAllAsync();
                ViewBag.Clients = await _clientService.GetAllAsync();
                return View(dto);
            }
            try
            {
                await _service.CreateAsync(dto);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                ViewBag.Cars = await _carService.GetAllAsync();
                ViewBag.Clients = await _clientService.GetAllAsync();
                return View(dto);
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }


}
