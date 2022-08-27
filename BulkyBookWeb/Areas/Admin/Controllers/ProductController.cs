using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    /// <summary>
    /// Controller whit "new" way to work, a create and update in one.
    /// </summary>
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }
       
        // GET
        public IActionResult Upsert(int? id)
        {
            // Implemet product VM to simplyfy the code..
            // If we need to add list selections we can do that in product VM
            ProductVM productVM = new()
            {
                Product = new(),
                CategoryList = _unitOfWork.Category.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
                CoverTypeList = _unitOfWork.CoverType.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
            };

            // Validate if we have id 
            if (id == null || id == 0)
            {
                // Create product
                //ViewBag.CategoryList = CategoryList;
                //ViewData["CoverTypeList"] = CoverTypeList;
                return View(productVM);
            }
            else
            {
                // Update product
                productVM.Product = _unitOfWork.Product.GetFirstOrDefault(u=>u.Id == id);
                return View(productVM);
            }

        }

        // POST 
        [HttpPost]
        // Validation Token for Cross side Attacks.. 
        [ValidateAntiForgeryToken]
        // Edit method that updates the object data.
        public IActionResult Upsert(ProductVM obj, IFormFile? file)
        {
            // Validate if modalstate is valid.. Class Required fields of Product
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if(file != null)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(wwwRootPath, @"Images\Products");
                    var extension = Path.GetExtension(file.FileName);

                    // Uppdate- need to look after the old file if we have one and
                    // replace it if needed.
                    if(obj.Product.ImageUrl != null)
                    {
                        var oldImagePath = Path.Combine(wwwRootPath,            obj.Product.ImageUrl.TrimStart('\\'));

                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        file.CopyTo(fileStreams);
                    }
                    obj.Product.ImageUrl = @"\Images\Products\" + fileName+extension;
                }
                if(obj.Product.Id == 0)
                {
                    // * Update the data to _db object
                    _unitOfWork.Product.Add(obj.Product);
                }
                else
                {
                    _unitOfWork.Product.Update(obj.Product);
                }
                // Saves into the database
                _unitOfWork.Save();
                // Will be temporary there for one respons
                TempData["success"] = "Product was created successfully";
                return RedirectToAction("Index");
            }

            // if not valid, show create view with the unfinnished obj
            return View(obj);
        }


    #region API Calls
    /// <summary>
    /// Creates a API to get the products to show in javascript
    /// </summary>
    /// <returns></returns>
        [HttpGet]
        public IActionResult GetAll()
        {
            var productList = _unitOfWork.Product.GetAll(includeProperties: "Category,CoverType");
            return Json(new { data=productList});
        }

        // POST 
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            // * Remove the data to _db object
            var obj = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == id);

            
            if (obj == null)
            {
                return Json(new { success = false, message = "Error while deleteing" });
            }

                var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath,obj.ImageUrl.TrimStart('\\'));
            

            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }
            _unitOfWork.Product.Remove(obj);

            // Saves into the database
            _unitOfWork.Save();

            return Json(new { success = true, message = "Delete Successful" });
        }

        #endregion

    }
}
