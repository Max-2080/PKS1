// Data/LibraryContext.cs
using Microsoft.EntityFrameworkCore;
using LibraryManagement.Models;
using System;

namespace LibraryManagement.Data
{
    public class LibraryContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Genre> Genres { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=LibraryDB;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Настройка Author
            modelBuilder.Entity<Author>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.Property(a => a.FirstName).IsRequired().HasMaxLength(50);
                entity.Property(a => a.LastName).IsRequired().HasMaxLength(50);
                entity.Property(a => a.Country).HasMaxLength(100);
            });

            // Настройка Genre
            modelBuilder.Entity<Genre>(entity =>
            {
                entity.HasKey(g => g.Id);
                entity.Property(g => g.Name).IsRequired().HasMaxLength(50);
                entity.Property(g => g.Description).HasMaxLength(500);
            });

            // Настройка Book
            modelBuilder.Entity<Book>(entity =>
            {
                entity.HasKey(b => b.Id);
                entity.Property(b => b.Title).IsRequired().HasMaxLength(200);
                entity.Property(b => b.ISBN).IsRequired().HasMaxLength(20);
                entity.Property(b => b.PublishYear).IsRequired();
                entity.Property(b => b.QuantityInStock).IsRequired();

                // Связи
                entity.HasOne(b => b.Author)
                    .WithMany(a => a.Books)
                    .HasForeignKey(b => b.AuthorId)
                    .OnDelete(DeleteBehavior.Restrict); // Не удалять автора при удалении книги

                entity.HasOne(b => b.Genre)
                    .WithMany(g => g.Books)
                    .HasForeignKey(b => b.GenreId)
                    .OnDelete(DeleteBehavior.Restrict); // Не удалять жанр при удалении книги
            });

            // Начальные данные
            modelBuilder.Entity<Genre>().HasData(
                new Genre { Id = 1, Name = "Роман", Description = "Художественная литература в жанре роман" },
                new Genre { Id = 2, Name = "Научная литература", Description = "Научные и образовательные издания" },
                new Genre { Id = 3, Name = "Детектив", Description = "Детективные романы и повести" }
            );

            modelBuilder.Entity<Author>().HasData(
                new Author { Id = 1, FirstName = "Лев", LastName = "Толстой", BirthDate = new DateTime(1828, 9, 9), Country = "Россия" },
                new Author { Id = 2, FirstName = "Федор", LastName = "Достоевский", BirthDate = new DateTime(1821, 11, 11), Country = "Россия" }
            );

            modelBuilder.Entity<Book>().HasData(
                new Book { Id = 1, Title = "Война и мир", AuthorId = 1, GenreId = 1, PublishYear = 1869, ISBN = "978-5-17-090630-5", QuantityInStock = 5 },
                new Book { Id = 2, Title = "Преступление и наказание", AuthorId = 2, GenreId = 1, PublishYear = 1866, ISBN = "978-5-04-088649-3", QuantityInStock = 3 }
            );
        }
    }
}
