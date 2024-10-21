using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BitirmeProjesi.Models;
using BitirmeProjesi.Data;
using BitirmeProjesi.ViewModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BitirmeProjesi.Controllers;

public class HomeController : Controller
{
    private readonly DataContext _context;

    public HomeController(DataContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var categories = _context.Categories
                .Select(category => new CategoryWithVehicleCountViewModel
                {
                    Category = category,
                    VehicleCount = _context.Cars.Count(v => v.CategoryId == category.CategoryId) // Araç sayısını al
                })
                .ToList();

        var carList = _context.Cars.ToList(); // Araçlarınızı alın

        var viewModel = new HomeIndexViewModel
        {
            Categories = categories,
            CarList = carList
        };

        return View(viewModel); // Modeli view'a gönder
    }

    // Detay sayfası için aksiyon,
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {

        var car = await _context.Cars.FindAsync(id);
        if (car == null)
        {
            return NotFound();
        }

        // BrandId'ye göre isimleri veritabanından çekme
        var brand = await _context.Brands.FirstOrDefaultAsync(b => b.BrandId == car.BrandId);

        if (brand == null)
        {
            return NotFound();
        }

        // BrandName ve CategoryName ViewBag'e atanıyor
        ViewBag.BrandName = brand.BrandName;

        // HorsePower ve MaxTorque verilerini de ekliyoruz
        car.HorsePower = _context.BrandModels.Where(m => m.BrandId == car.BrandId).Select(m => m.HorsePower).FirstOrDefault();
        car.MaxTorque = _context.BrandModels.Where(m => m.BrandId == car.BrandId).Select(m => m.MaxTorque).FirstOrDefault();

        return View(car);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var car = await _context.Cars.FindAsync(id);
        if (car == null)
        {
            return NotFound();
        }

        _context.Cars.Remove(car);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var car = await _context.Cars.FindAsync(id);
        if (car == null)
        {
            return NotFound();
        }

        // BrandId ve CategoryId'ye göre isimleri veritabanından çekme
        var brand = await _context.Brands.FirstOrDefaultAsync(b => b.BrandId == car.BrandId);
        var category = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryId == car.CategoryId);

        if (brand == null || category == null)
        {
            return NotFound();
        }

        // BrandName ve CategoryName ViewBag'e atanıyor
        ViewBag.BrandName = brand.BrandName;
        ViewBag.CategoryName = category.CategoryName;
        ViewBag.ModelId = car.ModelId;

        // Diğer dropdown verileri
        ViewBag.TransmissionType = car.transmissionType;

        // HorsePower ve MaxTorque verilerini de ekliyoruz
        car.HorsePower = _context.BrandModels.Where(m => m.BrandId == car.BrandId).Select(m => m.HorsePower).FirstOrDefault();
        car.MaxTorque = _context.BrandModels.Where(m => m.BrandId == car.BrandId).Select(m => m.MaxTorque).FirstOrDefault();

        return View(car);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(CarCreate model, IFormFile imageFile, int? id)
    {
        var allowedExtensions = new[] { ".jpg", ".png", ".jpeg" };
        bool isValid = false;

        // Veritabanındaki mevcut aracın bilgilerini alın
        var existingCar = await _context.Cars.FindAsync(id); // ModelId ile kaydı buluyoruz
        if (existingCar == null)
        {
            return NotFound();
        }

        // Kullanıcı tarafından düzenlenemeyen alanları koruyoruz
        model.BrandId = existingCar.BrandId;
        model.CategoryId = existingCar.CategoryId;
        model.HorsePower = existingCar.HorsePower;
        model.MaxTorque = existingCar.MaxTorque;
        model.ModelName = existingCar.ModelName;

        // Eğer yeni bir resim dosyası yüklenmişse, işlemi gerçekleştiriyoruz
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
                    existingCar.Image = randomFileName;  // Mevcut aracın resmini güncelliyoruz
                    isValid = true;
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Dosya yüklenirken bir hata oluştu: " + ex.Message);
                }
            }
        }
        else
        {
            // Resim yüklenmediyse mevcut resmi koruyoruz
            isValid = true;
        }

        if (isValid)
        {
            // Diğer düzenlenebilir alanları güncelliyoruz
            existingCar.Year = model.Year;
            existingCar.Millage = model.Millage;
            existingCar.transmissionType = model.transmissionType;
            existingCar.Color = model.Color;
            existingCar.Description = model.Description;
            existingCar.Price = model.Price;
            existingCar.Title = model.Title;

            // Veritabanındaki değişiklikleri kaydediyoruz
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        // Veriler güncellenmezse formu tekrar gösteriyoruz
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

    // Aracın mevcut olup olmadığını kontrol etmek için
    private bool CarExists(int id)
    {
        return _context.Cars.Any(e => e.ModelId == id);
    }

    public IActionResult CategoryTreePartial()
    {
        var categories = _context.Categories
            .Select(category => new CategoryWithVehicleCountViewModel
            {
                Category = category,
                VehicleCount = _context.Cars.Count(v => v.CategoryId == category.CategoryId) // Araç sayısını al
            })
            .ToList();

        return View(categories); // Modelleri view'a gönder
    }

    [HttpGet]
    public IActionResult GetCarsByCategory(int categoryId)
    {
        var cars = _context.Cars
            .Where(car => car.CategoryId == categoryId) // Seçilen kategoriye göre filtreleme
            .ToList();

        return PartialView("HomeListPartial", cars); // Filtrelenmiş araçları döndür
    }

}
