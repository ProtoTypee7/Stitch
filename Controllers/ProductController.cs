using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P2WebMVC.Data;
using P2WebMVC.Interfaces;
using P2WebMVC.Models.DomainModels;
using P2WebMVC.Models.JunctionModels;
using P2WebMVC.Models.ViewModels;
using P2WebMVC.Types;

namespace P2WebMVC.Controllers
{
  public class ProductController : Controller
  {


    private readonly SqlDbContext dbContext;

    private readonly ITokenService tokenService;


    public ProductController(SqlDbContext dbContext, ITokenService tokenService)
    {
      this.dbContext = dbContext;
      this.tokenService = tokenService;
    }

    // GET: ProductController
    [Authorize]
    [HttpGet]
    public async Task<ActionResult> Index(ProductCategory category)
    {
      try
      {
        Guid? userId = HttpContext.Items["UserId"] as Guid?;

        var products = await dbContext.Products
            .Where(p => p.IsActive && (category == ProductCategory.General || p.Category == category))
            .ToListAsync();

        var viewModel = new ProductView
        {
          Products = products,
          WishlistProductIds = userId == null
                ? new List<Guid>()
                : await dbContext.WishlistProducts
                    .Where(wp => wp.Wishlist.UserId == userId)
                    .Select(wp => wp.ProductId)
                    .ToListAsync()
        };

        ViewBag.Category = category;
        ViewBag.UserId = userId;

        return View(viewModel);
      }
      catch (System.Exception ex)
      {
        ViewBag.ErrorMessage = ex.Message;
        return View("Error");
      }
    }


    [HttpGet]
    public async Task<IActionResult> Details(Guid ProductId)
    {
      try
      {
        var product = await dbContext.Products.FindAsync(ProductId);

        var viewModel = new ProductView
        {
          Product = product

        };

        return View(viewModel);


      }
      catch (System.Exception ex)
      {

        ViewBag.ErrorMessage = ex.Message;
        return View("Error");

      }
    }


    [Authorize]
    [HttpPost]
    public async Task<IActionResult> AddToCart(Guid ProductId, string? Color = "default", int Quantity = 1, string? Size = "default")
    {
      try
      {
        if (HttpContext.Items["UserId"] is not Guid userId)
        {
          TempData["ErrorMessage"] = "User not Found!";
          return RedirectToAction("Login", "User");
        }

        var product = await dbContext.Products.FindAsync(ProductId);
        if (product == null)
        {
          TempData["ErrorMessage"] = "Product Not Found!";
          return RedirectToAction("Cart", "User");
        }

        var cart = await dbContext.Carts.FirstOrDefaultAsync(c => c.UserId == userId);
        if (cart == null)
        {
          cart = new Cart
          {
            UserId = userId,
            CartTotal = 0,
            CartProducts = []
          };
          await dbContext.Carts.AddAsync(cart);
        }

        var existingItem = await dbContext.CartProducts
            .FirstOrDefaultAsync(cp => cp.CartId == cart.CartId && cp.ProductId == ProductId);

        if (existingItem == null)
        {
          var cartProduct = new CartProduct
          {
            CartId = cart.CartId,
            ProductId = ProductId,
            Quantity = Quantity,
            Color = Color,
            Size = Size
          };
          await dbContext.CartProducts.AddAsync(cartProduct);
        }
        else
        {
          existingItem.Quantity += Quantity;
        }

        cart.CartTotal += Quantity * product.ProductPrice;
        await dbContext.SaveChangesAsync();

        return RedirectToAction("Cart", "User");
      }
      catch (Exception ex)
      {
        ViewBag.ErrorMessage = ex.Message;
        return View("Error");
      }
    }



    [Authorize]
    [HttpPost]
    public async Task<IActionResult> RemoveFromCart(Guid ProductId)
    {
      try
      {
        if (HttpContext.Items["UserId"] is not Guid userId)
        {
          TempData["ErrorMessage"] = "User not found.";
          return RedirectToAction("Login", "User");
        }

        var cart = await dbContext.Carts.FirstOrDefaultAsync(c => c.UserId == userId);
        if (cart == null)
        {
          TempData["ErrorMessage"] = "Cart not found.";
          return RedirectToAction("Cart", "User");
        }

        var cartItem = await dbContext.CartProducts
            .FirstOrDefaultAsync(cp => cp.CartId == cart.CartId && cp.ProductId == ProductId);

        if (cartItem != null)
        {
          var product = await dbContext.Products.FindAsync(ProductId);
          if (product != null)
          {
            cart.CartTotal -= product.ProductPrice * cartItem.Quantity;
          }

          dbContext.CartProducts.Remove(cartItem);
          await dbContext.SaveChangesAsync();
        }

        return RedirectToAction("Cart", "User");
      }
      catch (Exception ex)
      {
        ViewBag.ErrorMessage = ex.Message;
        return View("Error");
      }
    }

  }




}
