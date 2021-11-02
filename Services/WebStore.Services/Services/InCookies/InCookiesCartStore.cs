using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using WebStore.Domain.Entities;
using WebStore.Interfaces.Services;

namespace WebStore.Services.Services.InCookies
{
    public class InCookiesCartStore : ICartStore
    {
        private readonly IHttpContextAccessor _HttpContextAccessor;
        private readonly string _CartName;

        public InCookiesCartStore(IHttpContextAccessor HttpContextAccessor)
        {
            _HttpContextAccessor = HttpContextAccessor;

            var user = HttpContextAccessor.HttpContext!.User;
            var userName = user.Identity!.IsAuthenticated ? $"-{user.Identity.Name}" : null;

            _CartName = $"GB.WebStore.Cart{userName}";
        }

        public Cart Cart
        {
            get
            {
                var context = _HttpContextAccessor.HttpContext;
                var cookies = context!.Response.Cookies;

                var cartCookies = context.Request.Cookies[_CartName];
                if (cartCookies is null)
                {
                    var cart = new Cart();
                    cookies.Append(_CartName, JsonConvert.SerializeObject(cart));
                    return cart;
                }

                ReplaceCart(cookies, cartCookies);
                return JsonConvert.DeserializeObject<Cart>(cartCookies);
            }
            set => ReplaceCart(_HttpContextAccessor.HttpContext!.Response.Cookies, JsonConvert.SerializeObject(value));
        }

        private void ReplaceCart(IResponseCookies cookies, string cart)
        {
            cookies.Delete(_CartName);
            cookies.Append(_CartName, cart);
        }
        
    }
}