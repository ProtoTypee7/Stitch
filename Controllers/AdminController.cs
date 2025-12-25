using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using P2WebMVC.Data;
using P2WebMVC.Interfaces;
using P2WebMVC.Models.DomainModels;
using P2WebMVC.Types;

namespace P2WebMVC.Controllers
{
    public class AdminController : Controller
    {

        private readonly SqlDbContext dbContext;    // encapsulated feilds
        private readonly ITokenService tokenService;

        private readonly ICloudinaryService cloudinaryService;

        public AdminController(ITokenService tokenService, SqlDbContext dbContext, ICloudinaryService cloudinaryService)
        {
            this.tokenService = tokenService;
            this.dbContext = dbContext;
            this.cloudinaryService = cloudinaryService;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }


        [Authorize]
        [HttpGet]
        public async Task<ActionResult> Dashboard()
        {

            try
            {
                Guid? userId = HttpContext.Items["UserId"] as Guid?;

                var user = await dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);

                if (user?.Role == Role.User)
                {
                    return RedirectToAction("Dashboard", "User");
                }
                else if (user?.Role == Role.StoreKeeper)
                {
                    return RedirectToAction("Dashboard", "StoreKeeper");
                }
                else if (user?.Role == Role.Admin)
                {

                    return View();
                }
                else
                {
                    return RedirectToAction("Login", "User");
                }


            }
            catch (System.Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("Error");

            }


        }

        [Authorize]
        [HttpGet]
        [HttpGet]
        public ActionResult Createproduct()
        {
            ViewBag.CategoryList = new SelectList(Enum.GetValues(typeof(ProductCategory)));
            return View();
        }


        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Createproduct(Product product, IFormFile image)
        {
            try
            {
                if (image == null || image.Length == 0)
                {
                    ViewBag.ErrorMessage = "Kindly select the image file.";
                    ViewBag.CategoryList = new SelectList(Enum.GetValues(typeof(ProductCategory)));
                    return View();
                }

                if (!ModelState.IsValid)
                {
                    ViewBag.ErrorMessage = "All details with * are required.";
                    ViewBag.CategoryList = new SelectList(Enum.GetValues(typeof(ProductCategory)));
                    return View();
                }

                var secureUrl = await cloudinaryService.UploadImageAsync(image);
                product.ProductImage = secureUrl;


                // product.Category = ProductCategory.General;




                await dbContext.Products.AddAsync(product);
                await dbContext.SaveChangesAsync();

                TempData["SuccessMessage"] = "Product created successfully!";
                return RedirectToAction("ManageProducts");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("Error");
            }
        }


        [Authorize]
        [HttpGet]
        public async Task<ActionResult> ManageProducts()
        {
            var products = await dbContext.Products.ToListAsync();
            return View(products);
        }



        [Authorize]
        [HttpGet]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            var product = await dbContext.Products.FindAsync(id);
            if (product == null)
                return NotFound();

            dbContext.Products.Remove(product);
            await dbContext.SaveChangesAsync();

            TempData["SuccessMessage"] = "Product deleted.";
            return RedirectToAction("ManageProducts");
        }


        [Authorize]
        [HttpGet]
        public async Task<IActionResult> EditProduct(Guid id)
        {
            var product = await dbContext.Products.FindAsync(id);
            if (product == null)
                return NotFound();
            ViewBag.CategoryList = new SelectList(Enum.GetValues(typeof(ProductCategory)));
            return View(product);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> EditProduct(Product updatedProduct)
        {
            if (!ModelState.IsValid)
                return View(updatedProduct);

            var existingProduct = await dbContext.Products.FindAsync(updatedProduct.ProductId);
            if (existingProduct == null)
                return NotFound();


            existingProduct.ProductName = updatedProduct.ProductName;
            existingProduct.ProductDescription = updatedProduct.ProductDescription;
            existingProduct.ProductPrice = updatedProduct.ProductPrice;
            existingProduct.ProductStock = updatedProduct.ProductStock;
            existingProduct.Color = updatedProduct.Color;
            existingProduct.Size = updatedProduct.Size;
            existingProduct.Category = updatedProduct.Category;


            await dbContext.SaveChangesAsync();

            TempData["SuccessMessage"] = "Product updated successfully!";
            return RedirectToAction("ManageProducts");
        }


        [Authorize]
        [HttpGet]
        public async Task<IActionResult> AllOrders()
        {
            var orders = await dbContext.Orders
                .Include(o => o.Address)
                .Include(o => o.OrderProducts)
                    .ThenInclude(op => op.Product)
                .Include(o => o.Buyer)
                .OrderByDescending(o => o.DateCreated)
                .ToListAsync();

            return View(orders);
        }


        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ViewOrderDetails(Guid id)
        {
            var order = await dbContext.Orders
                .Include(o => o.OrderProducts)
                    .ThenInclude(op => op.Product)
                .Include(o => o.Buyer)
                .Include(o => o.Address)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
                return NotFound();

            return View(order);
        }



        [Authorize]
        [HttpGet]
        public async Task<IActionResult> AllUsers()
        {
            var users = await dbContext.Users.ToListAsync();
            return View(users);
        }



        [Authorize]
        [HttpPost]
        public async Task<IActionResult> DispatchOrder(Guid orderId)
        {
            var order = await dbContext.Orders.FindAsync(orderId);

            if (order == null)
            {
                TempData["ErrorMessage"] = "Order not found.";
                return RedirectToAction("AllOrders");
            }

            if (order.PaymentStatus == PaymentStatus.Succesfull && order.OrderStatus == OrderStatus.confirmed)
            {
                order.OrderStatus = OrderStatus.Shipped;
                order.ShippingDate = DateTime.UtcNow;

                await dbContext.SaveChangesAsync();
                TempData["SuccessMessage"] = "Order dispatched successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Order cannot be dispatched unless payment is successful and status is confirmed.";
            }

            return RedirectToAction("AllOrders");
        }






        [Authorize]
        [HttpPost]
        public async Task<IActionResult> PromoteToStoreKeeper(Guid id)
        {
            var user = await dbContext.Users.FindAsync(id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction("AllUsers");
            }

            user.Role = Role.StoreKeeper;
            await dbContext.SaveChangesAsync();

            TempData["SuccessMessage"] = $"{user.Username} has been promoted to StoreKeeper.";
            return RedirectToAction("AllUsers");
        }



        [Authorize]
        [HttpPost]
        public async Task<IActionResult> DemoteToUser(Guid userId)
        {
            var user = await dbContext.Users.FindAsync(userId);

            if (user == null || user.Role != Role.StoreKeeper)
            {
                TempData["ErrorMessage"] = "Invalid operation.";
                return RedirectToAction("AllUsers");
            }

            user.Role = Role.User;
            await dbContext.SaveChangesAsync();

            TempData["SuccessMessage"] = "User has been demoted to regular User.";
            return RedirectToAction("AllUsers");
        }


    }



}







