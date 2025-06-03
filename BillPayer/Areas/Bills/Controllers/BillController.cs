using BillPay.DataAccess.Repository;
using BillPay.DataAccess.Repository.IRepository;
using BillPay.Models;
using BillPay.Models.ViewModels.Bills;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace BillPayer.Areas.Bills.Controllers
{
    [Area("Bills")]
    [Authorize]
    public class BillController : Controller
    {
        private readonly IUnitOfWork _repo;
        private readonly UserManager<ApplicationUser> _userManager;
        public BillController(IUnitOfWork repo,UserManager<ApplicationUser> userManager) 
        { 
            _repo = repo;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();

        }
        public IActionResult Create()
        {
            try
            {
                BillRequiredComponents billRequiredComponents = new BillRequiredComponents()
                {
                    BillSummary = new BillSummary(),
                    UserList = _userManager.Users.Select
                (u =>
                    new SelectListItem
                    {
                        Text = u.Name,
                        Value = u.Id.ToString(),
                    }
                ),
                    ProductList = _repo.ProductRepo.GetAll().Select
                (u =>
                    new SelectListItem
                    {
                        Text = u.Name,
                        Value = u.Id.ToString()
                    }
                )
                };
                billRequiredComponents.BillSummary.Date = DateTime.Now;
                billRequiredComponents.BillSummary.BhukkadsList.Add(new Bhukkads() { Id = 1 });
                billRequiredComponents.BillSummary.BhukkadsList[0].Products.Add(new ProductDetails { Id = 1 });
                return View(billRequiredComponents);
            }
            catch
            {
                throw;
            }
            
        }
        [HttpPost]
        public IActionResult Create(BillRequiredComponents billRequiredComponents)
        {
            try
            {
                for (int i = 0; i < billRequiredComponents.BillSummary.BhukkadsList.Count; i++)
                {
                    for (int j = 0; j < billRequiredComponents.BillSummary.BhukkadsList[i].Products.Count; j++)
                    {
                        billRequiredComponents.BillSummary.BhukkadsList[i].TotalOfPerson += billRequiredComponents.BillSummary.BhukkadsList[i].Products[j].Total;
                    }
                    billRequiredComponents.BillSummary.GrandTotal += billRequiredComponents.BillSummary.BhukkadsList[i].TotalOfPerson;
                }
                _repo.BillSummaryRepo.Add(billRequiredComponents.BillSummary);
                _repo.Save();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                throw;
            }
        }

        public IActionResult Details(int id)
        {
            try
            {
                BillSummary billSummary = _repo.BillSummaryRepo.GetFirstOrDefault(x => x.Id == id, IncludeProperties: "Bhukkads");
                if (billSummary == null)
                {
                    TempData["error"] = "Bill Summary Not Found";
                    return RedirectToAction(nameof(Index));
                }
                BillRequiredComponents billRequiredComponents = new BillRequiredComponents()
                {
                    BillSummary = billSummary,
                    UserList = _userManager.Users.Select
                    (u =>
                        new SelectListItem
                        {
                            Text = u.Name,
                            Value = u.Id.ToString(),
                        }
                    ),
                    ProductList = _repo.ProductRepo.GetAll().Select
                    (u =>
                        new SelectListItem
                        {
                            Text = u.Name,
                            Value = u.Id.ToString()
                        }
                    )
                };
                return View(billRequiredComponents);
            }
            catch
            {
                throw;
            }
        }
        public IActionResult ProductsEdit (int id)
        {
            try
            {
                ProductViewModel entity = _repo.BillSummaryRepo.GetInfoBasedOnProductDetails(id);
                return View(entity);
            }
            catch
            {
                throw;
            }
        }
        [HttpPost]
        public IActionResult ProductsEdit(ProductViewModel productViewModel)
        {
            try
            {
                ProductDetails productDetails = new ProductDetails()
                {
                    Id = productViewModel.Id,
                    Qty = productViewModel.Qty,
                    Price = productViewModel.Price,
                    Discount = productViewModel.Discount,
                    Total = productViewModel.Total
                };

                _repo.ProductDetailsRepo.Update(productDetails);
                _repo.Save();
                _repo.ProductDetailsRepo.UpdateTotal(productViewModel.Id);
                return RedirectToAction(nameof(Details), new { Id = productViewModel.BillSummaryId });
            }
            catch
            {
                throw;
            }
        }
        public IActionResult AddProduct(int bhukkadsId)
        {
            try
            {
                AddProductDetailsViewModel entity = _repo.BhukkadsRepo.GetInfoBasedOnBhukkadsId(bhukkadsId);
                entity.ProductList = _repo.ProductRepo.GetAll().Select
                    (u =>
                        new SelectListItem
                        {
                            Text = u.Name,
                            Value = u.Id.ToString()
                        }
                    );
                return View(entity);
            }
            catch
            {
                throw;
            }
        }
        [HttpPost]
        public IActionResult AddProduct(AddProductDetailsViewModel entity)
        {
            try
            {
                ProductDetails productDetails = new ProductDetails()
                {
                    Id = 0,
                    BhukkadsId = entity.BhukkadsId,
                    ProductId = entity.ProductDetails.ProductId,
                    Qty = entity.ProductDetails.Qty,
                    Price = entity.ProductDetails.Price,
                    Discount = entity.ProductDetails.Discount,
                    Total = entity.ProductDetails.Total
                };
                _repo.ProductDetailsRepo.Add(productDetails);
                _repo.Save();
                _repo.ProductDetailsRepo.UpdateTotal(productDetails.Id);
                TempData["success"] = "Product Added Successfully";
                return RedirectToAction(nameof(Details), new { Id = entity.BillSummaryId });
            }
            catch
            {
                throw;
            }
        }
        public IActionResult AddUser(int billSummaryId)
        {
            try
            {
                BillSummary billSummary = _repo.BillSummaryRepo.GetFirstOrDefault(x => x.Id.Equals(billSummaryId));
                if (billSummary == null)
                {
                    TempData["error"] = "Bill Summary not found";
                    return RedirectToAction(nameof(Details), new { Id = billSummaryId });
                }
                BillRequiredComponents billRequiredComponents = new BillRequiredComponents()
                {
                    BillSummary = billSummary,
                    UserList = _userManager.Users.Select
                    (u =>
                        new SelectListItem
                        {
                            Text = u.Name,
                            Value = u.Id.ToString(),
                        }
                    ),
                    ProductList = _repo.ProductRepo.GetAll().Select
                    (u =>
                        new SelectListItem
                        {
                            Text = u.Name,
                            Value = u.Id.ToString()
                        }
                    )
                };
                billRequiredComponents.BillSummary.BhukkadsList.Add(new Bhukkads() { Id = 1 });
                billRequiredComponents.BillSummary.BhukkadsList[0].Products.Add(new ProductDetails { Id = 1 });
                return View(billRequiredComponents);
            }
            catch
            {
                throw;
            }
        }
        [HttpPost]
        public IActionResult AddUser(BillRequiredComponents billRequiredComponents)
        {
            try
            {
                for (int i = 0; i < billRequiredComponents.BillSummary.BhukkadsList[0].Products.Count; i++)
                {
                    billRequiredComponents.BillSummary.BhukkadsList[0].TotalOfPerson += billRequiredComponents.BillSummary.BhukkadsList[0].Products[i].Total;
                }
                Bhukkads bhukkad = new Bhukkads()
                {
                    Id = 0,
                    UserId = billRequiredComponents.BillSummary.BhukkadsList[0].UserId,
                    TotalOfPerson = billRequiredComponents.BillSummary.BhukkadsList[0].TotalOfPerson,
                    Paid = false,
                    BillSummaryId = billRequiredComponents.BillSummary.Id,
                    Products = billRequiredComponents.BillSummary.BhukkadsList[0].Products
                };
                _repo.BhukkadsRepo.Add(bhukkad);
                _repo.Save();
                bool isSuccess = _repo.BillSummaryRepo.UpdateGrandTotal(billRequiredComponents.BillSummary.Id);
                if (!isSuccess)
                {
                    TempData["error"] = "Error Updating GrandTotal";
                }
                return RedirectToAction(nameof(Details), new { Id = billRequiredComponents.BillSummary.Id });
            }
            catch
            {
                throw;
            }
        }

        #region API
        public JsonResult GetAll(int productId)
        {
            try
            {
                var entities = _repo.ProductRepo.GetFirstOrDefault(x => x.Id == productId);
                if (entities != null)
                {
                    return Json(new { success = true, data = entities });
                }
                else
                {
                    return Json(new
                    {
                        success = false,
                        data = new Product()
                        {
                            Id = productId
                        }
                    });
                }
            }
            catch
            {
                throw;
            }
        }
        public JsonResult GetBillsList()
        {
            try
            {
                var entities = _repo.BillSummaryRepo.GetAll();
                return Json(new { data = entities });
            }
            catch
            {
                throw;
            }
        }
        [HttpDelete]
        public JsonResult DeleteBillSummary(int? id)
        {
            try
            {
                BillSummary entity = _repo.BillSummaryRepo.GetFirstOrDefault(x => x.Id == id, IncludeProperties: "Bhukkads");
                if (entity == null)
                {
                    return Json(new { success = false, message = "Error while deleting" });
                }
                else
                {
                    _repo.BillSummaryRepo.Remove(entity);
                    _repo.Save();
                    return Json(new { success = true, message = "Deleted Successfully" });
                }
            }
            catch
            {
                throw;
            }
        }
        [HttpDelete]
        public JsonResult ProductsDelete(int? id)
        {
            try
            {
                ProductDetails productDetails = _repo.ProductDetailsRepo.GetFirstOrDefault(x => x.Id == id);
                if (productDetails == null)
                {
                    return Json(new { success = false, message = "Error while deleting" });
                }
                else
                {
                    _repo.ProductDetailsRepo.Remove(productDetails);
                    _repo.Save();
                    return Json(new { success = true, message = "Deleted Successfully" });
                }
            }
            catch
            {
                throw;
            }
        }
        [HttpPost]
        public JsonResult UpdatePayerId(int BillSummaryId, string PayerId)
        {
            try
            {
                bool isSuccess = _repo.BillSummaryRepo.UpdatePayerId(BillSummaryId, PayerId);
                if (isSuccess == false)
                {
                    return Json(new { success = false, message = "Error while Updating" });
                }
                return Json(new { success = true, message = "Updated Successfully" });
            }
            catch
            {
                throw;
            }
        }
        [HttpPost]
        public JsonResult UpdateDate(int BillSummaryId, DateTime DateEaten)
        {
            try
            {
                bool isSuccess = _repo.BillSummaryRepo.UpdateDate(BillSummaryId, DateEaten);
                if (isSuccess == false)
                {
                    return Json(new { success = false, message = "Error while Updating" });
                }
                return Json(new { success = true, message = "Updated Successfully" });
            }
            catch
            {
                throw;
            }
        }
        #endregion
    }
}
