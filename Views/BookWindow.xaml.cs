// Views/BookWindow.xaml.cs
using LibraryManagement.Data;
using LibraryManagement.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Windows;

namespace LibraryManagement.Views
{
    public partial class BookWindow : Window
    {
        private Book _book;
        private LibraryContext _context;

        public BookWindow(Book book, LibraryContext context)
        {
            InitializeComponent();
            _book = book;
            _context = context;
            
            LoadData();
            
            if (book.Id != 0)
            {
                Title = "Редактирование книги";
                LoadBookData();
            }
            else
            {
                Title = "Добавление книги";
            }
        }

        private void LoadData()
        {
            cmbAuthor.ItemsSource = _context.Authors.ToList();
            cmbGenre.ItemsSource = _context.Genres.ToList();
        }

        private void LoadBookData()
        {
            txtTitle.Text = _book.Title;
            cmbAuthor.SelectedItem = _context.Authors.Find(_book.AuthorId);
            cmbGenre.SelectedItem = _context.Genres.Find(_book.GenreId);
            txtYear.Text = _book.PublishYear.ToString();
            txtISBN.Text = _book.ISBN;
            txtQuantity.Text = _book.QuantityInStock.ToString();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show("Введите название книги");
                return;
            }

            if (cmbAuthor.SelectedItem == null)
            {
                MessageBox.Show("Выберите автора");
                return;
            }

            if (cmbGenre.SelectedItem == null)
            {
                MessageBox.Show("Выберите жанр");
                return;
            }

            _book.Title = txtTitle.Text;
            _book.AuthorId = ((Author)cmbAuthor.SelectedItem).Id;
            _book.GenreId = ((Genre)cmbGenre.SelectedItem).Id;
            _book.PublishYear = int.Parse(txtYear.Text);
            _book.ISBN = txtISBN.Text;
            _book.QuantityInStock = int.Parse(txtQuantity.Text);

            if (_book.Id == 0)
            {
                _context.Books.Add(_book);
            }
            else
            {
                _context.Entry(_book).State = EntityState.Modified;
            }

            _context.SaveChanges();
            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
