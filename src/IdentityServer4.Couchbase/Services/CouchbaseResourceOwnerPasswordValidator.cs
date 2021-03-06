﻿using System;
using System.Threading.Tasks;
using Identity.Couchbase;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer4.Couchbase.Services
{
    public class CouchbaseResourceOwnerPasswordValidator<TUser> : IResourceOwnerPasswordValidator
        where TUser : IUser
    {

        readonly UserManager<TUser> _userManager;

        public CouchbaseResourceOwnerPasswordValidator(UserManager<TUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<CustomGrantValidationResult> ValidateAsync(string userName, string password, ValidatedTokenRequest request)
        {

            var user = await _userManager.FindByNameAsync(userName);

            if (user == null)
            {
                return await Task.FromResult(new CustomGrantValidationResult("Invalid username or password"));
            }

            var result = await _userManager.CheckPasswordAsync(user, password);

            if (result)
            {
                return await Task.FromResult(new CustomGrantValidationResult(user.SubjectId, "password"));
            }

            return await Task.FromResult(new CustomGrantValidationResult("Invalid username or password"));

        }
    }
}
