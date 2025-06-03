using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BillPay.Models.ViewModels
{
    public class ProfileIndexModel
    {
        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Required]
        [Display(Name = "Address")]
        public string? Address { get; set; }
        [Required]
        [Display(Name = "Gender")]
        public char Gender { get; set; }
        [Required]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
        [Display(Name = "Image")]
        [ValidateNever]
        public string ImageURL { get; set; }
        [ValidateNever]
        [Display(Name = "Name in Esewa")]
        public string? EsewaName { get; set; }
        [ValidateNever]
        [Display(Name = "Esewa Phone Number")]
        public string? EsewaPhone { get; set; }
    }
}
