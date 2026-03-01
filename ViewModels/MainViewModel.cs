// ViewModels/MainViewModel.cs
using LibraryManagement.Data;
using LibraryManagement.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace LibraryManagement.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly LibraryContext _context;
        private ObservableCollection<Book> _books;
        private ObservableCollection<Author> _authors;
        private ObservableCollection<Genre> _genres;
        private Author _selectedAuthorFilter;
        private Genre _selectedGenreFilter;
        private string _searchText;

        public event PropertyChangedEventHandler PropertyChanged;

        public MainViewModel()
        {
            _context = new LibraryContext();
            _context.Database.EnsureCreated();
            
            LoadData();
            
            AddBookCommand = new RelayCommand(AddBook);
            EditBookCommand = new RelayCommand(EditBook, CanEditOrDeleteBook);
            DeleteBookCommand = new RelayCommand(DeleteBook, CanEditOrDeleteBook);
            ManageAuthorsCommand = new RelayCommand(ManageAuthors);
            ManageGenresCommand = new RelayCommand(ManageGenres);
        }

        public ObservableCollection<Book> Books
        {
            get { return _books; }
            set { _books = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Author> Authors
        {
            get { return _authors; }
            set { _authors = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Genre> Genres
        {
            get { return _genres; }
            set { _genres = value; OnPropertyChanged(); }
        }

        public Author SelectedAuthorFilter
        {
            get { return _selectedAuthorFilter; }
            set 
            { 
                _selectedAuthorFilter = value; 
                OnPropertyChanged();
                FilterBooks();
            }
        }

        public Genre SelectedGenreFilter
        {
            get { return _selectedGenreFilter; }
            set 
            { 
                _selectedGenreFilter = value; 
                OnPropertyChanged();
                FilterBooks();
            }
        }

        public string SearchText
        {
            get { return _searchText; }
            set 
            { 
                _searchText = value; 
                OnPropertyChanged();
                FilterBooks();
            }
        }

        public Book SelectedBook { get; set; }

        public ICommand AddBookCommand { get; }
        public ICommand EditBookCommand { get; }
        public ICommand DeleteBookCommand { get; }
        public ICommand ManageAuthorsCommand { get; }
        public ICommand ManageGenresCommand { get; }

        private void LoadData()
        {
            Authors = new ObservableCollection<Author>(_context.Authors.ToList());
            Genres = new ObservableCollection<Genre>(_context.Genres.ToList());
            LoadBooks();
        }

        private void LoadBooks()
        {
            var books = _context.Books
                .Include(b => b.Author)
                .Include(b => b.Genre)
                .ToList();
            Books = new ObservableCollection<Book>(books);
        }

        private void FilterBooks()
        {
            var query = _context.Books
                .Include(b => b.Author)
                .Include(b => b.Genre)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                query = query.Where(b => b.Title.Contains(SearchText));
            }

            if (SelectedAuthorFilter != null)
            {
                query = query.Where(b => b.AuthorId == SelectedAuthorFilter.Id);
            }

            if (SelectedGenreFilter != null)
            {
                query = query.Where(b => b.GenreId == SelectedGenreFilter.Id);
            }

            Books = new ObservableCollection<Book>(query.ToList());
        }

        private void AddBook(object parameter)
        {
            var window = new Views.BookWindow(new Book(), _context);
            window.ShowDialog();
            LoadBooks();
        }

        private void EditBook(object parameter)
        {
            if (SelectedBook != null)
            {
                var window = new Views.BookWindow(SelectedBook, _context);
                window.ShowDialog();
                LoadBooks();
            }
        }

        private bool CanEditOrDeleteBook(object parameter)
        {
            return SelectedBook != null;
        }

        private void DeleteBook(object parameter)
        {
            if (SelectedBook != null)
            {
                var result = System.Windows.MessageBox.Show(
                    $"Вы уверены, что хотите удалить книгу '{SelectedBook.Title}'?",
                    "Подтверждение удаления",
                    System.Windows.MessageBoxButton.YesNo,
                    System.Windows.MessageBoxImage.Question);

                if (result == System.Windows.MessageBoxResult.Yes)
                {
                    _context.Books.Remove(SelectedBook);
                    _context.SaveChanges();
                    LoadBooks();
                }
            }
        }

        private void ManageAuthors(object parameter)
        {
            var window = new Views.AuthorsWindow(_context);
            window.ShowDialog();
            Authors = new ObservableCollection<Author>(_context.Authors.ToList());
        }

        private void ManageGenres(object parameter)
        {
            var window = new Views.GenresWindow(_context);
            window.ShowDialog();
            Genres = new ObservableCollection<Genre>(_context.Genres.ToList());
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    // Вспомогательный класс для команд
    public class RelayCommand : ICommand
    {
        private Action<object> _execute;
        private Func<object, bool> _canExecute;

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }
    }
}
