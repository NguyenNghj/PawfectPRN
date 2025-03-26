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
    public class ProductViewModel : BaseViewModel
    {
        private ObservableCollection<Product> allProducts { get; set; }
        public ObservableCollection<Product> products { get; set; }
        public ObservableCollection<Category> categories { get; set; }

        public ICommand AddCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        public ICommand UpdateCommand { get; set; }
        public ICommand ResetCommand { get; set; }

        public ProductViewModel()
        {
            LoadCategory();
            LoadProducts();
            TextBoxItem = new Product();
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
                MessageBox.Show("Please select a product to delete!");
                return;
            }

            var result = MessageBox.Show("Are you sure you want to delete this product?", "Delete Confirmation",
                MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                using (var context = new PawfectPrnContext())
                {
                    var product = context.Products.FirstOrDefault(p => p.ProductId == SelectedItem.ProductId);
                    if (product != null)
                    {
                        context.Products.Remove(product);
                        context.SaveChanges();
                        MessageBox.Show("Product deleted successfully!");
                        LoadProducts();
                    }
                }
            }
        }

        private void Reset(object obj)
        {
            TextBoxItem = new Product();
            SearchText = null;
            SelectedCategory = null;
            OnPropertyChanged(nameof(TextBoxItem));
            OnPropertyChanged(nameof(SelectedCategory));
            OnPropertyChanged(nameof(SearchText));
            LoadProducts();
        }

        private void Search(object obj)
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                LoadProducts();
                return;
            }

            using (var context = new PawfectPrnContext())
            {
                var filteredProducts = context.Products
                    .Where(p => p.Name.ToLower().Contains(SearchText.ToLower()))
                    .ToList();
                products = new ObservableCollection<Product>(filteredProducts);
                OnPropertyChanged(nameof(products));
            }
        }

        private void Add(object obj)
        {
            if (TextBoxItem == null)
            {
                TextBoxItem = new Product();
            }

    
            using (var context = new PawfectPrnContext())
            {
                if (!ProductValidator.ValidateProduct(TextBoxItem, out string errorMessage, context))
                {
                    MessageBox.Show(errorMessage);
                    return;
                }

                if (context.Products.Any(p => p.Name.ToLower() == TextBoxItem.Name.ToLower()))
                {
                    MessageBox.Show("Product name already exists! Please choose a different name.");
                    return;
                }

                Product newProduct = new Product
                {
                    Name = TextBoxItem.Name,
                    Description = TextBoxItem.Description,
                    Price = TextBoxItem.Price,
                    StockQuantity = TextBoxItem.StockQuantity,
                    CategoryId = TextBoxItem.CategoryId
                };

                context.Products.Add(newProduct);
                context.SaveChanges();
                MessageBox.Show("Product added successfully!");
                LoadProducts();
            }
        }

        private void Update(object obj)
        {
            try
            {
                if (SelectedItem == null)
                {
                    MessageBox.Show("Please select a product to update!");
                    return;
                }

                if (TextBoxItem == null)
                {
                    MessageBox.Show("Error: TextBoxItem has not been initialized!");
                    return;
                }

          
                using (var context = new PawfectPrnContext())
                {
                    if (!ProductValidator.ValidateProduct(TextBoxItem, out string errorMessage, context))
                    {
                        MessageBox.Show(errorMessage);
                        return;
                    }

                    var existingProduct = context.Products.FirstOrDefault(p => p.ProductId == SelectedItem.ProductId);
                    if (existingProduct != null)
                    {
               
                        if (context.Products.Any(p => p.Name.ToLower() == TextBoxItem.Name.ToLower() && p.ProductId != SelectedItem.ProductId))
                        {
                            MessageBox.Show("Product name already exists! Please choose a different name.");
                            return;
                        }

                        existingProduct.Name = TextBoxItem.Name;
                        existingProduct.Description = TextBoxItem.Description;
                        existingProduct.Price = TextBoxItem.Price;
                        existingProduct.StockQuantity = TextBoxItem.StockQuantity;
                        existingProduct.CategoryId = TextBoxItem.CategoryId;

                        context.SaveChanges();
                        MessageBox.Show("Product updated successfully!");
                        LoadProducts();
                    }
                    else
                    {
                        MessageBox.Show("Product not found for update!");
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"Error: {e.Message}");
            }
        }

        private void LoadProducts()
        {
            using (var context = new PawfectPrnContext())
            {
                var productList = context.Products
                    .Include(p => p.Category)
                    .ToList();
                products = new ObservableCollection<Product>(productList);
                OnPropertyChanged(nameof(products));
            }
        }

        private void LoadCategory()
        {
            using (var context = new PawfectPrnContext())
            {
                categories = new ObservableCollection<Category>(context.Categories.ToList());
                OnPropertyChanged(nameof(categories));
            }
        }

        private Category _selectedCategory;
        public Category SelectedCategory
        {
            get { return _selectedCategory; }
            set
            {
                _selectedCategory = value;
                OnPropertyChanged(nameof(SelectedCategory));
                if (_selectedCategory != null && TextBoxItem != null)
                {
                    TextBoxItem.CategoryId = _selectedCategory.CategoryId;
                }
            }
        }

        private Product _textBoxItem;
        public Product TextBoxItem
        {
            get { return _textBoxItem; }
            set
            {
                _textBoxItem = value;
                if (_textBoxItem != null && categories != null)
                {
                    SelectedCategory = categories.FirstOrDefault(c => c.CategoryId == _textBoxItem.CategoryId);
                }
                OnPropertyChanged(nameof(TextBoxItem));
            }
        }

        private Product _selectedItem;
        public Product SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));
                if (_selectedItem != null)
                {
                    TextBoxItem = new Product
                    {
                        ProductId = _selectedItem.ProductId,
                        CategoryId = _selectedItem.CategoryId,
                        Name = _selectedItem.Name,
                        Description = _selectedItem.Description,
                        Price = _selectedItem.Price,
                        StockQuantity = _selectedItem.StockQuantity,
                        Category = _selectedItem.Category
                    };
                }
                else
                {
                    TextBoxItem = new Product();
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