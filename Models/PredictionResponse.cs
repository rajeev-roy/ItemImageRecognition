using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FindProductByImage.Models
{
    public class PredictionResponse
    {
        public string Id { get; set; }
        public string Project { get; set; }
        public string Iteration { get; set; }
        public string Created { get; set; }
        public IList<Dictionary<string, string>> Predictions { get; set; }
    }
}
