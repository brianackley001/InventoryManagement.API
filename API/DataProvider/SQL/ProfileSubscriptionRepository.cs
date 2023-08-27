using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using InventoryManagement.API.Models;
using Microsoft.Extensions.Configuration;

namespace InventoryManagement.API.DataProvider.SQL
{
    [ExcludeFromCodeCoverage]
    public class ProfileSubscriptionRepository : IProfileSubscriptionRepository
    {
        private readonly string _connectionString;
        public ProfileSubscriptionRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("InventoryManagementAPI_ConnectionString");
        }

        public async Task<RequestResponse<UserProfile>> GetProfileSubscriptions(string authId)
        {
            var requestResponse = new RequestResponse<UserProfile>();
            var userProfile = new UserProfile();
            var profileSubscriptions = new List<Subscription>();
            requestResponse.Item = new UserProfile();

            await Task.Run(() =>
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var parameter = new DynamicParameters();
                    parameter.Add("@authID", authId, DbType.String, ParameterDirection.Input);

                    try
                    {
                        using (var multi = connection.QueryMultiple("getProfileSubscriptions", parameter, commandType: CommandType.StoredProcedure))
                        {
                            userProfile = multi.ReadSingle<UserProfile>();
                            profileSubscriptions = multi.Read<Subscription>().ToList();
                        }
                        userProfile.Subscriptions = profileSubscriptions;
                        requestResponse.Success = true;
                    }
                    catch(Exception ex)
                    {
                        if(ex.Message.Contains("Sequence contains no elements"))
                        {
                            // No user profile stored
                            requestResponse.Success = false;

                        }
                        else
                        {
                            throw;
                        }
                    }
                    requestResponse.Item = userProfile;
                }
            }).ConfigureAwait(true);

            return requestResponse;
        }

        public async Task<RequestResponse<UserProfile>> UpsertProfile(UserProfile userProfile)
        {
            var requestResponse = new RequestResponse<UserProfile>();
            var result = 0;
            var returnValue = -1;

            await Task.Run(() =>
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var parameter = new DynamicParameters();
                    parameter.Add("@authID", userProfile.AuthId, DbType.String, ParameterDirection.Input);
                    parameter.Add("@source", userProfile.Source, DbType.String, ParameterDirection.Input);
                    parameter.Add("@name", userProfile.Name, DbType.String, ParameterDirection.Input);
                    parameter.Add("@isActive", userProfile.IsActive, DbType.Boolean, ParameterDirection.Input);
                    parameter.Add("@returnValue", returnValue, DbType.Int32, ParameterDirection.ReturnValue);
                    if (userProfile.Id > 0)
                    {
                        parameter.Add("@id", userProfile.Id, DbType.Int32, ParameterDirection.Input);
                    }

                    result = connection
                        .Execute("upsertProfile", parameter, commandType: CommandType.StoredProcedure);

                    returnValue = parameter.Get<int>("@returnValue");
                    requestResponse.Success = (userProfile.Id > 0 && returnValue == 0) || (userProfile.Id < 1 && returnValue > 0);
                    userProfile.Id = userProfile.Id > 0 ? userProfile.Id : returnValue;
                    requestResponse.Item = userProfile;
                }
            }).ConfigureAwait(true);

            return requestResponse;
        }

        public async Task<RequestResponse<Subscription>> UpsertProfileSubscription(Subscription subscription)
        {
            var requestResponse = new RequestResponse<Subscription>();
            var result = 0;
            var returnValue = -1;

            await Task.Run(() =>
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var parameter = new DynamicParameters();
                    parameter.Add("@subscriptionID", subscription.Id, DbType.Int32, ParameterDirection.Input);
                    parameter.Add("@profileID", subscription.ProfileId, DbType.Int32, ParameterDirection.Input);
                    parameter.Add("@isActive", subscription.IsActive, DbType.Boolean, ParameterDirection.Input);
                    parameter.Add("@isActiveSelection", subscription.IsSelectedSubscription, DbType.Boolean, ParameterDirection.Input);
                    parameter.Add("@returnValue", returnValue, DbType.Int32, ParameterDirection.ReturnValue);
                    if (subscription.ProfileSubscriptionId > 0)
                    {
                        parameter.Add("@id", subscription.ProfileSubscriptionId, DbType.Int32, ParameterDirection.Input);
                    }

                    result = connection
                        .Execute("upsertProfileSubscription", parameter, commandType: CommandType.StoredProcedure);

                    returnValue = parameter.Get<int>("@returnValue");
                    requestResponse.Success = (subscription.ProfileSubscriptionId > 0 && returnValue == 0) || (subscription.ProfileSubscriptionId < 1 && returnValue > 0);
                    subscription.ProfileSubscriptionId = subscription.ProfileSubscriptionId > 0 ? subscription.ProfileSubscriptionId : returnValue;
                    requestResponse.Item = subscription;
                }
            }).ConfigureAwait(true);

            return requestResponse;
        }

        public async Task<RequestResponse<Subscription>> UpsertSubscription(Subscription subscription)
        {
            var requestResponse = new RequestResponse<Subscription>();
            var result = 0;
            var returnValue = -1;

            await Task.Run(() =>
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var parameter = new DynamicParameters();
                    parameter.Add("@profileId", subscription.ProfileId, DbType.Int32, ParameterDirection.Input);
                    parameter.Add("@name", subscription.Name, DbType.String, ParameterDirection.Input);
                    parameter.Add("@isActive", subscription.IsActive, DbType.Boolean, ParameterDirection.Input);
                    parameter.Add("@isActiveSelection", subscription.IsSelectedSubscription, DbType.Boolean, ParameterDirection.Input);
                    parameter.Add("@returnValue", returnValue, DbType.Int32, ParameterDirection.ReturnValue);
                    if (subscription.Id > 0)
                    {
                        parameter.Add("@id", subscription.Id, DbType.Int32, ParameterDirection.Input);
                    }

                    result = connection
                        .Execute("upsertSubscription", parameter, commandType: CommandType.StoredProcedure);

                    returnValue = parameter.Get<int>("@returnValue");
                    requestResponse.Success = (subscription.Id > 0 && returnValue == 0) || (subscription.Id < 1 && returnValue > 0);
                    subscription.Id = subscription.Id > 0 ? subscription.Id : returnValue;
                    requestResponse.Item = subscription;
                }
            }).ConfigureAwait(true);

            return requestResponse;
        }
    }
}
