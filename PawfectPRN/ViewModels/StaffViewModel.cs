using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FirstCode.Helper;
using FirstCode.ViewModels;
using PawfectPRN.Models;
using System.Windows.Input;
using System.Windows;
using PawfectPRN.Validation;

namespace PawfectPRN.ViewModels
{
    internal class StaffViewModel : BaseViewModel

    {
        // Danh sách staff hiển thị trên giao diện
        public ObservableCollection<Account> Staffs { get; set; }
        private List<string> _genderOptions;
        public List<string> GenderOptions
        {
            get => _genderOptions;
            set
            {
                _genderOptions = value;
                OnPropertyChanged(nameof(GenderOptions));
            }
        }
        // Các command cho thao tác thêm, sửa, xóa và tìm kiếm
        public ICommand AddCommand { get; set; }
        public ICommand UpdateCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        public ICommand NewCommand { get; set; }

        public StaffViewModel()
        {

            // Khởi tạo đối tượng TextBoxItem để sẵn sàng nhập liệu mới
            TextBoxItem = new Account();
            GenderOptions = new List<string> { "male", "female", "other" };
            LoadStaffs();

            AddCommand = new RelayCommand(Add);
            UpdateCommand = new RelayCommand(Update);
            DeleteCommand = new RelayCommand(Delete);
            SearchCommand = new RelayCommand(Search);
            NewCommand = new RelayCommand(NewItem);
        }

        // Tải danh sách tài khoản có RoleName là "staff"
        private void LoadStaffs()
        {
            using (var context = new PawfectPrnContext())
            {
                var staffList = context.Accounts
                    .Where(a => a.RoleName.ToLower() == "staff")
                    .ToList();
                Staffs = new ObservableCollection<Account>(staffList);
                OnPropertyChanged(nameof(Staffs));
            }
        }

        // Tạo mới đối tượng nhập liệu (TextBoxItem)
        private void NewItem(object obj)
        {
            SelectedItem = null;
            TextBoxItem = new Account();
            OnPropertyChanged(nameof(TextBoxItem));
        }

        // Thêm tài khoản staff mới
        private void Add(object obj)
        {
            if (TextBoxItem == null)
            {
                TextBoxItem = new Account();
            }
            TextBoxItem.RoleName = "staff";

            using (var context = new PawfectPrnContext())
            {
                if (!StaffValidator.ValidateStaff(TextBoxItem, out string errorMessage, context))
                {
                    MessageBox.Show(errorMessage);
                    return;
                }

                context.Accounts.Add(new Account
                {
                    FullName = TextBoxItem.FullName,
                    Email = TextBoxItem.Email,
                    PhoneNumber = TextBoxItem.PhoneNumber,
                    Address = TextBoxItem.Address,
                    Gender = TextBoxItem.Gender,
                    RoleName = "staff",
                    Password = TextBoxItem.Password
                });

                context.SaveChanges();
                MessageBox.Show("Thêm staff thành công!");
                LoadStaffs();
            }
            TextBoxItem = new Account();
            OnPropertyChanged(nameof(TextBoxItem));
        }



        // Cập nhật thông tin staff đã chọn
        private void Update(object obj)
        {
            if (SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn staff để cập nhật!");
                return;
            }

            using (var context = new PawfectPrnContext())
            {
                if (!StaffValidator.ValidateStaff(TextBoxItem, out string errorMessage, context, true))
                {
                    MessageBox.Show(errorMessage);
                    return;
                }

                var existingStaff = context.Accounts.FirstOrDefault(a => a.AccountId == SelectedItem.AccountId);
                if (existingStaff != null)
                {
                    existingStaff.FullName = TextBoxItem.FullName;
                    existingStaff.Email = TextBoxItem.Email;
                    existingStaff.PhoneNumber = TextBoxItem.PhoneNumber;
                    existingStaff.Address = TextBoxItem.Address;
                    existingStaff.Gender = TextBoxItem.Gender;
                    existingStaff.RoleName = "staff";
                    existingStaff.Password = TextBoxItem.Password;

                    context.SaveChanges();
                    MessageBox.Show("Cập nhật staff thành công!");
                    LoadStaffs();
                }
                else
                {
                    MessageBox.Show("Không tìm thấy staff để cập nhật!");
                }
            }
        }

        // Xóa tài khoản staff đã chọn
        private void Delete(object obj)
        {
            if (SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn staff để xóa!");
                return;
            }

            var result = MessageBox.Show("Bạn có chắc chắn muốn xóa staff này?", "Xác nhận xóa",
                MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                using (var context = new PawfectPrnContext())
                {
                    var staff = context.Accounts
                        .FirstOrDefault(a => a.AccountId == SelectedItem.AccountId);
                    if (staff != null)
                    {
                        context.Accounts.Remove(staff);
                        context.SaveChanges();
                        MessageBox.Show("Xóa staff thành công!");
                        LoadStaffs();
                    }
                }
            }
        }

        // Tìm kiếm staff theo tên
        private void Search(object obj)
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                LoadStaffs(); // Nếu không có từ khóa, hiển thị toàn bộ danh sách
                return;
            }

            using (var context = new PawfectPrnContext())
            {
                var filteredStaffs = context.Accounts
                    .Where(a => a.RoleName.ToLower() == "staff" &&
                                a.FullName.ToLower().Contains(SearchText.ToLower()))
                    .ToList();
                Staffs = new ObservableCollection<Account>(filteredStaffs);
                OnPropertyChanged(nameof(Staffs));
            }
        }

        // Thuộc tính SelectedItem để lấy thông tin staff đang chọn
        private Account _selectedItem;
        public Account SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));
                if (_selectedItem != null)
                {
                    // Khi chọn staff, hiển thị thông tin lên các ô nhập liệu để chỉnh sửa
                    TextBoxItem = new Account
                    {
                        AccountId = _selectedItem.AccountId,
                        FullName = _selectedItem.FullName,
                        Email = _selectedItem.Email,
                        PhoneNumber = _selectedItem.PhoneNumber,
                        Address = _selectedItem.Address,
                        Gender = _selectedItem.Gender,
                        RoleName = _selectedItem.RoleName,
                        Password = _selectedItem.Password
                    };
                }
                else
                {
                    // Nếu không có lựa chọn, khởi tạo mới đối tượng nhập liệu
                    TextBoxItem = new Account();
                }
                OnPropertyChanged(nameof(TextBoxItem));
            }
        }

        // Thuộc tính TextBoxItem dùng để binding dữ liệu nhập từ giao diện
        private Account _textBoxItem;
        public Account TextBoxItem
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
