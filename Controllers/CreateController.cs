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

        // GET: Create
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

            // Tüm kategorileri al
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

            return View(viewModel);
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
                // Marka ve kategori ID kontrolü
                if (brandModel.BrandId <= 0)
                {
                    ModelState.AddModelError("BrandId", "Bir marka seçilmelidir.");
                }
                if (brandModel.CategoryId <= 0)
                {
                    ModelState.AddModelError("CategoryId", "Bir kategori seçilmelidir.");
                }

                // Hatalar yoksa veritabanına kaydetme
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

            // Hatalı durumda, tekrar aynı partial view'i döndür
            return PartialView("CreateModelPartial", brandModel);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            // Bu kategoriye ait aktif marka var mı kontrol et
            var activeCars = await _context.Brands
                .AnyAsync(category => category.CategoryId == id); // CategoryId'si bu olan markalar var mı kontrol et

            if (activeCars)
            {
                // Eğer marka kullanılıyorsa kullanıcıya uyarı göster
                TempData["DeleteCategoryError"] = "Bu markaya ait aktif ilan var, o yüzden marka silinemez.";
                return RedirectToAction("Create", "Create"); // Geri yönlendir ve hata mesajı göster
            }

            // Eğer markaya ait ilan yoksa marka silinir
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return RedirectToAction("Create", "Create"); // İşlem başarıyla tamamlandığında geri yönlendir
        }

        [HttpGet]
        public IActionResult EditCategory(int id)
        {
            var category = _context.Categories.Find(id); // Kategori verisini veri tabanından bul
            if (category == null)
            {
                return NotFound(); // Eğer kategori bulunamazsa 404 sayfası göster
            }

            return View(category); // Düzenleme sayfasına model olarak kategoriyi gönder
        }

        [HttpPost]
        public IActionResult EditCategory(Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Categories.Update(category); // Veriyi güncelle
                _context.SaveChanges(); // Değişiklikleri kaydet
                return RedirectToAction("Create"); // İşlem tamamlanınca ana sayfaya yönlendir
            }

            return View(category); // Hata olursa aynı sayfaya geri dön ve hatayı göster
        }

        [HttpPost]
        public async Task<IActionResult> DeleteBrand(int id)
        {
            // İlgili BrandId'yi veritabanından bul
            var brand = await _context.Brands.FindAsync(id);

            if (brand == null)
            {
                return NotFound(); // Eğer marka yoksa 404 hata döndür
            }

            // Bu markaya ait aktif ilan var mı kontrol et
            var activeCars = await _context.Cars
                .AnyAsync(car => car.BrandId == id); // BrandId'si bu olan ilanlar var mı kontrol et

            if (activeCars)
            {
                // Eğer marka kullanılıyorsa kullanıcıya uyarı göster
                TempData["DeleteBrandError"] = "Bu markaya ait aktif ilan var, o yüzden marka silinemez.";
                return RedirectToAction("Create", "Create"); // Geri yönlendir ve hata mesajı göster
            }

            // Eğer markaya ait ilan yoksa marka silinir
            _context.Brands.Remove(brand);
            await _context.SaveChangesAsync();

            return RedirectToAction("Create", "Create"); // İşlem başarıyla tamamlandığında geri yönlendir
        }

        [HttpGet]
        public IActionResult EditBrand(int id)
        {
            var brand = _context.Brands
                .Include(b => b.Category) // Category'yi dahil et
                .FirstOrDefault(b => b.BrandId == id); // Markayı bul

            if (brand == null)
            {
                return NotFound(); // Eğer marka bulunamazsa 404 döndür
            }

            ViewBag.Categories = _context.Categories.ToList(); // Tüm kategorileri al
            return View(brand); // Model olarak markayı gönder
        }

        [HttpPost]
        public IActionResult EditBrand(Brand brand)
        {
            if (ModelState.IsValid)
            {
                _context.Brands.Update(brand); // Veriyi güncelle
                _context.SaveChanges(); // Değişiklikleri kaydet
                return RedirectToAction("Create"); // İşlem tamamlanınca ana sayfaya yönlendir
            }

            return View(brand); // Hata olursa aynı sayfaya geri dön ve hatayı göster
        }

        [HttpPost]
        public async Task<IActionResult> DeleteBrandModel(int id)
        {
            var brandModel = await _context.BrandModels.FindAsync(id);
            if (brandModel == null)
            {
                return NotFound();
            }

            // Bu Model ait aktif ilan var mı kontrol et
            var activeCars = await _context.Cars
                .AnyAsync(brandModel => brandModel.ModelId == id); // CategoryId'si bu olan markalar var mı kontrol et

            if (activeCars)
            {
                // Eğer marka kullanılıyorsa kullanıcıya uyarı göster
                TempData["DeleteBrandModelError"] = "Bu modele ait aktif ilan var, o yüzden model silinemez.";
                return RedirectToAction("Create", "Create"); // Geri yönlendir ve hata mesajı göster
            }

            // Eğer markaya ait ilan yoksa marka silinir
            _context.BrandModels.Remove(brandModel);
            await _context.SaveChangesAsync();

            return RedirectToAction("Create", "Create"); // İşlem başarıyla tamamlandığında geri yönlendir
        }
    }
}
