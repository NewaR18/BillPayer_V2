using BillPay.DataAccess.Repository.IRepository;
using BillPay.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillPay.Utilities.RolesAndMenus
{
    public class CheckMenus
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUnitOfWork _repo;
        public CheckMenus(SignInManager<ApplicationUser> signInManager,
                            UserManager<ApplicationUser> userManager,
                            RoleManager<ApplicationRole> roleManager,
                            IHttpContextAccessor httpContextAccessor,
                            IUnitOfWork repo) 
        { 
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _httpContextAccessor = httpContextAccessor;
            _repo = repo;
        } 

        public async Task<IEnumerable<string>> GetMenuNames()
        {
            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext!.User);
            IEnumerable<string> menusNames = new List<string>();
            List<string> Menus = new List<string>();
            if (user != null)
            {
                var userRoleNames = await _userManager.GetRolesAsync(user);
                List<string> menuIds = new List<string>();
                //Find List of MenuId By User
                foreach (var userRoleName in userRoleNames)
                {
                    ApplicationRole applicationRole = await _roleManager.FindByNameAsync(userRoleName);
                    if (applicationRole.ListOfMenuId != null)
                    {
                        menuIds.Add(applicationRole.ListOfMenuId);
                    }
                }
                string menuIdsCombined = string.Join(",", menuIds);
                IEnumerable<string> menuIdsCombinedEnumerable = menuIdsCombined.Split(',');
                IEnumerable<string> menuIdsCombinedDistinct = menuIdsCombinedEnumerable.Distinct();
                menusNames = _repo.MenuRepo.GetAll().Where(menu => menuIdsCombinedDistinct.Any(x => x.Equals(menu.MenuId.ToString()))).Select(menu => menu.Name);
                if (menusNames.Count() == 0)
                {
                    Menus.Add("Home");
                    Menus.Add("Account");
                    menusNames = Menus;
                }
            }
            else
            {
                Menus.Add("Home");
                Menus.Add("Account");
                menusNames = Menus;
            }
            return menusNames;
        }
    }
}
