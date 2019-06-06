using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Trial2.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FindProductByImage.Controllers
{
    public class CameraController : Controller
    {
        //int id;
        static int count = 0;
        //private readonly DatabaseContext _context;
        private readonly IHostingEnvironment _environment;
        public CameraController(IHostingEnvironment hostingEnvironment)//, DatabaseContext context)
        {
            //id = (int)TempData["idData"];
            //count = 0;
            _environment = hostingEnvironment;
            System.Diagnostics.Debug.WriteLine("count= " + count);
            //_context = context;

        }

        [HttpGet]
        public IActionResult Capture()
        {
            System.Diagnostics.Debug.WriteLine("before retrun view() capture() count= " + count);
            return View();
        }


        [HttpPost]
        public IActionResult Capture(int id)
        {
            //int id2 = id1.ID;
            //int id1 = (int)TempData["idData"];
            //int id2 = id;
            var tempID=HttpContext.Session.GetString("idval");
            System.Diagnostics.Debug.WriteLine("capture(string) function first line count= " + count);
            if (count < 5)
            {
                try
                {
                    var files = HttpContext.Request.Form.Files;

                    if (files != null)
                    {
                        foreach (var file in files)
                        {
                            System.Diagnostics.Debug.WriteLine("foreach (var file in files) count= " + count);
                            if (file.Length > 0)
                            {
                                //byte[] temp=0;
                                //var reader = file.OpenReadStream();
                                //var fileContent = reader.Read(temp, 0, 100);
                                //List<byte[]> tempImageStore = new List<byte[]>();


                                // Getting Filename
                                var fileName = file.FileName;
                                //Console.WriteLine({ 0}, fileName);
                                System.Diagnostics.Debug.WriteLine("filename= " + fileName);

                                // Unique filename "Guid"
                                var myUniqueFileName = Convert.ToString(Guid.NewGuid());
                                // Getting Extension
                                var fileExtension = Path.GetExtension(fileName);
                                System.Diagnostics.Debug.WriteLine("fileExtension= " + fileExtension);

                                // Concating filename + fileExtension (unique filename)
                                var newFileName = string.Concat(myUniqueFileName, fileExtension);
                                System.Diagnostics.Debug.WriteLine("newFileName= " + newFileName);

                                //  Generating Path to store photo
                                var newFolderName = tempID;//.ToString();//DateTime.Now.ToString("dd-MM-yy hh-mm-ss");
                                string path = _environment.WebRootPath + "\\ImagesStorage\\" + newFolderName;
                                System.IO.Directory.CreateDirectory(path);
                                var filepath = Path.Combine(_environment.WebRootPath, "ImagesStorage", newFolderName) + $@"\{newFileName}";
                                //var filepath = Path.Combine(_environment.WebRootPath, "CameraPhotos")// + $@"\{newFolderName}"+$@"\{newFileName}";

                                System.Diagnostics.Debug.WriteLine("path= " + path);
                                System.Diagnostics.Debug.WriteLine("filepath= " + filepath);

                                if (!string.IsNullOrEmpty(filepath))
                                {
                                    System.Diagnostics.Debug.WriteLine(" before StoreInFolder(file, filepath) count= " + count);
                                    // Storing Image in Folder
                                    count++;
                                    StoreInFolder(file, filepath);
                                }

                                /*var imageBytes = System.IO.File.ReadAllBytes(filepath);
                                if (imageBytes != null)
                                {
                                    // Storing Image in Folder
                                    StoreInDatabase(imageBytes);
                                }*/

                            }
                        }
                        return Json(true);
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
            else
            {
                System.Diagnostics.Debug.WriteLine("count of pictures exceeded 5");
                return Json(false);
            }

        }


        private void StoreInList(IFormFile file, string fileName)
        {

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

        private void Done()
        {
            RedirectToPage("");
        }

        /*/// <summary>
        /// Saving captured image into database.
        /// </summary>
        /// <param name="imageBytes"></param>
        private void StoreInDatabase(byte[] imageBytes)
        {
            try
            {
                if (imageBytes != null)
                {
                    string base64String = Convert.ToBase64String(imageBytes, 0, imageBytes.Length);
                    string imageUrl = string.Concat("data:image/jpg;base64,", base64String);

                    ImageStore imageStore = new ImageStore()
                    {
                        CreateDate = DateTime.Now,
                        ImageBase64String = imageUrl,
                        ImageId = 0
                    };

                    _context.ImageStore.Add(imageStore);
                    _context.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }*/
    }
}