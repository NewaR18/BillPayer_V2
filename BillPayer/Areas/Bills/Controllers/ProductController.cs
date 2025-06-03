using BillPay.DataAccess.Repository.IRepository;
using BillPay.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace BillPayer.Areas.Bills.Controllers
{
    [Authorize]
    [Area("Bills")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _repo;
        public ProductController(IUnitOfWork unitOfWork)
        {
            _repo = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Product entity)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    _repo.ProductRepo.Add(entity);
                    _repo.Save();
                    TempData["success"] = "Product Created Successfully";
                    return RedirectToAction(nameof(Index));
                }
                TempData["error"] = "Product could not be created !! Validation error";
                return View();
            }
            catch
            {
                throw;
            }
        }
        [HttpGet]
        public IActionResult Details(int id)
        {
            try
            {
                Product entity = _repo.ProductRepo.GetFirstOrDefault(x => x.Id == id);
                return View(entity);
            }
            catch
            {
                throw;
            }
        }
        public IActionResult Edit(int id)
        {
            try
            {
                Product entity = _repo.ProductRepo.GetFirstOrDefault(x => x.Id == id);
                return View(entity);
            }
            catch
            {
                throw;
            }
        }
        [HttpPost]
        public IActionResult Edit(Product entity)
        {
            try
            {
                _repo.ProductRepo.Update(entity);
                _repo.Save();
                TempData["success"] = "Company Updated Successfully";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                throw;
            }
        }
        #region API
        [HttpGet]
        public JsonResult GetAll()
        {
            try
            {
                IEnumerable<Product> entities = _repo.ProductRepo.GetAll();
                return Json(new { data = entities });
            }
            catch
            {
                throw;
            }
        }
        public IActionResult Delete(int id)
        {
            try
            {
                Product product = _repo.ProductRepo.GetFirstOrDefault(x => x.Id == id);
                if (product == null)
                {
                    return Json(new { success = false, message = "Error while deleting" });
                }
                else
                {
                    _repo.ProductRepo.Remove(product);
                    _repo.Save();
                    return Json(new { success = true, message = "Deleted Successfully" });
                }
            }
            catch
            {
                throw;
            }
        }
        #endregion
    }
}
