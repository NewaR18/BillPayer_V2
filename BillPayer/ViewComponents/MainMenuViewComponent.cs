using BillPay.DataAccess.Repository;
using BillPay.DataAccess.Repository.IRepository;
using BillPay.Utilities.RolesAndMenus;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;

namespace BillPayer.ViewComponents
{
    public class MainMenuViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _repo;
        private readonly CheckMenus _checkMenu;
        public MainMenuViewComponent(IUnitOfWork repo, CheckMenus checkMenu)
        {
            _repo = repo;
            _checkMenu = checkMenu;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            IEnumerable<string> menusNames = await _checkMenu.GetMenuNames();
            return View(menusNames);
        }
    }
}
