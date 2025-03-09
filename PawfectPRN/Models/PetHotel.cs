using System;
using System.Collections.Generic;

namespace PawfectPRN.Models;

public partial class PetHotel
{
    public int PethotelId { get; set; }

    public string PethotelName { get; set; } = null!;

    public string? Size { get; set; }

    public bool? AvailabilityStatus { get; set; }

    public decimal Price { get; set; }

    public virtual ICollection<PethotelBooking> PethotelBookings { get; set; } = new List<PethotelBooking>();
}
