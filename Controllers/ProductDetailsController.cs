using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FindProductByImage.Models;
using FindProductByImage.ef;
using ReflectionIT.Mvc.Paging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Diagnostics;

namespace FindProductByImage.Controllers
{
    public class ProductDetailsController : Controller
    {
        static int fileCount;
        static int count = 1;
        private readonly DataContext _context;
        private readonly IHostingEnvironment _environment;

        public ProductDetailsController(IHostingEnvironment hostingEnvironment,DataContext context)
        {
            _context = context;
            _environment = hostingEnvironment;
        }

        // GET: ProductDetails
        public async Task<IActionResult> Index(int page = 1)
        {
            var query = _context.ProductDetails.AsNoTracking().OrderBy(s => s.ID);
            var model = await PagingList.CreateAsync(query, 5, page);
            return View(model);
        }

        // GET: ProductDetails/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productDetails = await _context.ProductDetails
                .FirstOrDefaultAsync(m => m.ID == id);
            if (productDetails == null)
            {
                return NotFound();
            }

            return View(productDetails);
        }

        // GET: ProductDetails/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ProductDetails/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,Price,Description,Datetime")] ProductDetails productDetails)
        {
            if (ModelState.IsValid)
            {
                productDetails.Datetime = DateTime.Now;
                _context.Add(productDetails);
                await _context.SaveChangesAsync();
                
                SaveImages(productDetails.ID);

                return RedirectToAction("Index", "ProductDetails");
            }

            else
            {
                return View(productDetails);
            }
     
         }

        private void SaveImages(int id)
        {
            var tempFolderPath = Path.Combine(_environment.WebRootPath, "temporary");
            if (!Directory.Exists(tempFolderPath))
            {
                Directory.CreateDirectory(tempFolderPath);
            }
            if (Directory.Exists(tempFolderPath))
            {
                fileCount = Directory.EnumerateFiles(tempFolderPath, "*.jpg", SearchOption.AllDirectories).Count();

                if (fileCount == 15)
                {
                    var newFolderName = id.ToString();
                    string path = _environment.WebRootPath + "\\ImagesStorage\\" + newFolderName;
                    System.IO.Directory.CreateDirectory(path);
                    var storeImagePath = Path.Combine(_environment.WebRootPath, "ImagesStorage", newFolderName);

                    if (System.IO.Directory.Exists(tempFolderPath))
                    {
                        string[] files = System.IO.Directory.GetFiles(tempFolderPath);
                        string fileName;
                        string destFile;

                        // Copy the files and overwrite destination files if they already exist.
                        foreach (string s in files)
                        {
                            fileName = System.IO.Path.GetFileName(s);
                            destFile = System.IO.Path.Combine(storeImagePath, fileName);
                            System.IO.File.Copy(s, destFile, true);
                            System.IO.File.Delete(s);
                        }

                    }

                    else
                    {
                        ViewBag.CameraError="Source path does not exist!";
                    }
                }
                else
                {
                    ViewBag.CameraError="less than 15";
                }
            }
            else
            {
                ViewBag.CameraError = "Product Image doesn't exists !";
                Directory.CreateDirectory(tempFolderPath);

            }
            

        }

        //private void SaveUploads(int id)
        //{
        //    var tempFolderPath = Path.Combine(_environment.WebRootPath, "upload");
        //    fileCount = Directory.EnumerateFiles(tempFolderPath, "*.jpg", SearchOption.AllDirectories).Count() +
        //        Directory.EnumerateFiles(tempFolderPath, "*.png", SearchOption.AllDirectories).Count();

        //    if (fileCount == 15)
        //    {
        //        var newFolderName = id.ToString();
        //        string path = _environment.WebRootPath + "\\ImagesStorage\\" + newFolderName;
        //        System.IO.Directory.CreateDirectory(path);
        //        var storeImagePath = Path.Combine(_environment.WebRootPath, "ImagesStorage", newFolderName);

        //        if (System.IO.Directory.Exists(tempFolderPath))
        //        {
        //            string[] files = System.IO.Directory.GetFiles(tempFolderPath);
        //            string fileName;
        //            string destFile;

        //            // Copy the files and overwrite destination files if they already exist.
        //            foreach (string s in files)
        //            {
        //                // Use static Path methods to extract only the file name from the path.
        //                fileName = System.IO.Path.GetFileName(s);
        //                destFile = System.IO.Path.Combine(storeImagePath, fileName);
        //                System.IO.File.Copy(s, destFile, true);
        //                System.IO.File.Delete(s);
        //            }

        //        }

        //        else
        //        {
        //            ViewBag.CameraError = "Source path does not exist!";
        //        }
        //    }
        //    else
        //    {
        //        ViewBag.CameraError = "less than 15";
        //    }
        //}

        // GET: ProductDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
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
            return View(productDetails);
        }

        // POST: ProductDetails/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Price,Description,Datetime")] ProductDetails productDetails)
        {
            if (id != productDetails.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    productDetails.Datetime = DateTime.Now;
                    _context.Update(productDetails);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductDetailsExists(productDetails.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(productDetails);
        }

        // GET: ProductDetails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productDetails = await _context.ProductDetails
                .FirstOrDefaultAsync(m => m.ID == id);
            if (productDetails == null)
            {
                return NotFound();
            }

            return View(productDetails);
        }

        // POST: ProductDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var productDetails = await _context.ProductDetails.FindAsync(id);
            _context.ProductDetails.Remove(productDetails);
            await _context.SaveChangesAsync();
            string path = _environment.WebRootPath + "\\ImagesStorage\\" + id;
            if (Directory.Exists(path))
                Directory.Delete(path, true);
            return RedirectToAction(nameof(Index));
        }

        private bool ProductDetailsExists(int id)
        {
            return _context.ProductDetails.Any(e => e.ID == id);
        }

        //public IActionResult UploadPictures(IList<IFormFile> pics)
        //{
        //    try
        //    {
        //        string path = _environment.WebRootPath + "\\upload\\";
        //        if (!Directory.Exists(path))
        //        {
        //            Directory.CreateDirectory(path);
        //        }

        //        foreach (var pic in pics)
        //        {
        //            if (pic != null)
        //            {
        //                var fileName = Path.Combine(path, Path.GetFileName(pic.FileName));
        //                pic.CopyTo(new FileStream(fileName, FileMode.Create));
        //                ViewData["noOfFiles"] = pics.Count;

        //            }
        //        }
        //        string dummy = pics.Count + "files successfully uploaded";
        //        ViewData["noOfFiles"] = dummy;
        //    }
        //    catch(Exception e)
        //    {
        //        throw;
        //    }

        //    return View("Create");
        //    //return Json(false);
            
        //}

        [HttpPost]
        public IActionResult Capture(int id)
        {
            var tempID = HttpContext.Session.GetString("idval");
            System.Diagnostics.Debug.WriteLine("capture(string) function first line count= " + count);
            if (count < 16)
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
                              
                                var fileName = file.FileName;

                                var myUniqueFileName = count.ToString();
     
                                var fileExtension = Path.GetExtension(fileName);

                                 var newFileName = string.Concat("Image", myUniqueFileName, fileExtension);
      
                                var filepath = Path.Combine(_environment.WebRootPath, "temporary") + $@"\{newFileName}";

                                if (!string.IsNullOrEmpty(filepath))
                                {
                                    ViewData["noOfPics"] = count + "pictures captured";
                                    count++;
                                    StoreInFolder(file, filepath);
                                }

                            }
                        }
                        if (count == 16)
                            count = 1;
                        return Json(count);
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
                //System.Diagnostics.Debug.WriteLine("count of pictures exceeded 15");

                return Json(count);
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

    }
}
