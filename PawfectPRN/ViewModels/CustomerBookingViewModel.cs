using FirstCode.Helper;
using FirstCode.ViewModels;
using Newtonsoft.Json;
using PawfectPRN.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using PawfectPRN.Validation;

public class CustomerBookingViewModel : BaseViewModel
{
    public Account Account { get; set; }
    public ObservableCollection<PethotelBooking> AllBookings { get; set; }
    public ObservableCollection<PethotelBooking> PethotelBookings { get; set; }
    public ObservableCollection<PetHotel> Pethotels { get; set; }

    public ICommand AddCommand { get; set; }
    public ICommand UpdateCommand { get; set; }
    public ICommand DeleteCommand { get; set; }
    public ICommand SearchCommand { get; set; }
    public ICommand ResetCommand { get; set; }

    private PethotelBooking _textboxItem;
    public PethotelBooking TextboxItem
    {
        get => _textboxItem;
        set
        {
            _textboxItem = value;
            OnPropertyChanged(nameof(TextboxItem));
            UpdatePrice();
        }
    }

    private PethotelBooking _selectedItem;
    public PethotelBooking SelectedItem
    {
        get => _selectedItem;
        set
        {
            _selectedItem = value;
            OnPropertyChanged(nameof(SelectedItem));
            if (_selectedItem != null)
            {
                var settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };
                TextboxItem = JsonConvert.DeserializeObject<PethotelBooking>(
                    JsonConvert.SerializeObject(_selectedItem, settings));
                OnPropertyChanged(nameof(TextboxItem));
            }
        }
    }

    private string _searchText;
    public string SearchText
    {
        get => _searchText;
        set { _searchText = value; OnPropertyChanged(nameof(SearchText)); }
    }

    public CustomerBookingViewModel(Account account)
    {
        Account = account;
        AllBookings = new ObservableCollection<PethotelBooking>();
        PethotelBookings = new ObservableCollection<PethotelBooking>();
        Pethotels = new ObservableCollection<PetHotel>();

        TextboxItem = new PethotelBooking
        {
            AccountId = account.AccountId,
            BookingDate = DateTime.Now,
            CheckoutDate = DateTime.Now.AddDays(1)
        };

        AddCommand = new RelayCommand(Add);
        UpdateCommand = new RelayCommand(Update);
        DeleteCommand = new RelayCommand(Delete);
        SearchCommand = new RelayCommand(Search);
        ResetCommand = new RelayCommand(Reset);

        LoadBookings();
        LoadPethotels();
    }

    private void LoadBookings()
    {
        using (var context = new PawfectPrnContext())
        {
            var bookingsFromDb = context.PethotelBookings
                .Include(b => b.Pethotel)
                .Where(b => b.AccountId == Account.AccountId)
                .ToList();
            AllBookings.Clear();
            PethotelBookings.Clear();
            foreach (var booking in bookingsFromDb)
            {
                AllBookings.Add(booking);
                PethotelBookings.Add(booking);
            }
            OnPropertyChanged(nameof(PethotelBookings));
        }
    }

    private void LoadPethotels()
    {
        using (var context = new PawfectPrnContext())
        {
            var pethotelsFromDb = context.PetHotels.ToList();
            Pethotels.Clear();
            foreach (var pethotel in pethotelsFromDb)
            {
                Pethotels.Add(pethotel);
            }
            OnPropertyChanged(nameof(Pethotels));
        }
    }

    private void UpdatePrice()
    {
        if (TextboxItem == null || TextboxItem.PethotelId == 0 || TextboxItem.BookingDate == null || TextboxItem.CheckoutDate == null)
            return;

        using (var context = new PawfectPrnContext())
        {
            var petHotel = context.PetHotels.Find(TextboxItem.PethotelId);
            if (petHotel != null)
            {
                DateTime bookingDate = TextboxItem.BookingDate.Value;
                DateTime checkoutDate = TextboxItem.CheckoutDate.Value;
                if (checkoutDate > bookingDate)
                {
                    TimeSpan stayDuration = checkoutDate - bookingDate;
                    int numberOfDays = stayDuration.Days > 0 ? stayDuration.Days : 1;
                    TextboxItem.Price = petHotel.Price * numberOfDays;
                    OnPropertyChanged(nameof(TextboxItem));
                }
            }
        }
    }

    private void Add(object obj)
    {
        using (var context = new PawfectPrnContext())
        {
            var validator = new CustomerBookingValidator();
            var petHotel = context.PetHotels.Find(TextboxItem.PethotelId);


            if (petHotel == null || petHotel.AvailabilityStatus != true)
            {
                MessageBox.Show("The selected pet hotel is not available.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var (isValid, errorMessage) = validator.ValidateAdd(TextboxItem, petHotel);
            if (!isValid)
            {
                MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DateTime bookingDate = TextboxItem.BookingDate ?? DateTime.Now;
            TimeSpan stayDuration = TextboxItem.CheckoutDate.Value - bookingDate;
            int numberOfDays = stayDuration.Days > 0 ? stayDuration.Days : 1;
            decimal calculatedPrice = petHotel.Price * numberOfDays;

            var newBooking = new PethotelBooking
            {
                AccountId = Account.AccountId,
                PethotelId = TextboxItem.PethotelId,
                BookingDate = bookingDate,
                CheckoutDate = TextboxItem.CheckoutDate.Value,
                ServiceDetails = TextboxItem.ServiceDetails,
                Status = "Pending",
                Price = calculatedPrice
            };

            context.PethotelBookings.Add(newBooking);
            context.SaveChanges();
            context.Entry(newBooking).Reference(b => b.Pethotel).Load();

            PethotelBookings.Add(newBooking);
            AllBookings.Add(newBooking);
            TextboxItem = new PethotelBooking
            {
                AccountId = Account.AccountId,
                BookingDate = DateTime.Now,
                CheckoutDate = DateTime.Now.AddDays(1)
            };
            OnPropertyChanged(nameof(PethotelBookings));
            OnPropertyChanged(nameof(TextboxItem));
            MessageBox.Show("Booking created successfully!", "Success",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    private void Update(object obj)
    {
        if (SelectedItem == null || SelectedItem.Status != "Pending")
        {
            MessageBox.Show("Only bookings with 'Pending' status can be updated.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        using (var context = new PawfectPrnContext())
        {
            var validator = new CustomerBookingValidator();
            var petHotel = context.PetHotels.Find(TextboxItem.PethotelId);

            if (petHotel == null || petHotel.AvailabilityStatus != true)
            {
                MessageBox.Show("The selected pet hotel is not available.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var (isValid, errorMessage) = validator.ValidateUpdate(SelectedItem, TextboxItem, petHotel);
            if (!isValid)
            {
                MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var bookingToUpdate = context.PethotelBookings.Find(SelectedItem.BookingId);
            if (bookingToUpdate != null)
            {
                DateTime bookingDate = TextboxItem.BookingDate ?? DateTime.Now;
                TimeSpan stayDuration = TextboxItem.CheckoutDate.Value - bookingDate;
                int numberOfDays = stayDuration.Days > 0 ? stayDuration.Days : 1;
                decimal calculatedPrice = petHotel.Price * numberOfDays;

                bookingToUpdate.PethotelId = TextboxItem.PethotelId;
                bookingToUpdate.BookingDate = bookingDate;
                bookingToUpdate.CheckoutDate = TextboxItem.CheckoutDate.Value;
                bookingToUpdate.ServiceDetails = TextboxItem.ServiceDetails;
                bookingToUpdate.Status = TextboxItem.Status ?? "Pending";
                bookingToUpdate.Price = calculatedPrice;

                context.SaveChanges();
                context.Entry(bookingToUpdate).Reference(b => b.Pethotel).Load();

                int index = PethotelBookings.IndexOf(SelectedItem);
                if (index != -1)
                {
                    PethotelBookings[index] = bookingToUpdate;
                    AllBookings[index] = bookingToUpdate;
                }
                OnPropertyChanged(nameof(PethotelBookings));
                MessageBox.Show("Booking updated successfully!", "Success",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
    private void Delete(object obj)
    {
        if (SelectedItem == null || SelectedItem.Status != "Pending")
        {
            MessageBox.Show("Only bookings with 'pending' status can be deleted.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        using (var context = new PawfectPrnContext())
        {
            var bookingToDelete = context.PethotelBookings.Find(SelectedItem.BookingId);
            if (bookingToDelete != null)
            {
                context.PethotelBookings.Remove(bookingToDelete);
                context.SaveChanges();
                AllBookings.Remove(SelectedItem);
                PethotelBookings.Remove(SelectedItem);
                SelectedItem = null;
                TextboxItem = new PethotelBooking { AccountId = Account.AccountId, BookingDate = DateTime.Now, CheckoutDate = DateTime.Now.AddDays(1) };
                OnPropertyChanged(nameof(PethotelBookings));
                OnPropertyChanged(nameof(TextboxItem));
            }
        }
    }

    private void Search(object obj)
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            PethotelBookings.Clear();
            foreach (var booking in AllBookings)
            {
                PethotelBookings.Add(booking);
            }
        }
        else
        {
            var filteredBookings = AllBookings.Where(b =>
                (b.ServiceDetails?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (b.Pethotel?.PethotelName?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false)
            ).ToList();

            PethotelBookings.Clear();
            foreach (var booking in filteredBookings)
            {
                PethotelBookings.Add(booking);
            }
        }
        OnPropertyChanged(nameof(PethotelBookings));
    }

    private void Reset(object obj)
    {
        SelectedItem = null;
        TextboxItem = new PethotelBooking { AccountId = Account.AccountId, BookingDate = DateTime.Now, CheckoutDate = DateTime.Now.AddDays(1) };
        SearchText = string.Empty;
        Search(null);
        OnPropertyChanged(nameof(TextboxItem));
    }
}