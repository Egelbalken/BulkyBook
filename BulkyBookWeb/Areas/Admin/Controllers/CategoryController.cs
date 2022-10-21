using BilkyBook.Utility;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // Use IEnumerable<Category> Then you dont need to assig a "var" and end it with .ToString();
        public IActionResult Index()
        {
            IEnumerable<Category> objCategoryList = _unitOfWork.Category.GetAll();

            return View(objCategoryList);
        }

        // Get
        public IActionResult Create()
        {

            return View();
        }

        // Get
        // Post 
        [HttpPost]
        // Validation Token for Cross side Attacks.. 
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category obj)
        {
            // Validate whit a Custom error message, so we not writing the same thing in the inputs.
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "The DisplayOrder can not exactly match the Name.");
            }

            // Validate if modalstate is valid.. Class Required fields of category
            if (ModelState.IsValid)
            {

                // adds the data to _db object
                _unitOfWork.Category.Add(obj);
                // Saves into the database
                _unitOfWork.Save();
                // Will be temporary there for one respons
                TempData["success"] = "Category created successfully";
                return RedirectToAction("Index");
            }

            // if not valid, show create view with the unfinnished obj
            return View(obj);
        }

        // GET
        // Gets the id of the db Key
        public IActionResult Edit(int? id)
        {
            // Validate if we have id 
            if (id == null || id == 0)
            {
                return NotFound();
            }
            // Find the id that match the category in db
            var categoryFromDb = _unitOfWork.Category.GetFirstOrDefault(u => u.Id == id);

            //var categoryFromDbFirst = _db.Categories.FirstOrDefault(u => u.Id==id);

            //var categoryFromDbSingle = _db.Categories.SingleOrDefault(u => u.Id == id);

            if (categoryFromDb == null)
            {
                return NotFound();
            }

            return View(categoryFromDb);
        }

        // POST 
        [HttpPost]
        // Validation Token for Cross side Attacks.. 
        [ValidateAntiForgeryToken]
        // Edit method that updates the object data.
        public IActionResult Edit(Category obj)
        {
            // Validate whit a Custom error message, so we not writing the same thing in the inputs.
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "The DisplayOrder can not exactly match the Name.");
            }

            // Validate if modalstate is valid.. Class Required fields of category
            if (ModelState.IsValid)
            {

                // * Update the data to _db object
                _unitOfWork.Category.Update(obj);
                // Saves into the database
                _unitOfWork.Save();
                // Will be temporary there for one respons
                TempData["success"] = "Category was updated successfully";
                return RedirectToAction("Index");
            }

            // if not valid, show create view with the unfinnished obj
            return View(obj);
        }

        // GET
        // Gets the id of the db Key to delete
        public IActionResult Delete(int? id)
        {
            // Validate if we have id 
            if (id == null || id == 0)
            {
                return NotFound();
            }

            // Find the id that match the category in db
            var categoryFromDbFirst = _unitOfWork.Category.GetFirstOrDefault(u => u.Id == id);

            if (categoryFromDbFirst == null)
            {
                return NotFound();
            }

            return View(categoryFromDbFirst);
        }

        // POST 
        // Validation Token for Cross side Attacks.. 
        // Method that deletes the object
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePOST(int? id)
        {
            // * Remove the data to _db object
            var obj = _unitOfWork.Category.GetFirstOrDefault(u => u.Id == id);

            if (obj == null)
            {
                return NotFound();
            }

            _unitOfWork.Category.Remove(obj);

            // Saves into the database
            _unitOfWork.Save();
            // Will be temporary there for one respons
            TempData["success"] = "Category was successfully deleted";
            // if not valid, show create view with the unfinnished obj
            return RedirectToAction("Index");
        }
    }
}
