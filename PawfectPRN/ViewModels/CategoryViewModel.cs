using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Newtonsoft.Json;
using System.Windows;
using FirstCode.Helper;
using FirstCode.ViewModels;
using PawfectPRN.Models;
using Microsoft.EntityFrameworkCore;
using PawfectPRN.Validation;
namespace PawfectPRN.ViewModels
{
    public class CategoryViewModel : BaseViewModel
    {
        public ObservableCollection<Category> categories { get; set; }

        public ICommand AddCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        public ICommand UpdateCommand { get; set; }
        public ICommand ResetCommand { get; set; }

        public CategoryViewModel()
        {
            LoadCategories();
            TextBoxItem = new Category();
            AddCommand = new RelayCommand(Add);
            UpdateCommand = new RelayCommand(Update);
            SearchCommand = new RelayCommand(Search);
            DeleteCommand = new RelayCommand(Delete);
            ResetCommand = new RelayCommand(Reset);
        }

        private void Delete(object obj)
        {
            if (SelectedItem == null)
            {
                MessageBox.Show("Please select a category to delete!");
                return;
            }

            var result = MessageBox.Show("Are you sure you want to delete this category? Deleting it may affect related products.",
                "Delete Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                using (var context = new PawfectPrnContext())
                {
                    var category = context.Categories.FirstOrDefault(c => c.CategoryId == SelectedItem.CategoryId);
                    if (category != null)
                    {
                        if (context.Products.Any(p => p.CategoryId == category.CategoryId))
                        {
                            MessageBox.Show("Cannot delete this category because it is associated with products!");
                            return;
                        }

                        context.Categories.Remove(category);
                        context.SaveChanges();
                        MessageBox.Show("Category deleted successfully!");
                        LoadCategories();
                    }
                }
            }
        }

        private void Reset(object obj)
        {
            TextBoxItem = new Category();
            SearchText = null;
            OnPropertyChanged(nameof(TextBoxItem));
            OnPropertyChanged(nameof(SearchText));
            LoadCategories();
        }

        private void Search(object obj)
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                LoadCategories();
                return;
            }

            using (var context = new PawfectPrnContext())
            {
                var filteredCategories = context.Categories
                    .Where(c => c.CategoryName.ToLower().Contains(SearchText.ToLower()))
                    .ToList();
                categories = new ObservableCollection<Category>(filteredCategories);
                OnPropertyChanged(nameof(categories));
            }
        }

        private void Add(object obj)
        {
            if (TextBoxItem == null)
            {
                TextBoxItem = new Category();
            }

            // Validation using CategoryValidator from the new namespace
            if (!CategoryValidator.ValidateCategoryName(TextBoxItem.CategoryName, out string errorMessage))
            {
                MessageBox.Show(errorMessage);
                return;
            }

            using (var context = new PawfectPrnContext())
            {
                if (context.Categories.Any(c => c.CategoryName.ToLower() == TextBoxItem.CategoryName.ToLower()))
                {
                    MessageBox.Show("Category name already exists! Please choose a different name.");
                    return;
                }

                Category newCategory = new Category
                {
                    CategoryName = TextBoxItem.CategoryName
                };

                context.Categories.Add(newCategory);
                context.SaveChanges();
                MessageBox.Show("Category added successfully!");
                LoadCategories();
            }
        }

        private void Update(object obj)
        {
            try
            {
                if (SelectedItem == null)
                {
                    MessageBox.Show("Please select a category to update!");
                    return;
                }

                if (TextBoxItem == null)
                {
                    MessageBox.Show("Error: TextBoxItem has not been initialized!");
                    return;
                }

                // Validation using CategoryValidator from the new namespace
                if (!CategoryValidator.ValidateCategoryName(TextBoxItem.CategoryName, out string errorMessage))
                {
                    MessageBox.Show(errorMessage);
                    return;
                }

                using (var context = new PawfectPrnContext())
                {
                    var existingCategory = context.Categories.FirstOrDefault(c => c.CategoryId == SelectedItem.CategoryId);
                    if (existingCategory != null)
                    {
                        if (context.Categories.Any(c => c.CategoryName.ToLower() == TextBoxItem.CategoryName.ToLower() && c.CategoryId != SelectedItem.CategoryId))
                        {
                            MessageBox.Show("Category name already exists! Please choose a different name.");
                            return;
                        }

                        existingCategory.CategoryName = TextBoxItem.CategoryName;
                        context.SaveChanges();
                        MessageBox.Show("Category updated successfully!");
                        LoadCategories();
                    }
                    else
                    {
                        MessageBox.Show("Category not found for update!");
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"Error: {e.Message}");
            }
        }

        private void LoadCategories()
        {
            using (var context = new PawfectPrnContext())
            {
                var categoryList = context.Categories.ToList();
                categories = new ObservableCollection<Category>(categoryList);
                OnPropertyChanged(nameof(categories));
            }
        }

        private Category _textBoxItem;
        public Category TextBoxItem
        {
            get { return _textBoxItem; }
            set
            {
                _textBoxItem = value;
                OnPropertyChanged(nameof(TextBoxItem));
            }
        }

        private Category _selectedItem;
        public Category SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));
                if (_selectedItem != null)
                {
                    TextBoxItem = new Category
                    {
                        CategoryId = _selectedItem.CategoryId,
                        CategoryName = _selectedItem.CategoryName
                    };
                }
                else
                {
                    TextBoxItem = new Category();
                }
            }
        }

        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
            }
        }
    }
}