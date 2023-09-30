using AspnetRunBasics.Models;
using AspnetRunBasics.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AspnetRunBasics.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ICatalogService _catalogService;
        private readonly IBasketService _basketServices;

        public IndexModel(ICatalogService catalogService, IBasketService basketServices)
        {
            _catalogService = catalogService ?? throw new ArgumentNullException(nameof(catalogService));
            _basketServices = basketServices ?? throw new ArgumentNullException(nameof(basketServices));
        }
         
        public IEnumerable<CatalogModel> ProductList { get; set; } = new List<CatalogModel>();


        public async Task<IActionResult> OnGetAsync()
        {
            ProductList = await _catalogService.GetCatalog();
            return Page();
        }

        public async Task<IActionResult> OnPostAddToCartAsync(string productId)
        {
            var product = await _catalogService.GetCatalog(productId);

            var userName = "swn";
            var basket = await _basketServices.GetBasket(userName);

            basket.Items.Add(new BasketItemModel
            {
                ProductId = productId,
                ProductName = product.Name,
                Price = product.Price,
                Quantity = 1,
                Color = "Black"
            });

            var basketUpdate = await _basketServices.UpdateBasket(basket);
            return RedirectToPage("cart");
        }
    }
}