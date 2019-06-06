using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FindProductByImage.Models
{
    public class ProductDetails
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "Name of the item is required.")]
        [RegularExpression(@"^[A-Za-z0-9\s ]+$", ErrorMessage = "Only Alphanumeric characters with spaces are allowed.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Price of the item is required.")]
        [RegularExpression(@"^([0-9]\d?\d?|0)(\.\d{1,2})?$", ErrorMessage = "range of the price is 0 to 999.99 and allows upto two decimal values")]
        [Range(0.00, 999.99, ErrorMessage =
             "Price should be between 0.00 and 999.99.")]
        public decimal Price { get; set; }
        [Required(ErrorMessage = "Description field is required.")]

        [RegularExpression(@"^[a-zA-Z\d\s]*$", ErrorMessage = "Only Alphanumeric characters with spaces are allowed.")]

        public string Description { get; set; }
        public DateTime Datetime { get; set; }
    }
}
