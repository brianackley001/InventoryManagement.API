using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using InventoryManagement.API.Models;
using Microsoft.Extensions.Configuration;


namespace InventoryManagement.API.DataProvider.SQL
{
    public class TagRepository : ITagRepository
    {
        private readonly string _connectionString;

        public TagRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("InventoryManagementAPI_ConnectionString");
        }


        public async Task<RequestResponse<List<Tag>>> GetTagCollection(int subscriptionId, int pageNumber, int pageSize)
        {
            var requestResponse = new RequestResponse<List<Tag>>();
            var tagCollection = new List<Tag>();

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

                    tagCollection = connection
                        .Query<Tag>("getTagCollection", parameter, commandType: CommandType.StoredProcedure).ToList();

                    var collectionTotal = parameter.Get<int>("@collectionTotal");
                    requestResponse.Item = tagCollection;
                    requestResponse.PagedCollection = new PagedCollection { CollectionTotal = collectionTotal, PageNumber = pageNumber, PageSize = pageSize };
                    requestResponse.Success = true;
                }
            }).ConfigureAwait(true);

            return requestResponse;
        }

        public async Task<RequestResponse<List<Tag>>> GetTags(int subscriptionId)
        {
            var requestResponse = new RequestResponse<List<Tag>>();
            var tagCollection = new List<Tag>();

            await Task.Run(() =>
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var parameter = new DynamicParameters();
                    parameter.Add("@subscriptionID", subscriptionId, DbType.Int32, ParameterDirection.Input);

                    tagCollection = connection
                        .Query<Tag>("getTags", parameter, commandType: CommandType.StoredProcedure).ToList();

                    requestResponse.Item = tagCollection;
                    requestResponse.Success = true;
                }
            }).ConfigureAwait(true);

            return requestResponse;
        }

        public async Task<int> UpsertTag(Tag tag)
        {
            var result = 0;
            var returnValue = -1;

            await Task.Run(() =>
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var parameter = new DynamicParameters();
                    parameter.Add("@subscriptionID", tag.SubscriptionId, DbType.Int32, ParameterDirection.Input);
                    parameter.Add("@name", tag.Name, DbType.String, ParameterDirection.Input);
                    parameter.Add("@isActive", tag.IsActive, DbType.Boolean, ParameterDirection.Input);
                    if (tag.Id > 0)
                    {
                        parameter.Add("@id", tag.Id, DbType.Int32, ParameterDirection.Input);
                    }
                    parameter.Add("@returnValue", returnValue, DbType.Int32, ParameterDirection.ReturnValue);


                    result = connection
                        .Execute("upsertTag", parameter, commandType: CommandType.StoredProcedure);
                    returnValue = parameter.Get<int>("@returnValue");
                }
            }).ConfigureAwait(true);
            return returnValue;
        }

        public async Task<RequestResponse<List<Tag>>> GetTagCollectionWithItemCounts(int subscriptionId, SearchRequest searchRequest)
        {
            var requestResponse = new RequestResponse<List<Tag>>();
            var tagCollection = new List<Tag>();

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

                    tagCollection = connection
                        .Query<Tag>("getTagsWithItemCounts", parameter, commandType: CommandType.StoredProcedure).ToList();

                    var collectionTotal = parameter.Get<int>("@collectionTotal");
                    requestResponse.Item = tagCollection;
                    requestResponse.PagedCollection = new PagedCollection { CollectionTotal = collectionTotal, PageNumber = searchRequest.PagedCollection.PageNumber, PageSize = searchRequest.PagedCollection.PageSize };
                    requestResponse.Success = true;
                }
            }).ConfigureAwait(true);

            return requestResponse;
        }
    }
}
