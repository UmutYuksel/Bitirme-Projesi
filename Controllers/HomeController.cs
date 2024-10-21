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

    // Constructor - Initializes DataContext
    public HomeController(DataContext context)
    {
        _context = context;
    }

    // GET: Index - Main page showing categories and cars
    public IActionResult Index()
    {
        var categories = _context.Categories
                .Select(category => new CategoryWithVehicleCountViewModel
                {
                    Category = category,
                    VehicleCount = _context.Cars.Count(v => v.CategoryId == category.CategoryId) // Get count of cars per category
                })
                .ToList();

        var carList = _context.Cars.ToList(); // Fetch the list of all cars

        var viewModel = new HomeIndexViewModel
        {
            Categories = categories,
            CarList = carList
        };

        return View(viewModel); // Send the ViewModel to the view
    }

    // GET: Details - Load car details page
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var car = await _context.Cars.FindAsync(id);
        if (car == null)
        {
            return NotFound(); // Car not found
        }

        // Fetch brand details based on BrandId
        var brand = await _context.Brands.FirstOrDefaultAsync(b => b.BrandId == car.BrandId);
        if (brand == null)
        {
            return NotFound(); // Brand not found
        }

        // Add BrandName to ViewBag
        ViewBag.BrandName = brand.BrandName;

        // Fetch HorsePower and MaxTorque from the model related to the car's brand
        car.HorsePower = _context.BrandModels.Where(m => m.BrandId == car.BrandId).Select(m => m.HorsePower).FirstOrDefault();
        car.MaxTorque = _context.BrandModels.Where(m => m.BrandId == car.BrandId).Select(m => m.MaxTorque).FirstOrDefault();

        return View(car); // Send the car model to the view
    }

    // POST: Delete - Deletes a car from the database
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var car = await _context.Cars.FindAsync(id);
        if (car == null)
        {
            return NotFound(); // Car not found
        }

        _context.Cars.Remove(car); // Remove car from database
        await _context.SaveChangesAsync(); // Save changes to the database
        return RedirectToAction("Index", "Home"); // Redirect to the homepage
    }

    // GET: Edit - Load the car edit page
    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound(); // Car ID is null
        }

        var car = await _context.Cars.FindAsync(id);
        if (car == null)
        {
            return NotFound(); // Car not found
        }

        // Fetch brand and category details based on IDs
        var brand = await _context.Brands.FirstOrDefaultAsync(b => b.BrandId == car.BrandId);
        var category = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryId == car.CategoryId);

        if (brand == null || category == null)
        {
            return NotFound(); // Brand or category not found
        }

        // Assign names to ViewBag
        ViewBag.BrandName = brand.BrandName;
        ViewBag.CategoryName = category.CategoryName;
        ViewBag.ModelId = car.ModelId;

        // Assign transmission type and other dropdown data
        ViewBag.TransmissionType = car.transmissionType;

        // Fetch HorsePower and MaxTorque
        car.HorsePower = _context.BrandModels.Where(m => m.BrandId == car.BrandId).Select(m => m.HorsePower).FirstOrDefault();
        car.MaxTorque = _context.BrandModels.Where(m => m.BrandId == car.BrandId).Select(m => m.MaxTorque).FirstOrDefault();

        return View(car); // Send the car model to the view
    }

    // POST: Edit - Handles car update
    [HttpPost]
    public async Task<IActionResult> Edit(CarCreate model, IFormFile imageFile, int? id)
    {
        var allowedExtensions = new[] { ".jpg", ".png", ".jpeg" };
        bool isValid = false;

        // Get existing car details from the database
        var existingCar = await _context.Cars.FindAsync(id);
        if (existingCar == null)
        {
            return NotFound(); // Car not found
        }

        // Preserve certain fields that the user cannot edit
        model.BrandId = existingCar.BrandId;
        model.CategoryId = existingCar.CategoryId;
        model.HorsePower = existingCar.HorsePower;
        model.MaxTorque = existingCar.MaxTorque;
        model.ModelName = existingCar.ModelName;

        // If a new image is uploaded, process it
        if (imageFile != null)
        {
            var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
            {
                ModelState.AddModelError("", "Please choose a valid image format.");
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
                    existingCar.Image = randomFileName; // Update car's image
                    isValid = true;
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred while uploading the file: " + ex.Message);
                }
            }
        }
        else
        {
            // Keep the existing image if no new one is uploaded
            isValid = true;
        }

        if (isValid)
        {
            // Update other editable fields
            existingCar.Year = model.Year;
            existingCar.Millage = model.Millage;
            existingCar.transmissionType = model.transmissionType;
            existingCar.Color = model.Color;
            existingCar.Description = model.Description;
            existingCar.Price = model.Price;
            existingCar.Title = model.Title;

            // Save changes to the database
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        // If the update fails, re-display the form with existing data
        ViewBag.BrandId = GetBrandSelectList();
        ViewBag.CategoryId = GetCategorySelectList();
        ViewBag.TransmissionType = GetTransmissionTypeSelectList();
        ViewBag.ModelId = GetBrandModelSelectList(model.ModelId);
        return View(model);
    }

    // AJAX: Get brands by category ID
    [HttpGet]
    public IActionResult GetBrandsByCategory(int categoryId)
    {
        var brands = _context.Brands
            .Where(b => b.CategoryId == categoryId) // Filter by CategoryId
            .Select(b => new SelectListItem
            {
                Value = b.BrandId.ToString(),
                Text = b.BrandName
            }).ToList();

        return Json(brands); // Return the list of brands as JSON
    }

    // AJAX: Get models by brand ID
    [HttpGet]
    public IActionResult GetModelsByBrand(int brandId)
    {
        var models = _context.BrandModels
            .Where(m => m.BrandId == brandId) // Filter by BrandId
            .Select(m => new SelectListItem
            {
                Value = m.ModelId.ToString(),
                Text = m.ModelName
            }).ToList();

        return Json(models); // Return the list of models as JSON
    }

    // Helper: Get brand list for dropdown
    private IEnumerable<SelectListItem> GetBrandSelectList()
    {
        return _context.Brands.Select(b => new SelectListItem
        {
            Value = b.BrandId.ToString(),
            Text = b.BrandName
        }).ToList();
    }

    // Helper: Get model list by brand ID for dropdown
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

    // Helper: Get category list for dropdown
    private IEnumerable<SelectListItem> GetCategorySelectList()
    {
        return _context.Categories.Select(c => new SelectListItem
        {
            Value = c.CategoryId.ToString(),
            Text = c.CategoryName
        }).ToList();
    }

    // Helper: Get transmission types for dropdown
    private IEnumerable<SelectListItem> GetTransmissionTypeSelectList()
    {
        return Enum.GetValues(typeof(TransmissionType)).Cast<TransmissionType>()
            .Select(t => new SelectListItem
            {
                Value = t.ToString(),
                Text = t.ToString()
            }).ToList();
    }

    // AJAX: Get horsepower and torque by model ID
    [HttpGet]
    public IActionResult GetHorsePowerAndTorqueByModel(int modelId)
    {
        var model = _context.BrandModels
            .Where(m => m.ModelId == modelId) // Filter by ModelId
            .Select(m => new
            {
                HorsePower = m.HorsePower,
                MaxTorque = m.MaxTorque
            }).FirstOrDefault();

        return Json(model); // Return HorsePower and MaxTorque as JSON
    }

    // AJAX: Filter ModelList by selectedCategory
    [HttpGet]
    public IActionResult GetCarsByCategory(int categoryId)
    {
        var cars = _context.Cars
            .Where(car => car.CategoryId == categoryId) // Seçilen kategoriye göre filtreleme
            .ToList();

        return PartialView("HomeListPartial", cars); // Filtrelenmiş araçları döndür
    }

}
