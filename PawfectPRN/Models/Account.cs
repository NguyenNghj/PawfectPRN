using System;
using System.Collections.Generic;

namespace PawfectPRN.Models;

public partial class Account
{
    public int AccountId { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public string? Address { get; set; }

    public string? RoleName { get; set; }

    public string Password { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<PethotelBooking> PethotelBookings { get; set; } = new List<PethotelBooking>();
}
