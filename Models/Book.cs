// Models/Book.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Models
{
    public class Book
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(200)]
        public string Title { get; set; }
        
        public int PublishYear { get; set; }
        
        [Required]
        [MaxLength(20)]
        public string ISBN { get; set; }
        
        public int QuantityInStock { get; set; }
        
        // Внешние ключи
        public int AuthorId { get; set; }
        public int GenreId { get; set; }
        
        // Навигационные свойства
        public virtual Author Author { get; set; }
        public virtual Genre Genre { get; set; }
    }
}
