using EShop.Web.Models.Enums;
using EShop.Web.Models.Shared;

namespace EShop.Web.Models.Domain
{
    public class Payment : BaseEntity
    {
        public int CCVNumber { get; set; }
        public DateTime ExpirationDate { get; set; }
        public long CardNumber { get; set; }
        public DateTime DateCreated { get; set; }
        public float Amount { get; set; }
        public PaymentStatus Status { get; set; }
    }
}
