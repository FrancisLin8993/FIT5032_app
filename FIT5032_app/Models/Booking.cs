namespace FIT5032_app.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Booking
    {
        public int BookingId { get; set; }

        public string UserId { get; set; }

        public int? EventId { get; set; }

        public virtual Event Event { get; set; }
    }
}
