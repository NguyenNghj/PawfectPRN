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
                using (var context = new PawfectprnContext())
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

            using (var context = new PawfectprnContext())
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

            // Validation sử dụng CategoryValidator từ namespace mới
            if (!CategoryValidator.ValidateCategoryName(TextBoxItem.CategoryName, out string errorMessage))
            {
                MessageBox.Show(errorMessage);
                return;
            }

            using (var context = new PawfectprnContext())
            {
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

                // Validation sử dụng CategoryValidator từ namespace mới
                if (!CategoryValidator.ValidateCategoryName(TextBoxItem.CategoryName, out string errorMessage))
                {
                    MessageBox.Show(errorMessage);
                    return;
                }

                using (var context = new PawfectprnContext())
                {
                    var existingCategory = context.Categories.FirstOrDefault(c => c.CategoryId == SelectedItem.CategoryId);
                    if (existingCategory != null)
                    {
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
            using (var context = new PawfectprnContext())
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