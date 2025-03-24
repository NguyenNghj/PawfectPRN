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
using PawfectPRN.Validation; // Thêm namespace của Validation

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
                MessageBox.Show("Vui lòng chọn sản phẩm để xóa!");
                return;
            }

            var result = MessageBox.Show("Bạn có chắc chắn muốn xóa sản phẩm này?", "Xác nhận xóa",
                MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                using (var context = new PawfectprnContext())
                {
                    var product = context.Products.FirstOrDefault(p => p.ProductId == SelectedItem.ProductId);
                    if (product != null)
                    {
                        context.Products.Remove(product);
                        context.SaveChanges();
                        MessageBox.Show("Xóa sản phẩm thành công!");
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

            using (var context = new PawfectprnContext())
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

            // Validation sử dụng ProductValidator từ namespace mới
            using (var context = new PawfectprnContext())
            {
                if (!ProductValidator.ValidateProduct(TextBoxItem, out string errorMessage, context))
                {
                    MessageBox.Show(errorMessage);
                    return;
                }

                // Kiểm tra trùng lặp Name
                if (context.Products.Any(p => p.Name.ToLower() == TextBoxItem.Name.ToLower()))
                {
                    MessageBox.Show("Tên sản phẩm đã tồn tại! Vui lòng chọn tên khác.");
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
                MessageBox.Show("Thêm sản phẩm thành công!");
                LoadProducts();
            }
        }

        private void Update(object obj)
        {
            try
            {
                if (SelectedItem == null)
                {
                    MessageBox.Show("Vui lòng chọn sản phẩm để cập nhật!");
                    return;
                }

                if (TextBoxItem == null)
                {
                    MessageBox.Show("Lỗi: TextBoxItem chưa được khởi tạo!");
                    return;
                }

                // Validation sử dụng ProductValidator từ namespace mới
                using (var context = new PawfectprnContext())
                {
                    if (!ProductValidator.ValidateProduct(TextBoxItem, out string errorMessage, context))
                    {
                        MessageBox.Show(errorMessage);
                        return;
                    }

                    var existingProduct = context.Products.FirstOrDefault(p => p.ProductId == SelectedItem.ProductId);
                    if (existingProduct != null)
                    {
                        // Kiểm tra trùng lặp Name (ngoại trừ chính nó)
                        if (context.Products.Any(p => p.Name.ToLower() == TextBoxItem.Name.ToLower() && p.ProductId != SelectedItem.ProductId))
                        {
                            MessageBox.Show("Tên sản phẩm đã tồn tại! Vui lòng chọn tên khác.");
                            return;
                        }

                        existingProduct.Name = TextBoxItem.Name;
                        existingProduct.Description = TextBoxItem.Description;
                        existingProduct.Price = TextBoxItem.Price;
                        existingProduct.StockQuantity = TextBoxItem.StockQuantity;
                        existingProduct.CategoryId = TextBoxItem.CategoryId;

                        context.SaveChanges();
                        MessageBox.Show("Cập nhật sản phẩm thành công!");
                        LoadProducts();
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy sản phẩm để cập nhật!");
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"Lỗi: {e.Message}");
            }
        }

        private void LoadProducts()
        {
            using (var context = new PawfectprnContext())
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
            using (var context = new PawfectprnContext())
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