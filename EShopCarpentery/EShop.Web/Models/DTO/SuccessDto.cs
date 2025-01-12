using EShop.Web.Models.Domain;

namespace EShop.Web.Models.DTO
{
    public class SuccessDto
    {
        public Order Order { get; set; }
        public bool WithPayment { get; set; }
    }
}
