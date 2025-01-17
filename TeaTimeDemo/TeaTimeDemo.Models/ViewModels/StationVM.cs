﻿﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeaTimeDemo.Models.ViewModels
{
    public class StationVM
    {
        public Station Station { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> StationList { get; set; }
    }
}