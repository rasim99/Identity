﻿namespace Identity.Areas.Admin.Models.User
{
    public class UserDetailVM
    {
        public string Email { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string PhoneNumber { get; set; }
        public List<string> Roles { get; set; }
    }
}
