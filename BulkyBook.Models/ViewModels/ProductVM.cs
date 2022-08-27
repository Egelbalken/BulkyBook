using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Models.ViewModels
{
    /// <summary>
    /// Class to collect all list selectors objects 
    /// </summary>
    public class ProductVM
    {
        public Product? Product { get;set;}

        // Dropdown list
        // Using a projection
        [ValidateNever]
        public IEnumerable<SelectListItem>? CategoryList { get; set; }
        [ValidateNever]
        // Using a projection
        public IEnumerable<SelectListItem>? CoverTypeList { get; set; }
    }
}
