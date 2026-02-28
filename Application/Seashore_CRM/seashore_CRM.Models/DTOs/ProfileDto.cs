using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace seashore_CRM.Models.DTOs
{
    public class ProfileViewDto
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string? FullName { get; set; }
        public string? Contact { get; set; }
        public string? Region { get; set; }
        public string RoleName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class ProfileUpdateDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [Remote(action: "VerifyEmail", controller: "Profile", AdditionalFields = "Id", ErrorMessage = "Email already in use")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Full Name is required")]
        [Remote(action: "VerifyFullName", controller: "Profile", AdditionalFields = "Id", ErrorMessage = "Full Name already in use")]
        public string FullName { get; set; }

        [Phone(ErrorMessage = "Invalid phone number")]
        [Remote(action: "VerifyContactNumber", controller: "Profile", AdditionalFields = "Id", ErrorMessage = "Contact number already in use")]
        public string? Contact { get; set; }

        public string? Region { get; set; }
    }

    public class ChangePasswordDto
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "Current password is required")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "New password is required")]
        public string NewPassword { get; set; }

        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }
    }
}