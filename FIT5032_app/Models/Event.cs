namespace FIT5032_app.Models
{
    using FIT5032_app.Utils;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    // The event model of the application
    public partial class Event
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Event()
        {
            Bookings = new HashSet<Booking>();
        }

        public int EventId { get; set; }

        [Required]
        [Display(Name = "Event Name")]
        [StringLength(50)]
        public string EventName { get; set; }
        [Required]
        [Display(Name = "Start Date & Time")]
        [DataType(DataType.DateTime)]        
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime StartDateTime { get; set; }
        //Restrict length of an event.
        [Required]
        [Range(1,4,ErrorMessage = "Please enter a number between 1 and 4")]
        [Display(Name = "Length(hours)")]       
        public int EventLength { get; set; }

        [Column(TypeName = "text")]
        [Required]
        [StringLength(1000)]
        public string Description { get; set; }

        [Display(Name = "Available to book?")]
        public bool Available { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Booking> Bookings { get; set; }

        //To check whether the input date time is greater than the current date time and less than the date one year later
        //And whether to start time is in the range from 9am to 7pm.
        public bool IsDateTimeValid()
        {
            var endDate = DateTime.Now.AddYears(1);
            TimeSpan minTime = new TimeSpan(9, 0, 0);
            TimeSpan maxTime = new TimeSpan(19, 0, 0);
            if ((DateTime.Now.CompareTo(StartDateTime) <= 0 && endDate.CompareTo(StartDateTime) >= 0) && (StartDateTime.TimeOfDay > minTime && StartDateTime.TimeOfDay < maxTime))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
