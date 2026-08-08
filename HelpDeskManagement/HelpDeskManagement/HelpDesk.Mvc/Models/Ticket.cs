using System;
using System.ComponentModel.DataAnnotations;

namespace HelpDesk.Mvc.Models
{
    public class Ticket
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [Display(Name = "Title")]
        [StringLength(200)]
        public string Title { get; set; }

        [Display(Name = "Description")]
        [StringLength(1000)]
        public string Description { get; set; }

        /// <summary>Valid values: Low, Medium, High</summary>
        [Required(ErrorMessage = "Priority is required.")]
        [Display(Name = "Priority")]
        public string Priority { get; set; }

        /// <summary>Valid values: Open, In Progress, Closed</summary>
        [Display(Name = "Status")]
        public string Status { get; set; }

        [Required(ErrorMessage = "Raised By is required.")]
        [Display(Name = "Raised By")]
        [StringLength(100)]
        public string RaisedBy { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
    }
}
