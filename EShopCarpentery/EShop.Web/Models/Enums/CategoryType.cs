using System.ComponentModel.DataAnnotations;

namespace EShop.Web.Models.Enums
{
    public enum CategoryType
    {
        [Display(Name = "Home")]
        Home,
        [Display(Name = "Outside")]
        Outside,
        [Display(Name = "Furniture")]
        Furniture,
        [Display(Name = "Decor")]
        Decor,
        [Display(Name = "Dining Table")]
        DiningTable,

        [Display(Name = "Bookshelf")]
        Bookshelf,

        [Display(Name = "Coffee Table")]
        CoffeeTable,

        [Display(Name = "Chair")]
        Chair,

        [Display(Name = "Cutting Board")]
        CuttingBoard,

        [Display(Name = "Planter")]
        Planter,

        [Display(Name = "Wall Shelf")]
        WallShelf,

        [Display(Name = "Serving Tray")]
        ServingTray,

        [Display(Name = "Cabinet")]
        Cabinet,

        [Display(Name = "Bed Frame")]
        BedFrame
    }
}
