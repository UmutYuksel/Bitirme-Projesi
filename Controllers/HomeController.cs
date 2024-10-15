using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BitirmeProjesi.Models;
using BitirmeProjesi.Data;
using SQLitePCL;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ModelBinding;

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
    var categories = _context.Categories.ToList(); // Categories veritabanından alınıyor
    return View(categories); // List<Category> tipinde model dönülüyor
}


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
