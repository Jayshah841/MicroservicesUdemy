using AspnetRunBasics.Models;
using AspnetRunBasics.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AspnetRunBasics
{
    public class CartModel : PageModel
    {
        private readonly IBasketService _basketServices;

        public CartModel(IBasketService basketServices)
        {
            _basketServices = basketServices ?? throw new ArgumentNullException(nameof(basketServices));
        }

        public BasketModel Cart { get; set; } = new BasketModel();

        public async Task<IActionResult> OnGetAsync()
        {
            var userName = "swn";
            Cart = await _basketServices.GetBasket(userName);

            return Page();
        }

        public async Task<IActionResult> OnPostRemoveToCartAsync(string productId)
        {
            var userName = "swn";
            var basket = await _basketServices.GetBasket(userName);

            var item = basket.Items.First(x => x.ProductId == productId);
            basket.Items.Remove(item);

            var basketUpdated = await _basketServices.UpdateBasket(basket);

            return RedirectToPage();
        }
    }
}