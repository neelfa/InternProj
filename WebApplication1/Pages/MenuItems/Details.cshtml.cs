using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Pages.MenuItems
{
    public class DetailsModel : PageModel
    {
        private readonly IMenuService _menuService;

        public DetailsModel(IMenuService menuService)
        {
            _menuService = menuService;
        }

        public MenuItem MenuItem { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var menuItem = await _menuService.GetByIdAsync(id);
            if (menuItem == null)
            {
                return NotFound();
            }

            MenuItem = menuItem;
            return Page();
        }
    }
}