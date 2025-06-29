using E_CommerceDataAccess.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_CommerceDataAccess.DTO
{
    public class CreateCartItemDTO
    {
        public int ProductID { get; set; }
        public int Quantity { get; set; }
    }

    public class GetCartItemDTO
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string Image { get; set; }
        public decimal TotalPriceForProduct => GetTotalPriceOfAllProducts();
        private decimal GetTotalPriceOfAllProducts()
        {
            return Quantity * Price;
        }
    }
}
