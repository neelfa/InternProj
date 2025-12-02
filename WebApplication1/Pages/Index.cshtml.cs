using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IMenuService _menuService;

        public IndexModel(IMenuService menuService)
        {
            _menuService = menuService;
        }

        public IEnumerable<MenuItem> MenuItems { get; set; } = new List<MenuItem>();

        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; } = string.Empty;

        [BindProperty(SupportsGet = true)]
        public MenuCategory? SelectedCategory { get; set; }

        public async Task OnGetAsync()
        {
            if (SelectedCategory.HasValue || !string.IsNullOrWhiteSpace(SearchTerm))
            {
                MenuItems = await _menuService.SearchAsync(SearchTerm, SelectedCategory);
            }
            else
            {
                MenuItems = await _menuService.GetAllAsync();
            }
        }
    }
}