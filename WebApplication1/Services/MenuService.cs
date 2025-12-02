using WebApplication1.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace WebApplication1.Services
{
    public interface IMenuService
    {
        Task<IEnumerable<MenuItem>> GetAllAsync();
        Task<MenuItem?> GetByIdAsync(int id);
        Task<MenuItem> AddAsync(MenuItem item);
        Task<MenuItem?> UpdateAsync(MenuItem item);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<MenuItem>> SearchAsync(string searchTerm, MenuCategory? category = null);
        Task<string> SaveImageAsync(IFormFile imageFile);
    }

    public class MenuService : IMenuService
    {
        private readonly List<MenuItem> _menuItems;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private int _nextId = 1;

        public MenuService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            _menuItems = new List<MenuItem>
            {
                new MenuItem
                {
                    Id = _nextId++,
                    Name = "Caesar Salad",
                    Price = 12.99m,
                    Category = MenuCategory.Appetizers,
                    Description = "Crisp romaine lettuce with parmesan and house-made croutons",
                    ImagePath = "https://images.unsplash.com/photo-1551248429-40975aa4de74?w=400"
                },
                new MenuItem
                {
                    Id = _nextId++,
                    Name = "Grilled Salmon",
                    Price = 24.99m,
                    Category = MenuCategory.Meals,
                    Description = "Atlantic salmon fillet with seasonal vegetables and lemon butter sauce",
                    ImagePath = "https://images.unsplash.com/photo-1467003909585-2f8a72700288?w=400"
                },
                new MenuItem
                {
                    Id = _nextId++,
                    Name = "Chocolate Cake",
                    Price = 8.99m,
                    Category = MenuCategory.Desserts,
                    Description = "Rich chocolate cake with ganache and fresh berries",
                    ImagePath = "https://images.unsplash.com/photo-1578985545062-69928b1d9587?w=400"
                },
                new MenuItem
                {
                    Id = _nextId++,
                    Name = "Iced Latte",
                    Price = 5.99m,
                    Category = MenuCategory.Drinks,
                    Description = "Cold brew coffee with milk and a touch of vanilla",
                    ImagePath = "https://images.unsplash.com/photo-1461023058943-07fcbe16d735?w=400"
                }
            };

            // Create uploads directory if it doesn't exist
            var uploadsPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }
        }

        public Task<IEnumerable<MenuItem>> GetAllAsync()
        {
            return Task.FromResult(_menuItems.AsEnumerable());
        }

        public Task<MenuItem?> GetByIdAsync(int id)
        {
            var item = _menuItems.FirstOrDefault(x => x.Id == id);
            return Task.FromResult(item);
        }

        public Task<MenuItem> AddAsync(MenuItem item)
        {
            item.Id = _nextId++;
            _menuItems.Add(item);
            return Task.FromResult(item);
        }

        public Task<MenuItem?> UpdateAsync(MenuItem item)
        {
            var existingItem = _menuItems.FirstOrDefault(x => x.Id == item.Id);
            if (existingItem != null)
            {
                existingItem.Name = item.Name;
                existingItem.Price = item.Price;
                existingItem.Category = item.Category;
                existingItem.Description = item.Description;
                existingItem.ImagePath = item.ImagePath;
                return Task.FromResult(existingItem);
            }
            return Task.FromResult<MenuItem?>(null);
        }

        public Task<bool> DeleteAsync(int id)
        {
            var item = _menuItems.FirstOrDefault(x => x.Id == id);
            if (item != null)
            {
                // Delete associated image file if it exists and is not a URL
                if (!string.IsNullOrEmpty(item.ImagePath) && !item.ImagePath.StartsWith("http"))
                {
                    var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, item.ImagePath.TrimStart('/'));
                    if (File.Exists(imagePath))
                    {
                        try
                        {
                            File.Delete(imagePath);
                        }
                        catch
                        {
                            // Ignore file deletion errors
                        }
                    }
                }
                _menuItems.Remove(item);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        public Task<IEnumerable<MenuItem>> SearchAsync(string searchTerm, MenuCategory? category = null)
        {
            var query = _menuItems.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(x => x.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                                       x.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
            }

            if (category.HasValue)
            {
                query = query.Where(x => x.Category == category.Value);
            }

            return Task.FromResult(query);
        }

        public async Task<string> SaveImageAsync(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
                return string.Empty;

            // Generate unique filename
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
            var uploadsPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
            var filePath = Path.Combine(uploadsPath, fileName);

            // Save the file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            // Return relative path for web access
            return $"/uploads/{fileName}";
        }
    }
}