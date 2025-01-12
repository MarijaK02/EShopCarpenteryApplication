using EShop.Web.Models.Enums;
using EShop.Web.Models.Shared;
using System.ComponentModel.DataAnnotations;

namespace EShop.Web.Models.Domain
{
    public class Order : BaseEntity
    {
        [Required]
        public string Address { get; set; }
        [Required]
        public string ContactPhone { get; set; }
        [Required]
        public float Price { get; set; }
        public string? AdditionalInformation { get; set; }
        public OrderStatus Status { get; set; }
        public string? OwnerId { get; set; }
        public EShopUser? Owner { get; set; }
        public ICollection<ProductInOrder>? ProductsInOrder { get; set; }
        public Guid? supplierId { get; set; }
        public virtual Supplier? Supplier { get; set; }
    }
}
