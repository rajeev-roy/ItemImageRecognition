using FindProductByImage.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FindProductByImage.ViewModel
{
    public class PredictionViewModel
    {
        [Required]
        [Range(1,int.MaxValue,ErrorMessage ="Please enter valid integer Number")]
        public int ID { get; set; }
        public List<ProductDetails> lstProductDetails { get; set; }
    }
}
