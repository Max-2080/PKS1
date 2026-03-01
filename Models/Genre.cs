// Models/Genre.cs
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Models
{
    public class Genre
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        
        [MaxLength(500)]
        public string Description { get; set; }
        
        // Навигационное свойство
        public virtual ICollection<Book> Books { get; set; }
    }
}
