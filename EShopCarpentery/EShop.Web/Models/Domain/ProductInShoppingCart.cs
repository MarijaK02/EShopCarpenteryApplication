﻿using EShop.Web.Models.Shared;

namespace EShop.Web.Models.Domain
{
    public class ProductInShoppingCart : BaseEntity
    {
        public Guid ProductId { get; set; }
        public Guid ShoppingCartId { get; set; }
        public Product? Product { get; set; }
        public ShoppingCart? ShoppingCart { get; set; }
        public int Quantity { get; set; }
    }
}
