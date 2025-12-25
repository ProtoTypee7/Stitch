using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using P2WebMVC.Data;
using P2WebMVC.Models;
using P2WebMVC.Models.ViewModels;

namespace P2WebMVC.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly SqlDbContext _context;
    [Authorize]
    public HomeController(ILogger<HomeController> logger, SqlDbContext context)
    {
        _logger = logger;
        _context = context;
    }
    [Authorize]
    public IActionResult Index()
    {
        var model = new ProductView
        {
            Products = _context.Products.ToList()
        };

        return View(model);
    }


    public IActionResult Privacy()
    {
        return View();
    }


    public IActionResult About()
    {
        return View();
    }



    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
