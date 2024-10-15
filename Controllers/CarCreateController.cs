using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using BitirmeProjesi.Models;
using System.Linq;
using BitirmeProjesi.Data;
using System.Collections.Generic;

namespace BitirmeProjesi.Controllers
{
    public class CarCreateController : Controller
    {
        private readonly DataContext _context;

        public CarCreateController(DataContext context)
        {
            _context = context;
        }

        // GET: Car/Create
        public IActionResult Create()
        {
            ViewBag.BrandId = new SelectList(Enumerable.Empty<SelectListItem>());
            ViewBag.CategoryId = GetCategorySelectList();
            ViewBag.TransmissionType = GetTransmissionTypeSelectList();
            ViewBag.ModelId = new SelectList(Enumerable.Empty<SelectListItem>());
            return View(new CarCreate());
        }

        // POST: Car/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CarCreate model)
        {
            if (ModelState.IsValid)
            {
                var newBrandModel = new BrandModel
                {
                    BrandId = model.BrandId,
                    ModelName = model.ModelName,
                    CategoryId = model.CategoryId
                };

                var newModelDetails = new ModelDetails
                {
                    Year = model.Year,
                    Millage = model.Millage,
                    HorsePower = model.HorsePower,
                    MaxTorque = model.MaxTorque,
                    Color = model.Color,
                    transmissionType = model.transmissionType
                };

                _context.BrandModels.Add(newBrandModel);
                _context.ModelDetails.Add(newModelDetails);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            // Hata durumunda verileri yeniden yükle
            ViewBag.BrandId = GetBrandSelectList();
            ViewBag.CategoryId = GetCategorySelectList();
            ViewBag.TransmissionType = GetTransmissionTypeSelectList();
            ViewBag.ModelId = GetBrandModelSelectList(model.BrandId);

            return View(model);
        }

        [HttpGet]
        public IActionResult GetBrandsByCategory(int categoryId)
        {
            var brands = _context.Brands
                .Where(b => b.CategoryId == categoryId) // CategoryId'ye göre filtreleme
                .Select(b => new SelectListItem
                {
                    Value = b.BrandId.ToString(),
                    Text = b.BrandName
                }).ToList();

            return Json(brands);
        }

        [HttpGet]
        public IActionResult GetModelsByBrand(int brandId)
        {
            var models = _context.BrandModels
                .Where(m => m.BrandId == brandId) // BrandId'ye göre filtreleme
                .Select(m => new SelectListItem
                {
                    Value = m.ModelId.ToString(),
                    Text = m.ModelName
                }).ToList();

            return Json(models);
        }

        private IEnumerable<SelectListItem> GetBrandSelectList()
        {
            return _context.Brands.Select(b => new SelectListItem
            {
                Value = b.BrandId.ToString(),
                Text = b.BrandName
            }).ToList();
        }

        private IEnumerable<SelectListItem> GetBrandModelSelectList(int brandId)
        {
            return _context.BrandModels
                .Where(b => b.BrandId == brandId)
                .Select(b => new SelectListItem
                {
                    Value = b.ModelId.ToString(),
                    Text = b.ModelName
                }).ToList();
        }

        private IEnumerable<SelectListItem> GetCategorySelectList()
        {
            return _context.Categories.Select(c => new SelectListItem
            {
                Value = c.CategoryId.ToString(),
                Text = c.CategoryName
            }).ToList();
        }

        private IEnumerable<SelectListItem> GetTransmissionTypeSelectList()
        {
            return Enum.GetValues(typeof(TransmissionType)).Cast<TransmissionType>()
                .Select(t => new SelectListItem
                {
                    Value = t.ToString(),
                    Text = t.ToString()
                }).ToList();
        }
    }
}
