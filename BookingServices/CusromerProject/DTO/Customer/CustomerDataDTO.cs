﻿using CustomerProject.DTO.Customer;
using System.ComponentModel.DataAnnotations;


namespace CusromerProject.DTO.Customer
{
    public class CustomerDataDTO
    {
        [Required(ErrorMessage = "Name Is Required")]
        [StringLength(75, MinimumLength = 3 , ErrorMessage = "Minimum Length For Name Is 3 Character And Maxmum Is 75 Character")]
        public string Name { get; set; }
        [Required(ErrorMessage = "UserName Is Required")]
        [UniqueUserName]
        public string Username { get; set; }
        [Required(ErrorMessage = "Email Is Required")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        ErrorMessage = "This InValid Email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Phone Is Required")]
        [RegularExpression(@"^(009665|9665|\+9665|05|5)(5|0|3|6|4|9|1|8|7)([0-9]{7})$",
        ErrorMessage = "Invalid Saudi phone number")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "AlternativePhone Is Required")]
        [RegularExpression(@"^(009665|9665|\+9665|05|5)(5|0|3|6|4|9|1|8|7)([0-9]{7})$",
        ErrorMessage = "Invalid Saudi phone number")]
        [UniquePhone]
        public string AlternativePhone { get; set; }
        [Required(ErrorMessage = "SSN Is Required")]
        [RegularExpression(@"^[12]\d{9}$", ErrorMessage = "Invalid Saudi SSN number")]
        [UniqueSSN]
        public string SSN { get; set; }
        [Required(ErrorMessage = "City Is Required")]
        public string City { get; set; }
        [Required(ErrorMessage = "Password Is Required")]
        [CheckPassword]
        public string Password { get; set; }
    }
}
