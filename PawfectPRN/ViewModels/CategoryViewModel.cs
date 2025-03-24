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
using System.Text.RegularExpressions;

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
                MessageBox.Show("Vui lòng chọn danh mục để xóa!");
                return;
            }

            var result = MessageBox.Show("Bạn có chắc chắn muốn xóa danh mục này? Nếu xóa, các sản phẩm liên quan có thể bị ảnh hưởng.",
                "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                using (var context = new PawfectPrnContext())
                {
                    var category = context.Categories.FirstOrDefault(c => c.CategoryId == SelectedItem.CategoryId);
                    if (category != null)
                    {
                        if (context.Products.Any(p => p.CategoryId == category.CategoryId))
                        {
                            MessageBox.Show("Không thể xóa danh mục này vì có sản phẩm liên quan!");
                            return;
                        }

                        context.Categories.Remove(category);
                        context.SaveChanges();
                        MessageBox.Show("Xóa danh mục thành công!");                   
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

            // Validation
            if (!ValidateCategoryName(TextBoxItem.CategoryName, out string errorMessage))
            {
                MessageBox.Show(errorMessage);
                return;
            }

            using (var context = new PawfectPrnContext())
            {
                // Kiểm tra trùng lặp CategoryName
                if (context.Categories.Any(c => c.CategoryName.ToLower() == TextBoxItem.CategoryName.ToLower()))
                {
                    MessageBox.Show("Tên danh mục đã tồn tại! Vui lòng chọn tên khác.");
                    return;
                }

                Category newCategory = new Category
                {
                    CategoryName = TextBoxItem.CategoryName
                };

                context.Categories.Add(newCategory);
                context.SaveChanges();
                MessageBox.Show("Thêm danh mục thành công!");
                LoadCategories();
            }
        }

        private void Update(object obj)
        {
            try
            {
                if (SelectedItem == null)
                {
                    MessageBox.Show("Vui lòng chọn danh mục để cập nhật!");
                    return;
                }

                if (TextBoxItem == null)
                {
                    MessageBox.Show("Lỗi: TextBoxItem chưa được khởi tạo!");
                    return;
                }

                // Validation
                if (!ValidateCategoryName(TextBoxItem.CategoryName, out string errorMessage))
                {
                    MessageBox.Show(errorMessage);
                    return;
                }

                using (var context = new PawfectPrnContext())
                {
                    var existingCategory = context.Categories.FirstOrDefault(c => c.CategoryId == SelectedItem.CategoryId);
                    if (existingCategory != null)
                    {
                        // Kiểm tra trùng lặp CategoryName (ngoại trừ chính nó)
                        if (context.Categories.Any(c => c.CategoryName.ToLower() == TextBoxItem.CategoryName.ToLower() && c.CategoryId != SelectedItem.CategoryId))
                        {
                            MessageBox.Show("Tên danh mục đã tồn tại! Vui lòng chọn tên khác.");
                            return;
                        }

                        existingCategory.CategoryName = TextBoxItem.CategoryName;
                        context.SaveChanges();
                        MessageBox.Show("Cập nhật danh mục thành công!");
                        LoadCategories();
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy danh mục để cập nhật!");
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"Lỗi: {e.Message}");
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

        // Hàm validation riêng cho CategoryName
        private bool ValidateCategoryName(string categoryName, out string errorMessage)
        {
            errorMessage = string.Empty;

            // Kiểm tra rỗng
            if (string.IsNullOrWhiteSpace(categoryName))
            {
                errorMessage = "Vui lòng nhập tên danh mục!";
                return false;
            }

            // Kiểm tra độ dài (giả sử tối đa 50 ký tự)
            if (categoryName.Length > 50)
            {
                errorMessage = "Tên danh mục không được vượt quá 50 ký tự!";
                return false;
            }

            // Kiểm tra ký tự hợp lệ: cho phép chữ cái (bao gồm có dấu), số và khoảng trắng
            if (!Regex.IsMatch(categoryName, @"^[\p{L}0-9\s]+$"))
            {
                errorMessage = "Tên danh mục chỉ được chứa chữ cái, số và khoảng trắng!";
                return false;
            }

            return true;
        }
    }
}