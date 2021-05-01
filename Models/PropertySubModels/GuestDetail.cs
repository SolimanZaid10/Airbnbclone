﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Airbnb.Models.PropertySubModels
{
    public class GuestDetail
    {
        public int Id { get; set; }

        [Required]
        [StringLength(500, MinimumLength = 2)]
        public string Name { get; set; }

        public List<PropertyGuestDetail> Properties { get; set; }
    }
}