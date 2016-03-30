﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Couchbase.Linq;
using IdentityServer4.Core.Models;
using IdentityServer4.Core.Services;
using Microsoft.Extensions.Logging;

namespace IdentityServer4.Couchbase.Services
{
    /// <summary>
    /// CORS policy service that configures the allowed origins from a list of clients' redirect URLs.
    /// </summary>
    public class CouchbaseCorsPolicyService : ICorsPolicyService
    {
        readonly ILogger<CouchbaseCorsPolicyService> _logger;
        readonly IBucketContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="CouchbaseCorsPolicyService"/> class.
        /// </summary>
        /// <param name="clients">The clients.</param>
        public CouchbaseCorsPolicyService(ILogger<CouchbaseCorsPolicyService> logger, IBucketContext context)
        {
            _logger = logger;
            _context = context;
        }

        /// <summary>
        /// Determines whether origin is allowed.
        /// </summary>
        /// <param name="origin">The origin.</param>
        /// <returns></returns>
        public Task<bool> IsOriginAllowedAsync(string origin)
        {
            var query =
                from client in _context.Query<CouchbaseWrapper<Client>>()
                from url in client.Model.AllowedCorsOrigins
                select url.GetOrigin();

            var result = query.Contains(origin, StringComparer.OrdinalIgnoreCase);

            if (result)
            {
                _logger.LogInformation("Client list checked and origin: {0} is allowed", origin);
            }
            else
            {
                _logger.LogInformation("Client list checked and origin: {0} is not allowed", origin);
            }
            
            return Task.FromResult(result);
        }
    }
}
