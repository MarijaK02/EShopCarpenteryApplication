using EShop.Web.Models.Domain;

namespace EShop.Web.Models.DTO
{
    public class ShoppingCartDto
    {
        public List<ProductInShoppingCart> ProductsInShoppingCart { get; set; }
        public float Total { get; set; }
    }
}
