using FirstCode.Helper;
using FirstCode.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;

namespace PawfectPRN.ViewModels
{
    internal class CustomerBookingViewModel : BaseViewModel
    {


        //    public ObservableCollection<FlightBooking> allFlightBookings { get; set; }
        //    public ObservableCollection<FlightBooking> FlightBookings { get; set; }

        //    public ICommand AddCommand { get; set; }
        //    public ICommand UpdateCommand { get; set; }
        //    public ICommand DeleteCommand { get; set; }
        //    public ICommand SearchCommand { get; set; }
        //    public ICommand ExportCommand { get; set; }

        //    public FlightBookingViewModel()
        //    {
        //        Load();
        //        AddCommand = new RelayCommand(Add);
        //        UpdateCommand = new RelayCommand(Update);
        //        DeleteCommand = new RelayCommand(Delete);
        //        SearchCommand = new RelayCommand(Search);
        //        ExportCommand = new RelayCommand(Export);
        //        textboxitem = new FlightBooking();
        //    }

        //    private FlightBooking _textboxitem;

        //    public FlightBooking textboxitem
        //    {
        //        get { return _textboxitem; }
        //        set
        //        {
        //            _textboxitem = value;
        //            OnPropertyChanged(nameof(textboxitem));
        //        }
        //    }

        //    private FlightBooking _selectitem;

        //    public FlightBooking selectitem
        //    {
        //        get { return _selectitem; }
        //        set
        //        {
        //            _selectitem = value;
        //            OnPropertyChanged(nameof(selectitem));
        //            if (_selectitem != null)
        //            {
        //                _textboxitem = JsonConvert.DeserializeObject<FlightBooking>(JsonConvert.SerializeObject(_selectitem));
        //                OnPropertyChanged(nameof(textboxitem));
        //            }
        //        }
        //    }

        //    private void Load()
        //    {
        //        using (var context = new Pe180897NguyenTriNghiContext())
        //        {
        //            var FlightBookingsFromDb = context.FlightBookings.ToList();
        //            allFlightBookings = new ObservableCollection<FlightBooking>(FlightBookingsFromDb);
        //            FlightBookings = new ObservableCollection<FlightBooking>(allFlightBookings);
        //            OnPropertyChanged(nameof(FlightBookings));
        //        }
        //    }

        //    private void Add(object obj)
        //    {
        //        if (string.IsNullOrWhiteSpace(textboxitem.PassengerName) ||
        //           string.IsNullOrWhiteSpace(textboxitem.FlightNumber) || string.IsNullOrWhiteSpace(textboxitem.Departure)
        //         || string.IsNullOrWhiteSpace(textboxitem.Destination) || string.IsNullOrWhiteSpace(textboxitem.DepartureDate)
        //         || string.IsNullOrWhiteSpace(textboxitem.DepartureTime) || string.IsNullOrWhiteSpace(textboxitem.SeatClass)
        //         || string.IsNullOrWhiteSpace(textboxitem.Price.ToString()))
        //        {
        //            MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
        //            return;
        //        }
        //        using (var context = new Pe180897NguyenTriNghiContext())
        //        {
        //            var newFlightBooking = new FlightBooking
        //            {
        //                PassengerName = textboxitem.PassengerName,
        //                FlightNumber = textboxitem.FlightNumber,
        //                Departure = textboxitem.Departure,
        //                Destination = textboxitem.Destination,
        //                DepartureDate = textboxitem.DepartureDate,
        //                DepartureTime = textboxitem.DepartureTime,
        //                SeatClass = textboxitem.SeatClass,
        //                Price = textboxitem.Price
        //            };

        //            context.FlightBookings.Add(newFlightBooking);
        //            context.SaveChanges();

        //            context.Entry(newFlightBooking).Reload();

        //            FlightBookings.Add(newFlightBooking); // Cập nhật UI
        //            allFlightBookings.Add(newFlightBooking);
        //            OnPropertyChanged(nameof(FlightBookings));
        //        }
        //    }

        //    private void Update(object obj)
        //    {
        //        if (selectitem == null) return;

        //        using (var context = new Pe180897NguyenTriNghiContext())
        //        {
        //            var FlightBookingToUpdate = context.FlightBookings.Find(selectitem.Id);
        //            if (FlightBookingToUpdate != null)
        //            {
        //                // Cập nhật thông tin từ textboxitem
        //                FlightBookingToUpdate.PassengerName = textboxitem.PassengerName;
        //                FlightBookingToUpdate.FlightNumber = textboxitem.FlightNumber;
        //                FlightBookingToUpdate.Departure = textboxitem.Departure;
        //                FlightBookingToUpdate.Destination = textboxitem.Destination;
        //                FlightBookingToUpdate.DepartureDate = textboxitem.DepartureDate;
        //                FlightBookingToUpdate.DepartureTime = textboxitem.DepartureTime;
        //                FlightBookingToUpdate.SeatClass = textboxitem.SeatClass;
        //                FlightBookingToUpdate.Price = textboxitem.Price;
        //                context.SaveChanges(); // Lưu thay đổi vào database

        //                // Cập nhật UI
        //                var index = FlightBookings.IndexOf(selectitem);
        //                if (index != -1)
        //                {
        //                    FlightBookings[index] = FlightBookingToUpdate;
        //                    allFlightBookings[index] = FlightBookingToUpdate;
        //                }
        //            }
        //        }
        //    }


        //    private void Delete(object obj)
        //    {
        //        if (selectitem == null) return;

        //        using (var context = new Pe180897NguyenTriNghiContext())
        //        {
        //            var FlightBookingToDelete = context.FlightBookings.Find(selectitem.Id);
        //            if (FlightBookingToDelete != null)
        //            {
        //                context.FlightBookings.Remove(FlightBookingToDelete);
        //                context.SaveChanges();

        //                // Xóa khỏi danh sách gốc và danh sách hiển thị
        //                allFlightBookings.Remove(selectitem);
        //                FlightBookings.Remove(selectitem);

        //                // Đặt lại selectitem
        //                selectitem = null;
        //                OnPropertyChanged(nameof(selectitem));

        //                textboxitem = new FlightBooking();
        //                // Cập nhật UI
        //                OnPropertyChanged(nameof(FlightBookings));
        //            }
        //        }
        //    }

        //    private string _searchText;
        //    public string SearchText
        //    {
        //        get => _searchText;
        //        set
        //        {
        //            _searchText = value;
        //            OnPropertyChanged(nameof(SearchText));
        //        }
        //    }


        //    private void Search(object obj)
        //    {
        //        if (string.IsNullOrWhiteSpace(SearchText))
        //        {
        //            FlightBookings.Clear();
        //            foreach (var FlightBooking in allFlightBookings)
        //            {
        //                FlightBookings.Add(FlightBooking);
        //            }
        //        }
        //        else
        //        {
        //            var filteredFlightBookings = allFlightBookings
        //                .Where(s => s.PassengerName.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
        //                .ToList();

        //            FlightBookings.Clear();
        //            foreach (var FlightBooking in filteredFlightBookings)
        //            {
        //                FlightBookings.Add(FlightBooking);
        //            }
        //        }

        //        OnPropertyChanged(nameof(FlightBookings));
        //    }

        //    private void Export(object obj)
        //    {
        //        string filename = "flightbooking.json";
        //        StoreToFile(filename);
        //    }

        //    public void StoreToFile(string filename)
        //    {
        //        try
        //        {
        //            string jsonData = System.Text.Json.JsonSerializer.Serialize(FlightBookings, new JsonSerializerOptions { WriteIndented = true });
        //            File.WriteAllText(filename, jsonData);
        //        }
        //        catch (Exception ex)
        //        {
        //            throw new Exception("Error writing to file: " + ex.Message);
        //        }
        //    }

        //}

    }
}
