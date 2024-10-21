using BitirmeProjesi.Data;
using BitirmeProjesi.Models;
using BitirmeProjesi.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace BitirmeProjesi.Controllers
{
    public class CreateController : Controller
    {
        private readonly DataContext _context;

        public CreateController(DataContext context)
        {
            _context = context;
        }

        // GET: Create - Initial page load for creating a car
        [HttpGet]
        public IActionResult Create(int? categoryId)
        {
            var viewModel = new CreateViewModel
            {
                Categories = _context.Categories.ToList(),
                Brands = _context.Brands.ToList(),
                BrandModels = _context.BrandModels.ToList(),
                CarList = _context.Cars.ToList(),
            };

            var categories = _context.Categories.ToList();
            if (categories == null || !categories.Any())
            {
                ModelState.AddModelError("", "Kategoriler yüklenemedi.");
                ViewBag.Categories = new SelectList(Enumerable.Empty<Category>(), "CategoryId", "CategoryName");
            }
            else
            {
                ViewBag.Categories = new SelectList(categories, "CategoryId", "CategoryName");
            }

            if (categoryId.HasValue && categoryId > 0)
            {
                ViewBag.Brands = new SelectList(_context.Brands.Where(b => b.CategoryId == categoryId), "BrandId", "BrandName");
            }
            else
            {
                ViewBag.Brands = new SelectList(Enumerable.Empty<Brand>(), "BrandId", "BrandName");
            }

            return View(viewModel);
        }

        // GET: Fetches brands based on selected category
        [HttpGet]
        public IActionResult GetBrandsByCategory(int categoryId)
        {
            var brands = _context.Brands
                .Where(b => b.CategoryId == categoryId)
                .Select(b => new { b.BrandId, b.BrandName })
                .ToList();

            return Json(brands);
        }

        // POST: Create a new brand under the selected category
        [HttpPost]
        public async Task<IActionResult> CreateBrandPartial(Brand brand, int categoryId)
        {
            if (categoryId <= 0)
            {
                ModelState.AddModelError("selectedCategoryId", "Lütfen geçerli bir kategori seçin.");
                return PartialView("CreateBrandPartial", brand);
            }

            if (ModelState.IsValid)
            {
                var brandData = new Brand
                {
                    BrandName = brand.BrandName,
                    CategoryId = categoryId
                };

                await _context.Brands.AddAsync(brandData);
                await _context.SaveChangesAsync();
                return RedirectToAction("Create", "Create");
            }
            else
            {
                ModelState.AddModelError("", "İstenen şekilde veri giriniz");
            }
            return PartialView("CreateBrandPartial", brand);
        }

        // POST: Create a new category
        [HttpPost]
        public async Task<IActionResult> CreateCategoryPartial(Category category)
        {
            if (ModelState.IsValid)
            {
                var categoryData = new Category
                {
                    CategoryName = category.CategoryName
                };

                await _context.Categories.AddAsync(categoryData);
                await _context.SaveChangesAsync();
                return RedirectToAction("Create", "Create");
            }
            else
            {
                ModelState.AddModelError("", "İstenen şekilde veri giriniz");
            }
            return PartialView("CreateCategoryPartial", category);
        }

        // POST: Create a new model under a selected brand
        [HttpPost]
        public async Task<IActionResult> CreateModelPartial(BrandModel brandModel)
        {
            if (ModelState.IsValid)
            {
                if (brandModel.BrandId <= 0)
                {
                    ModelState.AddModelError("BrandId", "Bir marka seçilmelidir.");
                }
                if (brandModel.CategoryId <= 0)
                {
                    ModelState.AddModelError("CategoryId", "Bir kategori seçilmelidir.");
                }

                if (ModelState.IsValid)
                {
                    var brandModelData = new BrandModel
                    {
                        BrandId = brandModel.BrandId,
                        ModelName = brandModel.ModelName,
                        CategoryId = brandModel.CategoryId,
                        HorsePower = brandModel.HorsePower,
                        MaxTorque = brandModel.MaxTorque
                    };
                    await _context.BrandModels.AddAsync(brandModelData);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Create", "Create");
                }
            }

            return PartialView("CreateModelPartial", brandModel);
        }

        // POST: Delete a category
        [HttpPost]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            var activeCars = await _context.Brands
                .AnyAsync(b => b.CategoryId == id);

            if (activeCars)
            {
                TempData["DeleteCategoryError"] = "Bu markaya ait aktif ilan var, o yüzden marka silinemez.";
                return RedirectToAction("Create", "Create");
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return RedirectToAction("Create", "Create");
        }

        // GET: Edit category
        [HttpGet]
        public IActionResult EditCategory(int id)
        {
            var category = _context.Categories.Find(id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Update category
        [HttpPost]
        public IActionResult EditCategory(Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Categories.Update(category);
                _context.SaveChanges();
                return RedirectToAction("Create");
            }

            return View(category);
        }

        // POST: Delete brand
        [HttpPost]
        public async Task<IActionResult> DeleteBrand(int id)
        {
            var brand = await _context.Brands.FindAsync(id);

            if (brand == null)
            {
                return NotFound();
            }

            var activeCars = await _context.Cars
                .AnyAsync(car => car.BrandId == id);

            if (activeCars)
            {
                TempData["DeleteBrandError"] = "Bu markaya ait aktif ilan var, o yüzden marka silinemez.";
                return RedirectToAction("Create", "Create");
            }

            _context.Brands.Remove(brand);
            await _context.SaveChangesAsync();

            return RedirectToAction("Create", "Create");
        }

        // GET: Edit brand
        [HttpGet]
        public IActionResult EditBrand(int id)
        {
            var brand = _context.Brands
                .Include(b => b.Category)
                .FirstOrDefault(b => b.BrandId == id);

            if (brand == null)
            {
                return NotFound();
            }

            ViewBag.Categories = _context.Categories.ToList();
            return View(brand);
        }

        // POST: Update brand
        [HttpPost]
        public IActionResult EditBrand(Brand brand)
        {
            if (ModelState.IsValid)
            {
                _context.Brands.Update(brand);
                _context.SaveChanges();
                return RedirectToAction("Create");
            }

            return View(brand);
        }

        // POST: Delete brand model
        [HttpPost]
        public async Task<IActionResult> DeleteBrandModel(int id)
        {
            var brandModel = await _context.BrandModels.FindAsync(id);
            if (brandModel == null)
            {
                return NotFound();
            }

            var activeCars = await _context.Cars
                .AnyAsync(car => car.ModelId == id);

            if (activeCars)
            {
                TempData["DeleteBrandModelError"] = "Bu modele ait aktif ilan var, o yüzden model silinemez.";
                return RedirectToAction("Create", "Create");
            }

            _context.BrandModels.Remove(brandModel);
            await _context.SaveChangesAsync();

            return RedirectToAction("Create", "Create");
        }
    }
}
