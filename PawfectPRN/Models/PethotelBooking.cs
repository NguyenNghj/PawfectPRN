using System;
using System.Collections.Generic;

namespace PawfectPRN.Models;

public partial class PethotelBooking
{
    public int BookingId { get; set; }

    public int AccountId { get; set; }

    public int PethotelId { get; set; }

    public DateTime? BookingDate { get; set; }

    public DateTime? CheckoutDate { get; set; }

    public string? ServiceDetails { get; set; }

    public string Status { get; set; } = null!;

    public decimal? Price { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual PetHotel Pethotel { get; set; } = null!;
}
