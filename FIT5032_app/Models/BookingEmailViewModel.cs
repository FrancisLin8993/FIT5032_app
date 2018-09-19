using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FIT5032_app.Models
{
    public class BookingEmailViewModel
    {
       
        public ApplicationUser User { get; set; }

        public Booking Booking { get; set; }

    }
}