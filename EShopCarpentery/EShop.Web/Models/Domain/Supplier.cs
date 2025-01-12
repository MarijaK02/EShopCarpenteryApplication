using EShop.Web.Models.Shared;
using System.ComponentModel.DataAnnotations;

namespace EShop.Web.Models.Domain
{
    public class Supplier : BaseEntity
    {
        public string Name { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public string Address { get; set; }
        [Phone]
        public int ContactPhone { get; set; }
        public float PricePerKm { get; set; }
        public float FreeDeliveryKm { get; set; }
        public List<Order>? Orders { get; set; }

    }
}
