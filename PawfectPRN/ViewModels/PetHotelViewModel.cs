using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FirstCode.Helper;
using PawfectPRN.Models;
using System.Windows.Input;
using System.Windows;
using FirstCode.ViewModels;
using PawfectPRN.Validation;

namespace PawfectPRN.ViewModels
{
    public class PetHotelViewModel : BaseViewModel
    {
        // Danh sách khách sạn hiển thị trên giao diện
        public ObservableCollection<PetHotel> petHotels { get; set; }
        private List<string> _sizeOptions;
        public List<string> SizeOptions
        {
            get => _sizeOptions;
            set
            {
                _sizeOptions = value;
                OnPropertyChanged(nameof(SizeOptions));
            }
        }


        // Các command cho thao tác thêm, xóa, cập nhật, tìm kiếm
        public ICommand AddCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public RelayCommand NewCommand { get; }
        public ICommand SearchCommand { get; set; }
        public ICommand UpdateCommand { get; set; }

        public PetHotelViewModel()

        {
            SizeOptions = new List<string> { "Small", "Large", "Medium" };

            LoadPetHotels();
            AddCommand = new RelayCommand(Add);
            UpdateCommand = new RelayCommand(Update);
            SearchCommand = new RelayCommand(Search);
            DeleteCommand = new RelayCommand(Delete);
            NewCommand = new RelayCommand(NewItem);
        }

        private void LoadPetHotels()
        {
          using (var context = new PawfectPrnContext())
            {
                var hotelList = context.PetHotels.ToList();
                petHotels = new ObservableCollection<PetHotel>(hotelList);
                OnPropertyChanged(nameof(petHotels));
            }
        }
        private void NewItem(object obj)
        {
            SelectedItem = null;
            TextBoxItem = new PetHotel();
            OnPropertyChanged(nameof(TextBoxItem));
        }

        private void Add(object obj)
        {
            if (TextBoxItem == null)
            {
                TextBoxItem = new PetHotel();
            }

            using (var context = new PawfectPrnContext())
            {
                if (!SizeOptions.Contains(TextBoxItem.Size))
                {
                    MessageBox.Show("Vui lòng chọn kích thước hợp lệ: Small, Medium, Large.");
                    return;
                }

                // Kiểm tra bắt buộc nhập tên khách sạn
                if (string.IsNullOrWhiteSpace(TextBoxItem.PethotelName))
                {
                    MessageBox.Show("Vui lòng nhập tên khách sạn!");
                    return;
                }
                if (context.PetHotels.Any(p => p.PethotelName.ToLower() == TextBoxItem.PethotelName.ToLower()))
                {
                    MessageBox.Show("Đã có chuồng này rồi! Vui lòng chọn tên khác.");
                    return;
                }

                PetHotel newHotel = new PetHotel
                {
                    PethotelName = TextBoxItem.PethotelName,
                    Size = TextBoxItem.Size,
                    AvailabilityStatus = TextBoxItem.AvailabilityStatus,
                    Price = TextBoxItem.Price
                };

                context.PetHotels.Add(newHotel);
                context.SaveChanges();
                MessageBox.Show("Thêm khách sạn thành công!");
                LoadPetHotels();
            }
        }

        private void Update(object obj)
        {
            try
            {
                if (SelectedItem == null)
                {
                    MessageBox.Show("Vui lòng chọn khách sạn để cập nhật!");
                    return;
                }

                if (TextBoxItem == null)
                {
                    MessageBox.Show("Lỗi: TextBoxItem chưa được khởi tạo!");
                    return;
                }

                using (var context = new PawfectPrnContext())
                { 

                    var existingHotel = context.PetHotels
                        .FirstOrDefault(p => p.PethotelId == SelectedItem.PethotelId);
                    if (!SizeOptions.Contains(TextBoxItem.Size))
                    {
                        MessageBox.Show("Vui lòng chọn kích thước hợp lệ: Nhỏ, Vừa, Lớn.");
                        return;
                    }


                    if (existingHotel == null)
                    {
                        MessageBox.Show("Không tìm thấy khách sạn để cập nhật!");
                        return;
                    }

                    // Kiểm tra hợp lệ bằng Validator
                    if (!PetHotelValidator.ValidatePetHotel(TextBoxItem, out string errorMessage, context))
                    {
                        MessageBox.Show(errorMessage);
                        return;
                    }
                    if (context.PetHotels.Any(p => p.PethotelName.ToLower() == TextBoxItem.PethotelName.ToLower()
         && p.PethotelId != existingHotel.PethotelId))
                    {
                        MessageBox.Show("Đã có chuồng này rồi! Vui lòng chọn tên khác.");
                        return;
                    }

                    // Kiểm tra trùng lặp tên khách sạn (ngoại trừ chính nó)
                    if (context.PetHotels.Any(p => p.PethotelName.ToLower() == TextBoxItem.PethotelName.ToLower()
                        && p.PethotelId != existingHotel.PethotelId))
                    {
                        MessageBox.Show("Tên khách sạn đã tồn tại! Vui lòng chọn tên khác.");
                        return;
                    }

                    // Cập nhật thông tin khách sạn
                    existingHotel.PethotelName = TextBoxItem.PethotelName;
                    existingHotel.Size = TextBoxItem.Size;
                    existingHotel.AvailabilityStatus = TextBoxItem.AvailabilityStatus;
                    existingHotel.Price = TextBoxItem.Price;

                    context.SaveChanges();
                    MessageBox.Show("Cập nhật khách sạn thành công!");
                    LoadPetHotels();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi hệ thống: {ex.Message}");
            }
        }

        private void Delete(object obj)
        {
            if (SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn khách sạn để xóa!");
                return;
            }

            var result = MessageBox.Show("Bạn có chắc chắn muốn xóa khách sạn này?", "Xác nhận xóa",
                MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                using (var context = new PawfectPrnContext())
                {
                    var hotel = context.PetHotels
                        .FirstOrDefault(p => p.PethotelId == SelectedItem.PethotelId);
                    if (hotel != null)
                    {
                        context.PetHotels.Remove(hotel);
                        context.SaveChanges();
                        MessageBox.Show("Xóa khách sạn thành công!");
                        LoadPetHotels();
                    }
                }
            }
        }

        private void Search(object obj)
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                LoadPetHotels(); // Nếu không có từ khóa, hiển thị toàn bộ danh sách
                return;
            }

            using (var context = new PawfectPrnContext())
            {
                var filteredHotels = context.PetHotels
                    .Where(p => p.PethotelName.ToLower().Contains(SearchText.ToLower()))
                    .ToList();
                petHotels = new ObservableCollection<PetHotel>(filteredHotels);
                OnPropertyChanged(nameof(petHotels));
            }
        }

        // Thuộc tính SelectedItem để lấy thông tin khách sạn đang chọn
        private PetHotel _selectedItem;
        public PetHotel SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));
                if (_selectedItem != null)
                {
                    // Gán giá trị từ khách sạn được chọn cho TextBoxItem (sử dụng cho việc chỉnh sửa)
                    TextBoxItem = new PetHotel
                    {
                        PethotelId = _selectedItem.PethotelId,
                        PethotelName = _selectedItem.PethotelName,
                        Size = _selectedItem.Size,
                        AvailabilityStatus = _selectedItem.AvailabilityStatus,
                        Price = _selectedItem.Price
                    };
                }
                else
                {
                    TextBoxItem = null;
                }
            }
        }

        // Thuộc tính TextBoxItem dùng để binding dữ liệu nhập từ giao diện
        private PetHotel _textBoxItem;
        public PetHotel TextBoxItem
        {
            get { return _textBoxItem; }
            set
            {
                _textBoxItem = value;
                OnPropertyChanged(nameof(TextBoxItem));
            }
        }

        // Thuộc tính SearchText dùng để lưu từ khóa tìm kiếm
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
