using Locatic.Application.DTOs;
using Locatic.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
namespace Locatic.Web.Controllers{
    public class CarController : Controller
    {
        private readonly ICarService _service;
        private readonly ICarModelService _carModelService;
        public CarController(ICarService service, ICarModelService carModelService)
        {
            _service = service;
            _carModelService = carModelService;
        }
        public async Task<IActionResult> Index()
        {
            var cars = await _service.GetAllAsync();
            return View(cars);
        }
        public async Task<IActionResult> Details(int id)
        {
            var car = await _service.GetByIdAsync(id);
            if (car == null) return NotFound();
            return View(car);
        }
        public async Task<IActionResult> Create()
        {
            ViewBag.CarModels = await _carModelService.GetAllAsync();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateCarDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.CarModels = await _carModelService.GetAllAsync();
                return View(dto);
            }
            await _service.CreateAsync(dto);
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Edit(int id)
        {
            var car = await _service.GetByIdAsync(id);
            if (car == null) return NotFound();
            ViewBag.CarModels = await _carModelService.GetAllAsync();
            var dto = new CreateCarDto
            {
                LicensePlate = car.LicensePlate,
                Year = car.Year,
                DailyRate = car.DailyRate,
                Seats = car.Seats,
                FuelType = car.FuelType,
                CarModelId = car.CarModelId
            };
            return View(dto);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, CreateCarDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.CarModels = await _carModelService.GetAllAsync();
                return View(dto);
            }
            await _service.UpdateAsync(id, dto);
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}