// Views/AuthorsWindow.xaml.cs
using LibraryManagement.Data;
using LibraryManagement.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace LibraryManagement.Views
{
    public partial class AuthorsWindow : Window
    {
        private LibraryContext _context;
        private ObservableCollection<Author> _authors;

        public AuthorsWindow(LibraryContext context)
        {
            InitializeComponent();
            _context = context;
            LoadData();
        }

        private void LoadData()
        {
            _authors = new ObservableCollection<Author>(_context.Authors.ToList());
            dgAuthors.ItemsSource = _authors;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var newAuthor = new Author();
            _authors.Add(newAuthor);
            dgAuthors.ScrollIntoView(newAuthor);
            dgAuthors.BeginEdit();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                dgAuthors.CommitEdit();
                _context.SaveChanges();
                MessageBox.Show("Изменения сохранены");
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}");
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selectedAuthor = dgAuthors.SelectedItem as Author;
            if (selectedAuthor != null)
            {
                var result = MessageBox.Show($"Удалить автора {selectedAuthor.FullName}?", 
                    "Подтверждение", MessageBoxButton.YesNo);
                
                if (result == MessageBoxResult.Yes)
                {
                    if (selectedAuthor.Id != 0)
                    {
                        _context.Authors.Remove(selectedAuthor);
                    }
                    _authors.Remove(selectedAuthor);
                }
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
