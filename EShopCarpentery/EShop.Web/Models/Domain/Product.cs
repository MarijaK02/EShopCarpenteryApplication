using System.ComponentModel.DataAnnotations;
using EShop.Web.Models.Enums;
using EShop.Web.Models.Shared;

namespace EShop.Web.Models.Domain
{
    public class Product : BaseEntity
    {
        public string? ProductName { get; set; }
        public string? ProductDescription { get; set; }
        public string? ProductImage { get; set; }
        public MaterialType MaterialType { get; set; }
        public List<CategoryType> CategoryTypes { get; set; }
        public decimal Height { get; set; }
        public decimal Width { get; set; }
        public decimal Length { get; set; }
        [Required]
        public float Price { get; set; }
        public int Stock { get; set; }
        public virtual IEnumerable<ProductInShoppingCart>? ProductInShoppingCarts { get; set; }
        public virtual IEnumerable<ProductInOrder>? ProductsInOrder { get; set; }
    }
}
