using Identity.Data;
using Identity.Entities;
using Identity.ViewModels.Basket;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Identity.Controllers
{
    [Authorize]
    public class BasketController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly AppDbContext _context;

        public BasketController(UserManager<User>userManager,
            AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }
        [HttpGet]
        public IActionResult Index()
        {
            var authUser = _userManager.GetUserAsync(User).Result;
            if (authUser is null) return Unauthorized();
            var user = _userManager.Users.Include(u => u.Basket).FirstOrDefault(u=>u.Id==authUser.Id);
            if (user.Basket is null) return View(new BasketIndexVM());
            
            var model = new BasketIndexVM
            {
                 BasketProducts= _context.BasketProducts.Include(bp=>bp.Product).ThenInclude(p=>p.Category).Where(bp => bp.BasketId == user.Basket.Id).ToList()
                
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult AddProduct(int productId)
        {
            var user = _userManager.GetUserAsync(User).Result;
            if (user == null) return Unauthorized();
            var product=_context.Products.Find(productId);
            if (product == null) return NotFound("cannot be add product to basket");
            if (product.StockQuantity == 0) return BadRequest("product out of stock");
            var basket = _context.Baskets.FirstOrDefault(b=>b.UserId==user.Id);
            if (basket == null)
            {
                basket= new Basket
                {
                    UserId = user.Id,
                    CretaAt = DateTime.Now
                };
                _context.Baskets.Add(basket);
            }
            var basketProduct = _context.BasketProducts.FirstOrDefault(b=>b.ProductId==productId && b.Basket.UserId==user.Id);
            if(basketProduct is null)
            {
                 basketProduct = new BasketProduct
                {
                    Basket = basket,
                    ProductId = productId,
                    Quantity = 1,
                    CretaAt = DateTime.Now

                };
                _context.BasketProducts.Add(basketProduct);

            }
            else
            {
                if (basketProduct.Quantity== product.StockQuantity ) return BadRequest("added maximum product");

                basketProduct.Quantity++;
                _context.BasketProducts.Update(basketProduct);

            }

            _context.SaveChanges();
            return Ok("successfuly added");
        }

        [HttpPost]
        public IActionResult IncreaseCount(int basketProductId)
        {
            var user=_userManager.GetUserAsync(User).Result;
            if (user == null) return Unauthorized();
            var basketProduct = _context.BasketProducts.Include(bp=>bp.Basket).FirstOrDefault(bp=>bp.Id==basketProductId);
            if (basketProduct is null) return NotFound("Not found product on basket");

            if (basketProduct.Basket.UserId != user.Id) return BadRequest("cannot be products count increased  ");


            var product =_context.Products.Find(basketProduct.ProductId);
            if (product is null) return NotFound("not found product");

            if (basketProduct.Quantity == product.StockQuantity) return BadRequest("added maximum product");

            basketProduct.Quantity++;
            _context.BasketProducts.Update(basketProduct);
            _context.SaveChanges();
            return Ok(new
            {
                quantity = basketProduct.Quantity,
                productTotalPrice = basketProduct.Quantity * product.Price,
                totalPrice = _context.BasketProducts.Include(bp=>bp.Product)
                                .Where(bp => bp.BasketId == user.Basket.Id)
                                .Sum(bp=>bp.Quantity*bp.Product.Price)
            });
        }   
        
        [HttpPost]
        public IActionResult DecreaseCount(int basketProductId)
        {
            var user=_userManager.GetUserAsync(User).Result;
            if (user == null) return Unauthorized();
            var basketProduct = _context.BasketProducts.Include(bp=>bp.Basket).FirstOrDefault(bp=>bp.Id==basketProductId);
            if (basketProduct is null) return NotFound("Not found product on basket");

            if (basketProduct.Basket.UserId != user.Id) return BadRequest("cannot be products count increased  ");


            var product =_context.Products.Find(basketProduct.ProductId);
            if (product is null) return NotFound("not found product");

            if (basketProduct.Quantity == 1) return BadRequest("must be count minimum 1");            
            
                basketProduct.Quantity--;
                _context.BasketProducts.Update(basketProduct);
            
            _context.SaveChanges();
            return Ok(new
            {
                quantity = basketProduct.Quantity,
                productTotalPrice = basketProduct.Quantity * product.Price,
                totalPrice = _context.BasketProducts.Include(bp => bp.Product)
                                   .Where(bp => bp.BasketId == user.Basket.Id)
                                   .Sum(bp => bp.Quantity * bp.Product.Price)
            });

        }


        [HttpPost]
        public IActionResult DeleteProduct(int basketProductId)
        {
            var user = _userManager.GetUserAsync(User).Result;
            if (user == null) return Unauthorized();

            var basketProduct = _context.BasketProducts.Include(bp => bp.Basket).FirstOrDefault(bp => bp.Id == basketProductId);
            if (basketProduct is null) return NotFound("Not found product on basket");

            if (basketProduct.Basket.UserId != user.Id) return BadRequest("cannot be product delete  ");
            _context.BasketProducts.Remove(basketProduct);
            _context.SaveChanges();
            return Ok(new
            {
                totalPrice = _context.BasketProducts.Include(bp => bp.Product)
                                   .Where(bp => bp.BasketId == user.Basket.Id)
                                   .Sum(bp => bp.Quantity * bp.Product.Price)
            });
        }
    }
}
