using BilkyBook.Utility;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    /// <summary>
    /// Controller whit "new" way to work, a create and update in one.
    /// </summary>
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // Use IEnumerable<CoverType> Then you dont need to assig a "var" and end it with .ToString();
        public IActionResult Index()
        {
            return View();
        }
       
        // GET
        public IActionResult Upsert(int? id)
        {
            Company company = new();

            // Validate if we have id 
            if (id == null || id == 0)
            {
                return View(company);
            }
            else
            {
                company = _unitOfWork.Company.GetFirstOrDefault(u=>u.Id == id);
                return View(company);
            }

        }

        // POST 
        [HttpPost]
        // Validation Token for Cross side Attacks.. 
        [ValidateAntiForgeryToken]
        // Edit method that updates the object data.
        public IActionResult Upsert(Company obj)
        {
            // Validate if modalstate is valid.. Class Required fields of Product
            if (ModelState.IsValid)
            {
                if(obj.Id == 0)
                {
                    _unitOfWork.Company.Add(obj);
                    TempData["success"] = "Comapny created successfully";
                }
                else
                {
                    _unitOfWork.Company.Update(obj);
                    TempData["success"] = "Comapny updated successfully";
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
        [HttpGet]
        public IActionResult GetAll()
        {
            var companyList = _unitOfWork.Company.GetAll();
            return Json(new { data = companyList });
        }

        // POST 
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            // * Remove the data to _db object
            var obj = _unitOfWork.Company.GetFirstOrDefault(u => u.Id == id);

            
            if (obj == null)
            {
                return Json(new { success = false, message = "Error while deleteing" });
            }

            _unitOfWork.Company.Remove(obj);

            // Saves into the database
            _unitOfWork.Save();

            return Json(new { success = true, message = "Delete Successful" });
        }

        #endregion

    }
}
