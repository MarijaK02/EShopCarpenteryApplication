using System.ComponentModel.DataAnnotations;

namespace EShop.Web.Models.Shared
{
    public class BaseEntity
    {
        [Required]
        public Guid Id { get; set; }
    }
}
