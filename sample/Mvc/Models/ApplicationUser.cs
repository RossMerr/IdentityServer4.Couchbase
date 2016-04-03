﻿using System;
using System.Collections.Generic;
using System.Security.Claims;
using Identity.Couchbase;

namespace Mvc.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IUser
    {
        public string Id { get; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string NormalizedUserName { get; set; }
        public string PasswordHash { get; set; }
        public ICollection<IRole> Roles { get; }
        public ICollection<UserLogin> Logins { get; }
        public string ConcurrencyStamp { get; set; }
        public ICollection<Claim> Claims { get; set; }
    }
}
