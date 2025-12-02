using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace WebApplication1.Models
{
    public class MenuItem
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Item name is required")]
        [StringLength(100, ErrorMessage = "Item name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        [Display(Name = "Price (?)")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public MenuCategory Category { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Food Image")]
        public string? ImagePath { get; set; }

        // For file uploads - not stored in database
        public IFormFile? ImageFile { get; set; }
    }

    public enum MenuCategory
    {
        Appetizers,
        Meals,
        Desserts,
        Drinks
    }
}