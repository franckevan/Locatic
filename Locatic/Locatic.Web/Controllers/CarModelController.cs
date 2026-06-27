using Locatic.Application.DTOs;
using Locatic.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Locatic.Web.Controllers{

    public class CarModelController : Controller
    {
        private readonly ICarModelService _service;
        private readonly IBrandService _brandService;

        public CarModelController(ICarModelService service, IBrandService brandService)
        {
            _service = service;
            _brandService = brandService;
        }

        public async Task<IActionResult> Index()
        {
            var models = await _service.GetAllAsync();
            return View(models);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Brands = await _brandService.GetAllAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCarModelDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Brands = await _brandService.GetAllAsync();
                return View(dto);
            }
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