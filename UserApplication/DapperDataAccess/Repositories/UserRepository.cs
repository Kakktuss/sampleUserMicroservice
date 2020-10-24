using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using UserApplication.ViewModels;

namespace UserApplication.DapperDataAccess.Repositories
{
    public class UserRepository : IUserRepository
    {

        private readonly string _connectionString;
        
        public UserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<UserViewModel>> GetUsersAsync()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                try
                {
                    var result = await connection.QueryAsync<UserViewModel>(
                        @"SELECT u.[Uuid] as Uuid,
                                u.[Username] as Username,
                                u.[Email] as Email,
                                u.[IsCompleted] as IsCompleted
                            FROM [dbo].[Users] u");

                    return result;
                }
                catch (InvalidOperationException e)
                {
                    return null;
                }
            }
        }

        public async Task<UserViewModel> FindUserByUuidAsync(string uuid)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                try
                {
                    var dynamicParameters = new DynamicParameters();

                    dynamicParameters.Add("@Uuid", uuid);

                    var result = await connection.QueryFirstAsync<UserViewModel>(
                        @"SELECT u.[Uuid] as Uuid,
                                u.[Username] as Username,
                                u.[Email] as Email,
                                u.[IsCompleted] as IsCompleted
                            FROM [dbo].[Users] u
                            WHERE u.[Uuid] = @Uuid",
                        dynamicParameters);

                    return result;
                }
                catch (InvalidOperationException e)
                {
                    return null;
                }
            }
        }

    }
}