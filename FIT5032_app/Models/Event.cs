namespace FIT5032_app.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

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
        //[DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime StartDateTime { get; set; }
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
    }
}
