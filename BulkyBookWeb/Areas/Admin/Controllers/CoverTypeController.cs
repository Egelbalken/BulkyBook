using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CoverTypeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CoverTypeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // Use IEnumerable<CoverType> Then you dont need to assig a "var" and end it with .ToString();
        public IActionResult Index()
        {
            IEnumerable<CoverType> objCoverTypeList = _unitOfWork.CoverType.GetAll();

            return View(objCoverTypeList);
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
        public IActionResult Create(CoverType obj)
        {

            // Validate if modalstate is valid.. Class Required fields of coverType
            if (ModelState.IsValid)
            {
                // adds the data to _db object
                _unitOfWork.CoverType.Add(obj);
                // Saves into the database
                _unitOfWork.Save();
                // Will be temporary there for one respons
                TempData["success"] = "CoverType created successfully";
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
            // Find the id that match the coverType in db
            var coverTypeLFromDb = _unitOfWork.CoverType.GetFirstOrDefault(u => u.Id == id);

            if (coverTypeLFromDb == null)
            {
                return NotFound();
            }

            return View(coverTypeLFromDb);
        }

        // POST 
        [HttpPost]
        // Validation Token for Cross side Attacks.. 
        [ValidateAntiForgeryToken]
        // Edit method that updates the object data.
        public IActionResult Edit(CoverType obj)
        {
            // Validate if modalstate is valid.. Class Required fields of coverType
            if (ModelState.IsValid)
            {

                // * Update the data to _db object
                _unitOfWork.CoverType.Update(obj);
                // Saves into the database
                _unitOfWork.Save();
                // Will be temporary there for one respons
                TempData["success"] = "Covertype was updated successfully";
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

            // Find the id that match the coverType in db
            var coverTypeLFromDb = _unitOfWork.CoverType.GetFirstOrDefault(u => u.Id == id);

            if (coverTypeLFromDb == null)
            {
                return NotFound();
            }

            return View(coverTypeLFromDb);
        }

        // POST 
        // Validation Token for Cross side Attacks.. 
        // Method that deletes the object
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePOST(int? id)
        {
            // * Remove the data to _db object
            var obj = _unitOfWork.CoverType.GetFirstOrDefault(u => u.Id == id);

            if (obj == null)
            {
                return NotFound();
            }

            _unitOfWork.CoverType.Remove(obj);

            // Saves into the database
            _unitOfWork.Save();
            // Will be temporary there for one respons
            TempData["success"] = "Covertype was successfully deleted";
            // if not valid, show create view with the unfinnished obj
            return RedirectToAction("Index");
        }
    }
}
