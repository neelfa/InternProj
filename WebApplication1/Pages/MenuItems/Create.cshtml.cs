using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Pages.MenuItems
{
    public class CreateModel : PageModel
    {
        private readonly IMenuService _menuService;

        public CreateModel(IMenuService menuService)
        {
            _menuService = menuService;
        }

        [BindProperty]
        public MenuItem MenuItem { get; set; } = new MenuItem();

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Handle image upload
            if (MenuItem.ImageFile != null && MenuItem.ImageFile.Length > 0)
            {
                // Validate file type
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var fileExtension = Path.GetExtension(MenuItem.ImageFile.FileName).ToLowerInvariant();

                if (allowedExtensions.Contains(fileExtension))
                {
                    MenuItem.ImagePath = await _menuService.SaveImageAsync(MenuItem.ImageFile);
                }
                else
                {
                    ModelState.AddModelError("MenuItem.ImageFile", "Please upload a JPG or PNG image.");
                    return Page();
                }
            }

            await _menuService.AddAsync(MenuItem);
            return RedirectToPage("/Index");
        }
    }
}