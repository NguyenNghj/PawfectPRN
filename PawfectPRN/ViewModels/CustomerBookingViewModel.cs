using FirstCode.Helper;
using FirstCode.ViewModels;
using Newtonsoft.Json;
using PawfectPRN.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows;
using Microsoft.EntityFrameworkCore;

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

    public CustomerBookingViewModel(Account account)
    {
        Account = account;
        AddCommand = new RelayCommand(Add);
        UpdateCommand = new RelayCommand(Update);
        DeleteCommand = new RelayCommand(Delete);
        SearchCommand = new RelayCommand(Search);
        ResetCommand = new RelayCommand(Reset);
        TextboxItem = new PethotelBooking();
    }

    private void LoadBookings()
    {
        using (var context = new PawfectPrnContext())
        {
            var bookingsFromDb = context.PethotelBookings
                                         .Include(b => b.Pethotel)  // Ensure Pethotel is loaded
                                         .Where(b => b.AccountId == Account.AccountId)
                                         .ToList();
            AllBookings = new ObservableCollection<PethotelBooking>(bookingsFromDb);
            PethotelBookings = new ObservableCollection<PethotelBooking>(AllBookings);
            OnPropertyChanged(nameof(PethotelBookings));
        }
    }


    private PethotelBooking _textboxItem;
    public PethotelBooking TextboxItem
    {
        get { return _textboxItem; }
        set { _textboxItem = value; OnPropertyChanged(nameof(TextboxItem)); }
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
                TextboxItem = JsonConvert.DeserializeObject<PethotelBooking>(JsonConvert.SerializeObject(_selectedItem));
                OnPropertyChanged(nameof(TextboxItem));
            }
        }
    }

    //private void LoadBookings()
    //{
    //    using (var context = new PawfectPrnContext())
    //    {
    //        var bookingsFromDb = context.PethotelBookings
    //                                     .Where(b => b.AccountId == Account.AccountId)
    //                                     .ToList();
    //        AllBookings = new ObservableCollection<PethotelBooking>(bookingsFromDb);
    //        PethotelBookings = new ObservableCollection<PethotelBooking>(AllBookings);
    //        OnPropertyChanged(nameof(PethotelBookings));
    //    }
    //}

    private void Add(object obj)
    {
        if (TextboxItem.PethotelId == 0 || TextboxItem.CheckoutDate == default)
        {
            MessageBox.Show("Please enter complete booking information!", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        using (var context = new PawfectPrnContext())
        {
            var newBooking = new PethotelBooking
            {
                AccountId = TextboxItem.AccountId,
                PethotelId = TextboxItem.PethotelId,
                BookingDate = DateTime.Now,
                CheckoutDate = TextboxItem.CheckoutDate,
                ServiceDetails = TextboxItem.ServiceDetails,
                Status = "Pending"
            };

            context.PethotelBookings.Add(newBooking);
            context.SaveChanges();
            context.Entry(newBooking).Reload();

            PethotelBookings.Add(newBooking);
            AllBookings.Add(newBooking);
            OnPropertyChanged(nameof(PethotelBookings));
        }
    }

    private void Update(object obj)
    {
        if (SelectedItem == null) return;
        using (var context = new PawfectPrnContext())
        {
            var bookingToUpdate = context.PethotelBookings.Find(SelectedItem.BookingId);
            if (bookingToUpdate != null)
            {
                bookingToUpdate.CheckoutDate = TextboxItem.CheckoutDate;
                bookingToUpdate.ServiceDetails = TextboxItem.ServiceDetails;
                bookingToUpdate.Status = TextboxItem.Status;
                context.SaveChanges();
                var index = PethotelBookings.IndexOf(SelectedItem);
                if (index != -1)
                {
                    PethotelBookings[index] = bookingToUpdate;
                    AllBookings[index] = bookingToUpdate;
                }
                OnPropertyChanged(nameof(PethotelBookings));
            }
        }
    }

    private void Delete(object obj)
    {
        if (SelectedItem == null) return;
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
                TextboxItem = new PethotelBooking();
                OnPropertyChanged(nameof(PethotelBookings));
            }
        }
    }

    private string _searchText;
    public string SearchText
    {
        get => _searchText;
        set { _searchText = value; OnPropertyChanged(nameof(SearchText)); }
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
            var filteredBookings = AllBookings.Where(b => b.ServiceDetails.Contains(SearchText, StringComparison.OrdinalIgnoreCase)).ToList();
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
        TextboxItem = new PethotelBooking();
        SearchText = string.Empty;
        Search(null);
    }
}
