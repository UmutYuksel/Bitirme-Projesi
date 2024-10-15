using BitirmeProjesi.Data;
using BitirmeProjesi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        // GET: Create
        [HttpGet]
        public IActionResult Create(int? categoryId)
        {
            // Tüm kategorileri al
            ViewBag.Categories = new SelectList(_context.Categories, "CategoryId", "CategoryName");

            // Eğer kategori ID'si varsa, markaları filtrele
            if (categoryId.HasValue && categoryId > 0)
            {
                ViewBag.Brands = new SelectList(_context.Brands.Where(b => b.CategoryId == categoryId), "BrandId", "BrandName");
            }
            else
            {
                // Eğer kategori seçilmemişse, boş bir liste oluştur
                ViewBag.Brands = new SelectList(Enumerable.Empty<Brand>(), "BrandId", "BrandName");
            }

            return View();
        }

        [HttpGet]
        public IActionResult GetBrandsByCategory(int categoryId)
        {
            var brands = _context.Brands
                .Where(b => b.CategoryId == categoryId)
                .Select(b => new { b.BrandId, b.BrandName }) // Gerekli alanları seç
                .ToList();

            return Json(brands); // JSON formatında geri döndür
        }

        [HttpPost]
        public async Task<IActionResult> CreateBrandPartial(Brand brand, int categoryId)
        {
            // Kategori bilgilerini al
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
                    CategoryId = categoryId // Seçilen kategori ID'sini burada kullanıyoruz
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

        [HttpPost]
        public async Task<IActionResult> CreateModelPartial(BrandModel brandModel)
        {
            if (ModelState.IsValid)
            {
                var brandModelData = new BrandModel
                {
                    BrandId = brandModel.BrandId,
                    ModelName = brandModel.ModelName,
                    CategoryId = brandModel.CategoryId
                };
                await _context.BrandModels.AddAsync(brandModelData);
                await _context.SaveChangesAsync();
                return RedirectToAction("Create", "Create");
            }
            return PartialView("CreateModelPartial", brandModel);
        }
    }
}
