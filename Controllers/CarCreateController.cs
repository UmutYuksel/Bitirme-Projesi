using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using BitirmeProjesi.Models;
using System.Linq;
using BitirmeProjesi.Data;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

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
        [HttpGet]
        public IActionResult Create()
        {

            // Kategoriler için varsayılan seçenekle birlikte SelectList oluşturuyoruz
            var categories = GetCategorySelectList().ToList();
            categories.Insert(0, new SelectListItem { Value = "", Text = "Kategori Seçiniz" });

            ViewBag.CategoryId = categories;
            ViewBag.BrandId = new SelectList(Enumerable.Empty<SelectListItem>());
            ViewBag.TransmissionType = GetTransmissionTypeSelectList();
            ViewBag.ModelId = new SelectList(Enumerable.Empty<SelectListItem>());

            return View(new CarCreate());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CarCreate model, IFormFile imageFile)
        {
            var allowedExtensions = new[] { ".jpg", ".png", ".jpeg" };
            bool IsValid = false;

            if (imageFile != null)
            {
                var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("", "Geçerli bir resim türü seçiniz.");
                }
                else
                {
                    var randomFileName = $"{Guid.NewGuid()}{extension}";
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", randomFileName);

                    try
                    {
                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(stream);
                        }
                        model.Image = randomFileName;
                        IsValid = true;
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "Dosya yüklenirken bir hata oluştu: " + ex.Message);
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "Bir resim seçiniz!");
            }

            if (IsValid)
            {
                var newCarCreate = new CarCreate
                {
                    BrandId = model.BrandId,
                    CategoryId = model.CategoryId,
                    Year = model.Year,
                    Millage = model.Millage,
                    HorsePower = model.HorsePower,
                    MaxTorque = model.MaxTorque,
                    Color = model.Color,
                    transmissionType = model.transmissionType,
                    Image = model.Image,
                    Description = model.Description,
                    Price = model.Price,
                    AdvertTime = DateTime.Now,
                    ModelName = model.ModelName,
                    Title = model.Title
                };

                await _context.Cars.AddAsync(newCarCreate);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }


            ViewBag.BrandId = GetBrandSelectList();
            ViewBag.CategoryId = GetCategorySelectList();
            ViewBag.TransmissionType = GetTransmissionTypeSelectList();
            ViewBag.ModelId = GetBrandModelSelectList(model.ModelId);
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

        [HttpGet]
        public IActionResult GetHorsePowerAndTorqueByModel(int modelId)
        {
            var model = _context.BrandModels
                .Where(m => m.ModelId == modelId)
                .Select(m => new
                {
                    m.HorsePower,
                    m.MaxTorque
                })
                .FirstOrDefault();

            return Json(model);
        }
    }
}
