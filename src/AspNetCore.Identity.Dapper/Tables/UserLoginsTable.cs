﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.Dapper
{
    /// <summary>
    /// The default implementation of <see cref="IUserLoginsTable{TUser, TKey, TUserLogin}"/>.
    /// </summary>
    /// <typeparam name="TDbConnection">The type of the database connection class used to access the store.</typeparam>
    /// <typeparam name="TUser">The type representing a user.</typeparam>
    /// <typeparam name="TKey">The type of the primary key for a user.</typeparam>
    /// <typeparam name="TUserLogin">The type representing a user external login.</typeparam>
    public class UserLoginsTable<TDbConnection, TUser, TKey, TUserLogin> : IUserLoginsTable<TUser, TKey, TUserLogin>
        where TDbConnection : IDbConnection
        where TUser : IdentityUser<TKey>
        where TKey : IEquatable<TKey>
        where TUserLogin : IdentityUserLogin<TKey>, new()
    {
        /// <summary>
        /// Creates a new instance of <see cref="UserLoginsTable{TDbConnection, TUser, TKey, TUserLogin}"/>.
        /// </summary>
        /// <param name="dbConnection">The <see cref="IDbConnection"/> to use.</param>
        public UserLoginsTable(TDbConnection dbConnection) {
            DbConnection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection));
        }

        /// <summary>
        /// The <see cref="IDbConnection"/> to use.
        /// </summary>
        protected TDbConnection DbConnection { get; set; }

        /// <inheritdoc/>
        public virtual async Task<IEnumerable<TUserLogin>> GetLoginsAsync(TKey userId) {
            const string sql = "SELECT * " +
                               "FROM [dbo].[AspNetUserLogins] " +
                               "WHERE [UserId] = @UserId;";
            var userLogins = await DbConnection.QueryAsync<TUserLogin>(sql, new { UserId = userId });
            return userLogins;
        }

        /// <inheritdoc/>
        public virtual async Task<TUser> FindByLoginAsync(string loginProvider, string providerKey) {
            string[] sql =
            {
                "SELECT [UserId] " +
                "FROM [dbo].[AspNetUserLogins] " +
                "WHERE [LoginProvider] = @LoginProvider AND [ProviderKey] = @ProviderKey;"
            };
            var userId = await DbConnection.QuerySingleOrDefaultAsync<TKey>(sql[0], new {
                LoginProvider = loginProvider,
                ProviderKey = providerKey
            });
            if (userId == null) {
                return null;
            }
            sql[0] = "SELECT * " +
                     "FROM [dbo].[AspNetUsers] " +
                     "WHERE [Id] = @Id;";
            var user = await DbConnection.QuerySingleAsync<TUser>(sql[0], new { Id = userId });
            return user;
        }

        /// <inheritdoc/>
        public virtual async Task<TUserLogin> FindUserLoginAsync(string loginProvider, string providerKey) {
            const string sql = "SELECT * " +
                               "FROM [dbo].[AspNetUserLogins] " +
                               "WHERE [LoginProvider] = @LoginProvider AND [ProviderKey] = @ProviderKey;";
            var userLogin = await DbConnection.QuerySingleOrDefaultAsync<TUserLogin>(sql, new {
                LoginProvider = loginProvider,
                ProviderKey = providerKey
            });
            return userLogin;
        }

        /// <inheritdoc/>
        public virtual async Task<TUserLogin> FindUserLoginAsync(TKey userId, string loginProvider, string providerKey) {
            const string sql = "SELECT * " +
                               "FROM [dbo].[AspNetUserLogins] " +
                               "WHERE [UserId] = @UserId AND [LoginProvider] = @LoginProvider AND [ProviderKey] = @ProviderKey;";
            var userLogin = await DbConnection.QuerySingleOrDefaultAsync<TUserLogin>(sql, new {
                UserId = userId,
                LoginProvider = loginProvider,
                ProviderKey = providerKey
            });
            return userLogin;
        }
    }
}
