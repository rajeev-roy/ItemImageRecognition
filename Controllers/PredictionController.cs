using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
//using DemoWebCam.EntityStore;
using FindProductByImage.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace FindProductByImage.Controllers
{
    public class PredictionController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        static public string tag_name = "Object not found";
        //private readonly DatabaseContext _context;
        private readonly IHostingEnvironment _environment;
        //string connectionString = @"Data Source=pratik\pratik_ncr;Initial Catalog=ProductScanDB;Integrated Security=True";
        public PredictionController(IHostingEnvironment hostingEnvironment)//, DatabaseContext context)
        {
            _environment = hostingEnvironment;
            //_context = context;

        }

        [HttpGet]
        public IActionResult Predict()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Predict(string name)
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
                            MakePredictionRequest(filepath).Wait();
                            //Console.WriteLine("\n\nHit ENTER to exit...");
                            //Console.ReadLine();
                            //var imageBytes = System.IO.File.ReadAllBytes(filepath);
                            //if (imageBytes != null)
                            //{
                            //    // Storing Image in Folder
                            //    StoreInDatabase(imageBytes);
                            //}

                        }
                    }
                    ViewBag.Message = tag_name;
                    return Json(tag_name);
                }
                else
                {
                    return Json(false);
                }
            }
            catch (Exception)
            {
                throw;
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
        public static async Task MakePredictionRequest(string imageFilePath)
        {
            var client = new HttpClient();

            // Request headers - replace this example key with your valid Prediction-Key.
            client.DefaultRequestHeaders.Add("Prediction-Key", "2800619721c248a291c61647aa3d3129");

            // Prediction URL - replace this example URL with your valid Prediction URL.
            string url = "https://centralindia.api.cognitive.microsoft.com/customvision/v3.0/Prediction/3fb9d879-1634-4a50-a35b-77003651195c/classify/iterations/Iteration2/image";

            HttpResponseMessage response;

            // Request body. Try this sample with a locally stored image.
            byte[] byteData = GetImageAsByteArray(imageFilePath);

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response = await client.PostAsync(url, content);
                string prediction_response = response.Content.ReadAsStringAsync().Result;
                PredictionResponse jsonresponse = JsonConvert.DeserializeObject<PredictionResponse>(prediction_response);
                foreach (var tags in jsonresponse.Predictions)
                {
                    double probability = Convert.ToDouble(tags["probability"]);
                    if (probability > 0.8)
                    {
                        tag_name = tags["tagName"];

                        break;
                    }

                }
                //prediction_response.
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