using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using FindProductByImage.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using FindProductByImage.ef;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training.Models;
using Microsoft.Extensions.Configuration;
using FindProductByImage.ViewModel;

namespace FindProductByImage.Controllers
{
    public class PredictionController : Controller
    {
        public IConfiguration Configuration { get; }
        public DataContext _context;
        public static List<ProductDetails> lstProduct=new List<ProductDetails>();
        public IActionResult Index()
        {
            return View();
        }
        
        static public string tag_name = "Product not found";
        private readonly IHostingEnvironment _environment;
        
        public PredictionController(IHostingEnvironment hostingEnvironment, DataContext context,IConfiguration configuration)
        {
            _environment = hostingEnvironment;
            _context = context;
            Configuration = configuration;
            
        }

        [HttpGet]
        public IActionResult Predict()
        {
            return View();
        }

        //[HttpGet]
        public async Task<IActionResult> SearchItem(int? id)
        {
            if (id == null)
            {
                return NotFound();
               
            }

            var productDetails = await _context.ProductDetails.FindAsync(id);
            if (productDetails == null)
            {
                return NotFound();
            }
            //var productDetails = await _context.ProductDetails
            //    .FirstOrDefaultAsync(m => m.ID == Int32.Parse(id));
            //if (productDetails == null)
            //{
            //    return NotFound();
            //}
            lstProduct.Add(productDetails);
            PredictionViewModel predictionViewModel = new PredictionViewModel();
            predictionViewModel.lstProductDetails = lstProduct;

            return View("Predict", predictionViewModel);
           
        }


        [HttpPost]
        public string Predict(string name)
        {
            try
            {
                var files = HttpContext.Request.Form.Files;
                if (files != null)
                {
                    foreach (var file in files)
                    {
                        if (file.Length > 0)
                        {
                            // Getting Filename
                            var fileName = file.FileName;
                            // Unique filename "Guid"
                            var myUniqueFileName = Convert.ToString(Guid.NewGuid());
                            // Getting Extension
                            var fileExtension = Path.GetExtension(fileName);
                            // Concating filename + fileExtension (unique filename)
                            var newFileName = string.Concat(myUniqueFileName, fileExtension);
                            //  Generating Path to store photo 
                            var filepath = Path.Combine(_environment.WebRootPath, "CameraPhotos") + $@"\{newFileName}";

                            if (!string.IsNullOrEmpty(filepath))
                            {
                                // Storing Image in Folder
                                StoreInFolder(file, filepath);
                            }
                            MakePredictionRequest(filepath,Configuration)/*.Wait()*/;
                            
                        }
                    }
                    //ViewBag.Message = tag_name;
                    //SearchItem(tag_name);
                   
                    return tag_name;
                }
                else
                {
                    return tag_name;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// Saving captured image into Folder.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="fileName"></param>
        private void StoreInFolder(IFormFile file, string fileName)
        {
            using (FileStream fs = System.IO.File.Create(fileName))
            {
                file.CopyTo(fs);
                fs.Flush();
            }
        }
        public void MakePredictionRequest(string imageFilePath,IConfiguration configuration)
        {
           
            CustomVisionPredictionClient endpoint = new CustomVisionPredictionClient()
            {
                ApiKey = configuration.GetSection("AzureKeys:PredictionKey").Value,
                Endpoint = "https://centralindia.api.cognitive.microsoft.com"
            };
            

            using (var stream = System.IO.File.OpenRead(imageFilePath))
            {
                var result = endpoint.ClassifyImage(Guid.Parse(System.IO.File.ReadAllText(@"./ProjectId.txt")), System.IO.File.ReadAllText(@"./PublishedModelName.txt"), System.IO.File.OpenRead(imageFilePath));
               
                
                //Loop over each prediction and write out the results
                foreach (var c in result.Predictions)
                {
                    if (c.Probability > 0.80)
                    {
                        tag_name = c.TagName;
                    }
                    //Console.WriteLine($"\t{c.TagName}: {c.Probability:P1} [ {c.BoundingBox.Left}, {c.BoundingBox.Top}, {c.BoundingBox.Width}, {c.BoundingBox.Height} ]");
                }


            }
        }

        private static byte[] GetImageAsByteArray(string imageFilePath)
        {
            FileStream fileStream = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read);
            BinaryReader binaryReader = new BinaryReader(fileStream);
            return binaryReader.ReadBytes((int)fileStream.Length);
        }
        
    }
}