using System;
using System.ComponentModel.DataAnnotations;

namespace HelpDesk.Api.Models
{
    public class Ticket
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        /// <summary>Valid values: Low, Medium, High</summary>
        [Required]
        [StringLength(20)]
        public string Priority { get; set; }

        /// <summary>Valid values: Open, In Progress, Closed</summary>
        [Required]
        [StringLength(20)]
        public string Status { get; set; }

        [Required]
        [StringLength(100)]
        public string RaisedBy { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
