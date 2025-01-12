using System.ComponentModel.DataAnnotations;

namespace EShop.Web.Models.Enums
{
    public enum MaterialType
    {
        [Display(Name = "Solid Wood")]
        SolidWood,

        [Display(Name = "Plywood")]
        Plywood,

        [Display(Name = "MDF (Medium Density Fiberboard)")]
        MDF,

        [Display(Name = "Particle Board")]
        ParticleBoard,

        [Display(Name = "Reclaimed Wood")]
        ReclaimedWood,

        [Display(Name = "Bamboo")]
        Bamboo,

        [Display(Name = "Veneer")]
        Veneer,

        [Display(Name = "Metal")]
        Metal,

        [Display(Name = "Glass")]
        Glass,

        [Display(Name = "Resin")]
        Resin
    }
}
