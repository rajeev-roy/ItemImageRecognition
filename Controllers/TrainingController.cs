using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training.Models;
using System.IO;
using System.Threading;
using FindProductByImage.ef;
using System.Net.Http.Headers;

using System.Net.Http;
using System.Web;

namespace FindProductByImage.Controllers
{
    public class TrainingController : Controller
    {
        private readonly DataContext _context;
        public string BaseImagesFolderPath = @"./wwwroot/ImagesStorage/";
        //public string BaseImagesFolderPath = @"C:\Users\Pratik saxsena\Desktop\NCR_Internship\frontend\FindProductByImage5\wwwroot\ImagesStorage\" ;
        private const string SouthCentralUsEndpoint = "https://centralindia.api.cognitive.microsoft.com";
        public IActionResult Index()
        {
            return View();
        }
        public TrainingController(DataContext context)
        {
            _context = context;
        }
        public IActionResult Train()
        {
            string trainingKey = "86669c56dcd746609e9d41de499ed397";
            string predictionKey = "2800619721c248a291c61647aa3d3129";

            // Create the Api, passing in the training key
            CustomVisionTrainingClient trainingApi = new CustomVisionTrainingClient()
            {
                ApiKey = trainingKey,
                Endpoint = SouthCentralUsEndpoint
            };

            // Find the object detection domain
            var domains = trainingApi.GetDomains();
            var objDetectionDomain = domains.FirstOrDefault(d => d.Type == "Classification");

            // Create a new project
            var project = trainingApi.CreateProject("FindProductByImage", null, objDetectionDomain.Id);
            //Make new tags in the new project
            List<int> AvailableID = _context.ProductDetails.Select(u => u.ID).ToList();
            
            List<Tag> TagList = new List<Tag> { };
            foreach (var tag_name in AvailableID)
            {
                TagList.Add(trainingApi.CreateTag(project.Id, tag_name.ToString()));
            }
            double[] NormalizedRegion = new double[] { 0.1, 0.1, 0.1, 0.1 };
            Dictionary<string, double[]> fileToRegionMap = new Dictionary<string, double[]>();
            foreach (var sub_folder in AvailableID)
            {
                string TargetFolder = BaseImagesFolderPath + sub_folder.ToString();
                if (Directory.Exists(TargetFolder))
                {
                    // This path is a directory
                    string[] fileEntries = Directory.GetFiles(TargetFolder);
                    foreach (var filename in fileEntries)
                    {
                        //string withoutext = Path.GetFileNameWithoutExtension(filename);
                        fileToRegionMap.Add(filename, NormalizedRegion);
                    }
                }
                else
                {
                    continue;
                }

            }

            foreach (var sub_folder in AvailableID)
            {
                string TargetFolder = BaseImagesFolderPath + sub_folder.ToString();
                var imageFileEntries = new List<ImageFileCreateEntry>();
                if (Directory.Exists(TargetFolder))
                {
                    // This path is a directory
                    string[] fileEntries = Directory.GetFiles(TargetFolder);
                    foreach (var fileName in fileEntries)
                    {
                        var region = fileToRegionMap[fileName];
                        imageFileEntries.Add(new ImageFileCreateEntry(fileName, System.IO.File.ReadAllBytes(fileName), null, new List<Region>(new Region[] { new Region(TagList.Find(x => x.Name == sub_folder.ToString()).Id, region[0], region[1], region[2], region[3]) })));
                    }
                    trainingApi.CreateImagesFromFiles(project.Id, new ImageFileCreateBatch(imageFileEntries));
                }
                else
                {
                    continue;
                }
                
            }

            Iteration iteration = trainingApi.TrainProject(project.Id);
            // The returned iteration will be in progress, and can be queried periodically to see when it has completed
            while (iteration.Status == "Training")
            {
                Thread.Sleep(1000);

                // Re-query the iteration to get its updated status
                iteration = trainingApi.GetIteration(project.Id, iteration.Id);
            }

            // The iteration is now trained. Publish it to the prediction end point.
            var publishedModelName = "FindProductByImageModel";
            var predictionResourceId = "/subscriptions/cdb468a6-b070-4ee1-961a-72615ccadae3/resourceGroups/demo_resource/providers/Microsoft.CognitiveServices/accounts/demo_resource_prediction";
            trainingApi.PublishIteration(project.Id, iteration.Id, publishedModelName, predictionResourceId);
            //Recursively deleting local image files used for training
            foreach (var sub_folder in AvailableID)
            {
                string TargetFolder = BaseImagesFolderPath + sub_folder.ToString();
                if (Directory.Exists(TargetFolder))
                {
                    Directory.Delete(TargetFolder,true);
                }
                else
                {
                    continue;
                }

            }
            return View();
        }

       

        
    }
}