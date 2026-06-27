using Locatic.Application.DTOs;
using Locatic.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Locatic.Web.Controllers{

    public class BrandController : Controller
    {
        private readonly IBrandService _service;

        public BrandController(IBrandService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            var brands = await _service.GetAllAsync();
            return View(brands);
        }

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(CreateBrandDto dto)
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