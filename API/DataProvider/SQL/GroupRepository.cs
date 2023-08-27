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
    public class GroupRepository : IGroupRepository
    {
        private readonly string _connectionString;

        public GroupRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("InventoryManagementAPI_ConnectionString");
        }

        public async Task<RequestResponse<List<Group>>> GetGroupCollection(int subscriptionId, int pageNumber, int pageSize)
        {
            var requestResponse = new RequestResponse<List<Group>>();
            var groupCollection = new List<Group>();

            await Task.Run(() =>
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var parameter = new DynamicParameters();
                    parameter.Add("@subscriptionID", subscriptionId, DbType.Int32, ParameterDirection.Input);
                    parameter.Add("@pageNum", pageNumber, DbType.Int32, ParameterDirection.Input);
                    parameter.Add("@pageSize", pageSize, DbType.Int32, ParameterDirection.Input);
                    parameter.Add("@collectionTotal", subscriptionId, DbType.Int32, ParameterDirection.Output);

                    groupCollection = connection
                        .Query<Group>("getGroupCollection", parameter, commandType: CommandType.StoredProcedure).ToList();

                    var collectionTotal = parameter.Get<int>("@collectionTotal");
                    requestResponse.Item = groupCollection;
                    requestResponse.PagedCollection = new PagedCollection { CollectionTotal = collectionTotal, PageNumber = pageNumber, PageSize = pageSize };
                    requestResponse.Success = true;
                }
            }).ConfigureAwait(true);

            return requestResponse;
        }

        public async Task<RequestResponse<List<Group>>> GetGroupCollectionWithItemCounts(int subscriptionId, SearchRequest searchRequest)
        {
            var requestResponse = new RequestResponse<List<Group>>();
            var groupCollection = new List<Group>();

            await Task.Run(() =>
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var parameter = new DynamicParameters();
                    parameter.Add("@subscriptionID", subscriptionId, DbType.Int32, ParameterDirection.Input);
                    parameter.Add("@pageNum", searchRequest.PagedCollection.PageNumber, DbType.Int32, ParameterDirection.Input);
                    parameter.Add("@pageSize", searchRequest.PagedCollection.PageSize, DbType.Int32, ParameterDirection.Input);
                    parameter.Add("@sortBy", searchRequest.SortBy, DbType.String, ParameterDirection.Input);
                    parameter.Add("@sortAsc", searchRequest.SortAscending, DbType.Boolean, ParameterDirection.Input);
                    parameter.Add("@collectionTotal", searchRequest.PagedCollection.CollectionTotal, DbType.Int32, ParameterDirection.Output);

                    groupCollection = connection
                        .Query<Group>("getGroupsWithItemCounts", parameter, commandType: CommandType.StoredProcedure).ToList();

                    var collectionTotal = parameter.Get<int>("@collectionTotal");
                    requestResponse.Item = groupCollection;
                    requestResponse.PagedCollection = new PagedCollection { CollectionTotal = collectionTotal, PageNumber = searchRequest.PagedCollection.PageNumber, PageSize = searchRequest.PagedCollection.PageSize };
                    requestResponse.Success = true;
                }
            }).ConfigureAwait(true);

            return requestResponse;
        }

        public async Task<RequestResponse<List<Group>>> GetGroups(int subscriptionId)
        {
            var requestResponse = new RequestResponse<List<Group>>();
            var groupCollection = new List<Group>();

            await Task.Run(() =>
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var parameter = new DynamicParameters();
                    parameter.Add("@subscriptionID", subscriptionId, DbType.Int32, ParameterDirection.Input);

                    groupCollection = connection
                        .Query<Group>("getGroups", parameter, commandType: CommandType.StoredProcedure).ToList();

                    requestResponse.Item = groupCollection;
                    requestResponse.Success = true;
                }
            }).ConfigureAwait(true);

            return requestResponse;
        }

        public async Task<int> UpsertGroup(Group group)
        {
            var result = 0;
            var returnValue = -1;

            await Task.Run(() =>
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var parameter = new DynamicParameters();
                    parameter.Add("@subscriptionID", group.SubscriptionId, DbType.Int32, ParameterDirection.Input);
                    parameter.Add("@name", group.Name, DbType.String, ParameterDirection.Input);
                    parameter.Add("@isActive", group.IsActive, DbType.Boolean, ParameterDirection.Input);
                    if (group.Id > 0)
                    {
                        parameter.Add("@id", group.Id, DbType.Int32, ParameterDirection.Input);
                    }
                    parameter.Add("@returnValue", returnValue, DbType.Int32, ParameterDirection.ReturnValue);


                    result = connection
                        .Execute("upsertGroup", parameter, commandType: CommandType.StoredProcedure);
                    returnValue = parameter.Get<int>("@returnValue");
                }
            }).ConfigureAwait(true);
            return returnValue;
        }
    }
}
