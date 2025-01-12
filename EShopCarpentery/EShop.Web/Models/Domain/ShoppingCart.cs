using EShop.Web.Models.Shared;
using Microsoft.AspNetCore.Identity;

namespace EShop.Web.Models.Domain
{
    public class ShoppingCart : BaseEntity
    {
        public string? OwnerId { get; set; }
        public EShopUser? Owner { get; set; }
        public virtual ICollection<ProductInShoppingCart>? ProductInShoppingCarts { get; set; }
    }
}
