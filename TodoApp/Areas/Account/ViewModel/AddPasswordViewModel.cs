﻿using System.ComponentModel.DataAnnotations;

namespace TodoApp.ViewModel
{
    public class AddPasswordViewModel
    {
        [Required]
        public string NewPassword { get; set; }
        [Required]
        [Compare("NewPassword", ErrorMessage = "password do not match")]
        public string ConfirmPassword { get; set; }
    }
}
