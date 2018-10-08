using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

// A view model includes the user and the booking objects, in order to show the booking and user information in a web page.
namespace FIT5032_app.Models
{
    public class BookingEmailViewModel
    {
       
        public ApplicationUser User { get; set; }

        public Booking Booking { get; set; }

    }
}