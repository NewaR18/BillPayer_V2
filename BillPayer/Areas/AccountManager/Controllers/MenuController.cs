using BillPay.DataAccess.Repository.IRepository;
using BillPay.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BillPayer.Areas.AccountManager.Controllers
{
    [Area("AccountManager")]
    [Authorize]
    public class MenuController : Controller
    {
        private readonly IUnitOfWork _repo;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        public MenuController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _repo = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public IActionResult Index()
        {
            try
            {
                IEnumerable<Menu> entities = _repo.MenuRepo.GetAll();
                return View(entities);
            }
            catch
            {

                throw;
            }
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Menu entity)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    _repo.MenuRepo.Add(entity);
                    _repo.Save();
                    TempData["success"] = "Item Created Successfully";
                    return RedirectToAction(nameof(Index));
                }
                TempData["error"] = "Item could not be created !! Validation error";
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
                Menu entity = _repo.MenuRepo.GetFirstOrDefault(x => x.MenuId == id);
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
                Menu entity = _repo.MenuRepo.GetFirstOrDefault(x => x.MenuId == id);
                return View(entity);
            }
            catch
            {
                throw;
            }
        }
        [HttpPost]
        public IActionResult Edit(Menu entity)
        {
            try
            {
                _repo.MenuRepo.Update(entity);
                _repo.Save();
                TempData["success"] = "Item Updated Successfully";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                throw;
            }
        }
        #region API
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (id == 0)
                {
                    return Json(new { success = false, message = "Error" });
                }
                else
                {
                    var roles = _roleManager.Roles.ToList().Where(x => x.ListOfMenuId.Split(',').ToList().Contains(id.ToString()));
                    foreach (var role in roles)
                    {
                        List<string> roleMenus = role.ListOfMenuId.Split(',').ToList();
                        if (roleMenus.Exists(x => x.Equals(id.ToString())))
                        {
                            roleMenus.Remove(id.ToString());
                        }
                        role.ListOfMenuId = string.Join(",", roleMenus);
                        var result = await _roleManager.UpdateAsync(role);
                    }
                    Menu entityMenu = _repo.MenuRepo.GetFirstOrDefault(x => x.MenuId.Equals(id));
                    _repo.MenuRepo.Remove(entityMenu);
                    _repo.Save();
                    return Json(new { success = true, message = "Menu Deleted Successfully" });
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
