﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Couchbase.Core;
using Couchbase.Linq;
using IdentityServer4.Couchbase.Wrappers;
using IdentityServer4.Models;
using IdentityServer4.Services;

namespace IdentityServer4.Couchbase.Services
{
    /// <summary>
    /// Couchbase token handle store
    /// </summary>
    public class CouchbaseTokenHandleStore : ITokenHandleStore
    {
        readonly IBucket _bucket;
        readonly IBucketContext _context;

        public CouchbaseTokenHandleStore(IBucket bucket, IBucketContext context)
        {
            _bucket = bucket;
            _context = context;
        }


        /// <summary>
        /// Stores the data.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public Task StoreAsync(string key, Token value)
        {
            return _bucket.InsertAsync(TokenWrapper.TokenWrapperId(key), new TokenWrapper()
            {
                Id = key,
                Model = new Token()
            });
        }

        /// <summary>
        /// Retrieves the data.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public async Task<Token> GetAsync(string key)
        {
            var result = await _bucket.GetAsync<TokenWrapper>(TokenWrapper.TokenWrapperId(key));
            return result.Success ? result.Value.Model : null;
        }

        /// <summary>
        /// Removes the data.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public Task RemoveAsync(string key)
        {
            return _bucket.RemoveAsync(TokenWrapper.TokenWrapperId(key));
        }

        /// <summary>
        /// Retrieves all data for a subject identifier.
        /// </summary>
        /// <param name="subject">The subject identifier.</param>
        /// <returns>
        /// A list of token metadata
        /// </returns>
        public Task<IEnumerable<ITokenMetadata>> GetAllAsync(string subject)
        {
            var query =
                from item in _context.Query<TokenWrapper>()
                where item.Model.SubjectId == subject
                select item.Model;
            var list = query.ToArray();
            return Task.FromResult(list.Cast<ITokenMetadata>());
        }

        /// <summary>
        /// Revokes all data for a client and subject id combination.
        /// </summary>
        /// <param name="subject">The subject.</param>
        /// <param name="client">The client.</param>
        /// <returns></returns>
        public Task RevokeAsync(string subject, string client)
        {
            var query =
                from item in _context.Query<TokenWrapper>()
                where item.Model.SubjectId == subject && item.Model.ClientId == client
                select item.Id;

            foreach (var key in query)
            {
                _bucket.RemoveAsync(key);
            }

            return Task.FromResult(0);
        }
    }
}