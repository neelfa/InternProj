using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.Models;
using WebApplication1.Services;
using System.IO;

namespace WebApplication1.Pages.MenuItems
{
    public class EditModel : PageModel
    {
        private readonly IMenuService _menuService;

        public EditModel(IMenuService menuService)
        {
            _menuService = menuService;
        }

        [BindProperty]
        public MenuItem MenuItem { get; set; } = new MenuItem();

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
                    // Get the existing item to delete old image if needed
                    var existingItem = await _menuService.GetByIdAsync(MenuItem.Id);
                    
                    // Save new image
                    MenuItem.ImagePath = await _menuService.SaveImageAsync(MenuItem.ImageFile);
                    
                    // Delete old image file if it exists and is not a URL
                    if (existingItem != null && !string.IsNullOrEmpty(existingItem.ImagePath) && 
                        !existingItem.ImagePath.StartsWith("http"))
                    {
                        var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existingItem.ImagePath.TrimStart('/'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            try
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                            catch
                            {
                                // Ignore file deletion errors
                            }
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("MenuItem.ImageFile", "Please upload a JPG or PNG image.");
                    return Page();
                }
            }

            var result = await _menuService.UpdateAsync(MenuItem);
            if (result == null)
            {
                return NotFound();
            }

            return RedirectToPage("/Index");
        }
    }
}