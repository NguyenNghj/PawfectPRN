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
    public class StaffBookingViewModel : BaseViewModel
    {
        public ObservableCollection<PethotelBooking> bookings { get; set; }
        public ObservableCollection<PetHotel> petHotels { get; set; }
        public ObservableCollection<Account> accounts { get; set; }
        public ObservableCollection<string> statuses { get; set; }

        public ICommand AddCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        public ICommand UpdateCommand { get; set; }
        public ICommand ResetCommand { get; set; }

        public StaffBookingViewModel()
        {
            LoadPetHotels();
            LoadAccounts();
            LoadBookings();
            statuses = new ObservableCollection<string> { "Pending", "Confirmed", "Checked-in", "Checked-out", "Cancelled" };
            OnPropertyChanged(nameof(statuses));
            TextBoxItem = new PethotelBooking();
            AddCommand = new RelayCommand(Add);
            UpdateCommand = new RelayCommand(Update);
            SearchCommand = new RelayCommand(Search);
            DeleteCommand = new RelayCommand(Delete);
            ResetCommand = new RelayCommand(Reset);
        }

        private void Add(object obj)
        {
            if (TextBoxItem == null)
            {
                TextBoxItem = new PethotelBooking();
            }

            // Validation
            using (var context = new PawfectPrnContext())
            {
                if (TextBoxItem.AccountId == 0 || TextBoxItem.PethotelId == 0 || !BookingDate.HasValue || !BookingTime.HasValue || !CheckoutDate.HasValue || !CheckoutTime.HasValue)
                {
                    MessageBox.Show("Vui lòng điền đầy đủ thông tin: Khách hàng, Khách sạn, Ngày đặt, Thời gian đặt, Ngày trả, Thời gian trả!");
                    return;
                }

                // Kết hợp ngày và giờ
                var bookingDateTime = BookingDate.Value.Date + BookingTime.Value.TimeOfDay;
                var checkoutDateTime = CheckoutDate.Value.Date + CheckoutTime.Value.TimeOfDay;

                PethotelBooking newBooking = new PethotelBooking
                {
                    AccountId = TextBoxItem.AccountId,
                    PethotelId = TextBoxItem.PethotelId,
                    BookingDate = bookingDateTime,
                    CheckoutDate = checkoutDateTime,
                    ServiceDetails = TextBoxItem.ServiceDetails,
                    Status = TextBoxItem.Status ?? "Pending"
                };

                context.PethotelBookings.Add(newBooking);
                context.SaveChanges();
                MessageBox.Show("Thêm đặt phòng thành công!");
                LoadBookings();
            }
        }

        private void Update(object obj)
        {
            try
            {
                if (SelectedItem == null)
                {
                    MessageBox.Show("Vui lòng chọn đặt phòng để cập nhật!");
                    return;
                }

                if (!BookingDate.HasValue || !BookingTime.HasValue || !CheckoutDate.HasValue || !CheckoutTime.HasValue)
                {
                    MessageBox.Show("Vui lòng điền đầy đủ thông tin ngày đặt và ngày trả!");
                    return;
                }

                var bookingDateTime = BookingDate.Value.Date + BookingTime.Value.TimeOfDay;
                var checkoutDateTime = CheckoutDate.Value.Date + CheckoutTime.Value.TimeOfDay;

                using (var context = new PawfectPrnContext())
                {
                    var existingBooking = context.PethotelBookings.FirstOrDefault(b => b.BookingId == SelectedItem.BookingId);
                    if (existingBooking != null)
                    {
                        existingBooking.AccountId = TextBoxItem.AccountId;
                        existingBooking.PethotelId = TextBoxItem.PethotelId;
                        existingBooking.BookingDate = bookingDateTime;
                        existingBooking.CheckoutDate = checkoutDateTime;
                        existingBooking.ServiceDetails = TextBoxItem.ServiceDetails;
                        existingBooking.Status = TextBoxItem.Status;

                        context.SaveChanges();
                        MessageBox.Show("Cập nhật đặt phòng thành công!");
                        LoadBookings();
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy đặt phòng để cập nhật!");
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"Lỗi: {e.Message}");
            }
        }

        private void Delete(object obj)
        {
            if (SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn đặt phòng để xóa!");
                return;
            }

            var result = MessageBox.Show("Bạn có chắc chắn muốn xóa đặt phòng này?", "Xác nhận xóa",
                MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                using (var context = new PawfectPrnContext())
                {
                    var booking = context.PethotelBookings.FirstOrDefault(b => b.BookingId == SelectedItem.BookingId);
                    if (booking != null)
                    {
                        context.PethotelBookings.Remove(booking);
                        context.SaveChanges();
                        MessageBox.Show("Xóa đặt phòng thành công!");
                        LoadBookings();
                    }
                }
            }
        }

        private void Reset(object obj)
        {
            TextBoxItem = new PethotelBooking();
            BookingDate = null;
            BookingTime = null;
            CheckoutDate = null;
            CheckoutTime = null;
            SearchText = null;
            SelectedAccount = null;
            SelectedPetHotel = null;
            OnPropertyChanged(nameof(TextBoxItem));
            OnPropertyChanged(nameof(BookingDate));
            OnPropertyChanged(nameof(BookingTime));
            OnPropertyChanged(nameof(CheckoutDate));
            OnPropertyChanged(nameof(CheckoutTime));
            OnPropertyChanged(nameof(SelectedAccount));
            OnPropertyChanged(nameof(SelectedPetHotel));
            OnPropertyChanged(nameof(SearchText));
            LoadBookings();
        }

        private void Search(object obj)
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                LoadBookings();
                return;
            }

            using (var context = new PawfectPrnContext())
            {
                var filteredBookings = context.PethotelBookings
                    .Include(b => b.Account)
                    .Include(b => b.Pethotel)
                    .Where(b => b.Account.FullName.ToLower().Contains(SearchText.ToLower()) ||
                                b.Pethotel.PethotelName.ToLower().Contains(SearchText.ToLower()))
                    .ToList();
                bookings = new ObservableCollection<PethotelBooking>(filteredBookings);
                OnPropertyChanged(nameof(bookings));
            }
        }

        private void LoadBookings()
        {
            using (var context = new PawfectPrnContext())
            {
                var bookingList = context.PethotelBookings
                    .Include(b => b.Account)
                    .Include(b => b.Pethotel)
                    .ToList();
                bookings = new ObservableCollection<PethotelBooking>(bookingList);
                OnPropertyChanged(nameof(bookings));
            }
        }

        private void LoadPetHotels()
        {
            using (var context = new PawfectPrnContext())
            {
                petHotels = new ObservableCollection<PetHotel>(context.PetHotels.ToList());
                OnPropertyChanged(nameof(petHotels));
            }
        }

        private void LoadAccounts()
        {
            using (var context = new PawfectPrnContext())
            {
                accounts = new ObservableCollection<Account>(context.Accounts.ToList());
                OnPropertyChanged(nameof(accounts));
            }
        }

        private Account _selectedAccount;
        public Account SelectedAccount
        {
            get { return _selectedAccount; }
            set
            {
                _selectedAccount = value;
                OnPropertyChanged(nameof(SelectedAccount));
                if (_selectedAccount != null && TextBoxItem != null)
                {
                    TextBoxItem.AccountId = _selectedAccount.AccountId;
                }
            }
        }

        private PetHotel _selectedPetHotel;
        public PetHotel SelectedPetHotel
        {
            get { return _selectedPetHotel; }
            set
            {
                _selectedPetHotel = value;
                OnPropertyChanged(nameof(SelectedPetHotel));
                if (_selectedPetHotel != null && TextBoxItem != null)
                {
                    TextBoxItem.PethotelId = _selectedPetHotel.PethotelId;
                }
            }
        }

        private PethotelBooking _textBoxItem;
        public PethotelBooking TextBoxItem
        {
            get { return _textBoxItem; }
            set
            {
                _textBoxItem = value;
                if (_textBoxItem != null)
                {
                    SelectedAccount = accounts?.FirstOrDefault(a => a.AccountId == _textBoxItem.AccountId);
                    SelectedPetHotel = petHotels?.FirstOrDefault(p => p.PethotelId == _textBoxItem.PethotelId);
                    BookingDate = _textBoxItem.BookingDate?.Date;
                    BookingTime = _textBoxItem.BookingDate;
                    CheckoutDate = _textBoxItem.CheckoutDate?.Date;
                    CheckoutTime = _textBoxItem.CheckoutDate;
                }
                OnPropertyChanged(nameof(TextBoxItem));
                OnPropertyChanged(nameof(BookingDate));
                OnPropertyChanged(nameof(BookingTime));
                OnPropertyChanged(nameof(CheckoutDate));
                OnPropertyChanged(nameof(CheckoutTime));
            }
        }

        private PethotelBooking _selectedItem;
        public PethotelBooking SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));
                if (_selectedItem != null)
                {
                    TextBoxItem = new PethotelBooking
                    {
                        BookingId = _selectedItem.BookingId,
                        AccountId = _selectedItem.AccountId,
                        PethotelId = _selectedItem.PethotelId,
                        BookingDate = _selectedItem.BookingDate,
                        CheckoutDate = _selectedItem.CheckoutDate,
                        ServiceDetails = _selectedItem.ServiceDetails,
                        Status = _selectedItem.Status,
                        Account = _selectedItem.Account,
                        Pethotel = _selectedItem.Pethotel
                    };
                }
                else
                {
                    TextBoxItem = new PethotelBooking();
                }
            }
        }

        private DateTime? _bookingDate;
        public DateTime? BookingDate
        {
            get { return _bookingDate; }
            set
            {
                _bookingDate = value;
                OnPropertyChanged(nameof(BookingDate));
            }
        }

        private DateTime? _bookingTime;
        public DateTime? BookingTime
        {
            get { return _bookingTime; }
            set
            {
                _bookingTime = value;
                OnPropertyChanged(nameof(BookingTime));
            }
        }

        private DateTime? _checkoutDate;
        public DateTime? CheckoutDate
        {
            get { return _checkoutDate; }
            set
            {
                _checkoutDate = value;
                OnPropertyChanged(nameof(CheckoutDate));
            }
        }

        private DateTime? _checkoutTime;
        public DateTime? CheckoutTime
        {
            get { return _checkoutTime; }
            set
            {
                _checkoutTime = value;
                OnPropertyChanged(nameof(CheckoutTime));
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