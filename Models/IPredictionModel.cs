using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FindProductByImage.Models
{
    public interface IPredictionModel
    {
        ProductDetails SearchProduct(int id);
    }
}
