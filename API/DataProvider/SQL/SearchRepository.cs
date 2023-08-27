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
    public class SearchRepository : ISearchRepository
    {
        private readonly string _connectionString;

        public SearchRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("InventoryManagementAPI_ConnectionString");
        }

        public async Task<RequestResponse<ItemByAttributeSearchDataCollection>> GetItemsByGroupCollection(int subscriptionId, SearchRequest searchRequest)
        {
            var requestResponse = new RequestResponse<ItemByAttributeSearchDataCollection>();
            var uniqueItemCollection = new List<Item>();
            var groupCollection = new List<Group>();
            var itemGroupCollection = new List<ItemGroup>();
            var tagCollection = new List<Tag>();
            var itemTagCollection = new List<ItemTag>();

            await Task.Run(() =>
            {
                var dt = new DataTable("GroupIds");
                dt.Columns.Add("id", typeof(int));
                foreach (int id in searchRequest.IdCollection)
                {
                    dt.Rows.Add(id);
                }

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
                    parameter.Add("@ids", dt.AsTableValuedParameter("dbo.IDTVP"));

                    using (var multi = connection.QueryMultiple("getItemsByGroups", parameter, commandType: CommandType.StoredProcedure))
                    {
                        uniqueItemCollection = multi.Read<Item>().ToList();
                        tagCollection = multi.Read<Tag>().ToList();
                        itemTagCollection = multi.Read<ItemTag>().ToList();
                        groupCollection = multi.Read<Group>().ToList();
                        itemGroupCollection = multi.Read<ItemGroup>().ToList();
                    }

                    var collectionTotal = parameter.Get<int>("@collectionTotal");
                    var resultDataContainer = new ItemByAttributeSearchDataCollection(items: uniqueItemCollection, tags: tagCollection, itemTags: itemTagCollection, groups: groupCollection, itemGroups: itemGroupCollection);

                    requestResponse.Item = resultDataContainer;
                    requestResponse.PagedCollection = new PagedCollection { CollectionTotal = collectionTotal, PageNumber = searchRequest.PagedCollection.PageNumber, PageSize = searchRequest.PagedCollection.PageSize };
                    requestResponse.Success = true;
                }
            }).ConfigureAwait(true);

            return requestResponse;
        }

        public async Task<RequestResponse<ItemByAttributeSearchDataCollection>> GetItemsByTagCollection(int subscriptionId, SearchRequest searchRequest)
        {
            var requestResponse = new RequestResponse<ItemByAttributeSearchDataCollection>();
            var uniqueItemCollection = new List<Item>();
            var tagCollection = new List<Tag>();
            var itemTagCollection = new List<ItemTag>();
            var groupCollection = new List<Group>();
            var itemGroupCollection = new List<ItemGroup>();

            await Task.Run(() =>
            {
                var dt = new DataTable("TagIds");
                dt.Columns.Add("id", typeof(int));
                foreach (int id in searchRequest.IdCollection)
                {
                    dt.Rows.Add(id);
                }

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
                    parameter.Add("@ids", dt.AsTableValuedParameter("dbo.IDTVP"));

                    using (var multi = connection.QueryMultiple("getItemsByTags", parameter, commandType: CommandType.StoredProcedure))
                    {
                        uniqueItemCollection = multi.Read<Item>().ToList();
                        tagCollection = multi.Read<Tag>().ToList();
                        itemTagCollection = multi.Read<ItemTag>().ToList();
                        groupCollection = multi.Read<Group>().ToList();
                        itemGroupCollection = multi.Read<ItemGroup>().ToList();
                    }

                    var collectionTotal = parameter.Get<int>("@collectionTotal");
                    var resultDataContainer = new ItemByAttributeSearchDataCollection(items: uniqueItemCollection, tags:tagCollection, itemTags:itemTagCollection, groups: groupCollection, itemGroups: itemGroupCollection);

                    requestResponse.Item = resultDataContainer;
                    requestResponse.PagedCollection = new PagedCollection { CollectionTotal = collectionTotal, PageNumber = searchRequest.PagedCollection.PageNumber, PageSize = searchRequest.PagedCollection.PageSize };
                    requestResponse.Success = true;
                }
            }).ConfigureAwait(true);

            return requestResponse;
        }

        public async Task<RequestResponse<SearchResultDataCollection>> GetSearchResultCollection(int subscriptionId, SearchRequest searchRequest)
        {
            var requestResponse = new RequestResponse<SearchResultDataCollection>();
            var resultCollection = new List<SearchResult>();
            var tagCollection = new List<Tag>();
            var itemTagCollection = new List<ItemTag>();
            var groupCollection = new List<Group>();
            var itemGroupCollection = new List<ItemGroup>();

            await Task.Run(() =>
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var parameter = new DynamicParameters();
                    parameter.Add("@subscriptionID", subscriptionId, DbType.Int32, ParameterDirection.Input);
                    parameter.Add("@searchTerm", searchRequest.SearchTerm, DbType.String, ParameterDirection.Input);
                    parameter.Add("@collectionTotal", subscriptionId, DbType.Int32, ParameterDirection.Output);
                    parameter.Add("@pageNum", searchRequest.PagedCollection.PageNumber, DbType.Int32, ParameterDirection.Input);
                    parameter.Add("@pageSize", searchRequest.PagedCollection.PageSize, DbType.Int32, ParameterDirection.Input);
                    parameter.Add("@sortBy", searchRequest.SortBy, DbType.String, ParameterDirection.Input);
                    parameter.Add("@sortAsc", searchRequest.SortAscending, DbType.Boolean, ParameterDirection.Input);
                    parameter.Add("@collectionTotal", searchRequest.PagedCollection.CollectionTotal, DbType.Int32, ParameterDirection.Output);

                    if (searchRequest.SearchType != string.Empty)
                    {
                        parameter.Add("@searchType", searchRequest.SearchType, DbType.String, ParameterDirection.Input);
                    }

                    using(var multi = connection.QueryMultiple("getSearchResults", parameter, commandType: CommandType.StoredProcedure))
                    {
                        resultCollection = multi.Read<SearchResult>().ToList();
                        tagCollection = multi.Read<Tag>().ToList();
                        itemTagCollection = multi.Read<ItemTag>().ToList();
                        groupCollection = multi.Read<Group>().ToList();
                        itemGroupCollection = multi.Read<ItemGroup>().ToList();
                    }

                    var collectionTotal = parameter.Get<int>("@collectionTotal");
                    var resultDataContainer = new SearchResultDataCollection { SearchResults = resultCollection, ItemGroups = itemGroupCollection, ItemTags = itemTagCollection, Tags = tagCollection, Groups = groupCollection };
                    requestResponse.Item = resultDataContainer;
                    requestResponse.PagedCollection = new PagedCollection { CollectionTotal = collectionTotal, PageNumber = searchRequest.PagedCollection.PageNumber, PageSize = searchRequest.PagedCollection.PageSize };
                    requestResponse.Success = true;
                }
            }).ConfigureAwait(true);

            return requestResponse;
        }

        public async Task<RequestResponse<SearchResultDataCollection>> GetLowQuantityItemCollection(int subscriptionId, SearchRequest searchRequest)
        {
            var requestResponse = new RequestResponse<SearchResultDataCollection>();
            var resultCollection = new List<SearchResult>();
            var tagCollection = new List<Tag>();
            var itemTagCollection = new List<ItemTag>();
            var groupCollection = new List<Group>();
            var itemGroupCollection = new List<ItemGroup>();
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

                    using (var multi = connection.QueryMultiple("getLowQuantityItems", parameter, commandType: CommandType.StoredProcedure))
                    {
                        resultCollection = multi.Read<SearchResult>().ToList();
                        tagCollection = multi.Read<Tag>().ToList();
                        itemTagCollection = multi.Read<ItemTag>().ToList();
                        groupCollection = multi.Read<Group>().ToList();
                        itemGroupCollection = multi.Read<ItemGroup>().ToList();
                    }

                    var collectionTotal = parameter.Get<int>("@collectionTotal");
                    var resultDataContainer = new SearchResultDataCollection { SearchResults = resultCollection, ItemGroups = itemGroupCollection, ItemTags = itemTagCollection, Tags = tagCollection, Groups = groupCollection };
                    requestResponse.Item = resultDataContainer;
                    requestResponse.PagedCollection = new PagedCollection { CollectionTotal = collectionTotal, PageNumber = searchRequest.PagedCollection.PageNumber, PageSize = searchRequest.PagedCollection.PageSize };
                    requestResponse.Success = true;
                }
            }).ConfigureAwait(true);

            return requestResponse;
        }
    }
}
