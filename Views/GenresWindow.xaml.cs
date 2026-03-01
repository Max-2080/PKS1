// Views/GenresWindow.xaml.cs
using LibraryManagement.Data;
using LibraryManagement.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace LibraryManagement.Views
{
    public partial class GenresWindow : Window
    {
        private LibraryContext _context;
        private ObservableCollection<Genre> _genres;

        public GenresWindow(LibraryContext context)
        {
            InitializeComponent();
            _context = context;
            LoadData();
        }

        private void LoadData()
        {
            _genres = new ObservableCollection<Genre>(_context.Genres.ToList());
            dgGenres.ItemsSource = _genres;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var newGenre = new Genre();
            _genres.Add(newGenre);
            dgGenres.ScrollIntoView(newGenre);
            dgGenres.BeginEdit();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                dgGenres.CommitEdit();
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
            var selectedGenre = dgGenres.SelectedItem as Genre;
            if (selectedGenre != null)
            {
                var result = MessageBox.Show($"Удалить жанр {selectedGenre.Name}?", 
                    "Подтверждение", MessageBoxButton.YesNo);
                
                if (result == MessageBoxResult.Yes)
                {
                    if (selectedGenre.Id != 0)
                    {
                        _context.Genres.Remove(selectedGenre);
                    }
                    _genres.Remove(selectedGenre);
                }
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
