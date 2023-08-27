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
    public class ShoppingListRepository : IShoppingListRepository
    {
        private readonly string _connectionString;

        public ShoppingListRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("InventoryManagementAPI_ConnectionString");
        }

        public async Task<RequestResponse<List<ShoppingList>>> GetShoppingListCollection(int subscriptionId, int pageNumber, int pageSize)
        {
            var requestResponse = new RequestResponse<List<ShoppingList>>();
            var shoppingListCollection = new List<ShoppingList>();

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

                    shoppingListCollection = connection
                        .Query<ShoppingList>("getShoppingLists", parameter, commandType: CommandType.StoredProcedure).ToList();


                    var collectionTotal = parameter.Get<int>("@collectionTotal");
                    requestResponse.Item = shoppingListCollection;
                    requestResponse.PagedCollection = new PagedCollection { CollectionTotal = collectionTotal, PageNumber = pageNumber, PageSize = pageSize };
                    requestResponse.Success = true;
                }
            }).ConfigureAwait(true);

            return requestResponse;
        }

        public async Task<RequestResponse<ShoppingListItemCollection>> GetShoppingListItems(int subscriptionId, int shoppingListId, SearchRequest searchRequest)
        {
            var requestResponse = new RequestResponse<ShoppingListItemCollection>();
            var shoppingListCollection = new ShoppingListItemCollection();
            var shoppingListItems = new List<ShoppingListItem>();
            var itemGroupCollection = new List<ItemGroup>();
            var itemTagCollection = new List<ItemTag>();

            await Task.Run(() =>
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var parameter = new DynamicParameters();
                    parameter.Add("@subscriptionID", subscriptionId, DbType.Int32, ParameterDirection.Input);
                    parameter.Add("@shoppingListID", shoppingListId, DbType.Int32, ParameterDirection.Input);
                    parameter.Add("@sortBy", searchRequest.SortBy, DbType.String, ParameterDirection.Input);
                    parameter.Add("@sortAsc", searchRequest.SortAscending, DbType.Boolean, ParameterDirection.Input);


                    using (var multi = connection.QueryMultiple("getShoppingListItems", parameter, commandType: CommandType.StoredProcedure))
                    {
                        shoppingListItems = multi.Read<ShoppingListItem>().ToList();
                        itemGroupCollection = multi.Read<ItemGroup>().ToList();
                        itemTagCollection = multi.Read<ItemTag>().ToList();
                    }

                    shoppingListCollection.ShoppingListItems = shoppingListItems;
                    shoppingListCollection.ItemTags = itemTagCollection;
                    shoppingListCollection.ItemGroups = itemGroupCollection;
                    requestResponse.Item = shoppingListCollection;
                    requestResponse.Success = true;
                }
            }).ConfigureAwait(true);



            return requestResponse;
        }


        public async Task<RequestResponse<ShoppingList>> UpsertShoppingList(ShoppingList shoppingList)
        {
            var requestResponse = new RequestResponse<ShoppingList>();
            var returnValue = -1;
            var newList = false;

            await Task.Run(() =>
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var parameter = new DynamicParameters();
                    parameter.Add("@subscriptionID", shoppingList.SubscriptionId, DbType.Int32, ParameterDirection.Input);
                    parameter.Add("@name", shoppingList.Name, DbType.String, ParameterDirection.Input);
                    parameter.Add("@isActive", shoppingList.IsActive, DbType.Boolean, ParameterDirection.Input);
                    if (shoppingList.Id > 0)
                    {
                        parameter.Add("@id", shoppingList.Id, DbType.Int32, ParameterDirection.Input);
                    }
                    else
                    {
                        newList = true;
                    }
                    parameter.Add("@returnValue", returnValue, DbType.Int32, ParameterDirection.ReturnValue);


                    var result = connection
                        .Execute("upsertShoppingList", parameter, commandType: CommandType.StoredProcedure);
                    returnValue = parameter.Get<int>("@returnValue");

                    requestResponse.Success = (shoppingList.Id > 0 && returnValue == 0) || (shoppingList.Id < 1 && returnValue > 0);
                    shoppingList.Id = newList ? returnValue : shoppingList.Id;
                    requestResponse.Item = shoppingList;
                }
            }).ConfigureAwait(true);
            return requestResponse;
        }

        public async Task<RequestResponse<bool>> UpsertShoppingListItems(int shoppingListId, List<ShoppingListTableValueParameter> itemTvp)
        {
            var requestResponse = new RequestResponse<bool>();
            var returnValue = -1;

            await Task.Run(() =>
            {
                var dt = new DataTable("ShoppingListItems");
                dt.Columns.Add("shoppingListID", typeof(int));
                dt.Columns.Add("itemID", typeof(int));
                dt.Columns.Add("subscriptionID", typeof(int));
                dt.Columns.Add("isActive", typeof(bool));
                dt.Columns.Add("isSelected", typeof(bool));
                foreach (ShoppingListTableValueParameter item in itemTvp)
                {
                    dt.Rows.Add(item.ShoppingListId, item.ItemId, item.SubscriptionId, item.IsActive, item.IsSelected);
                }
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var parameter = new DynamicParameters();
                    parameter.Add("@TVP", dt.AsTableValuedParameter("dbo.ShoppingListItemTVP"));
                    if (shoppingListId > 0)
                    {
                        parameter.Add("@id", shoppingListId, DbType.Int32, ParameterDirection.Input);
                    }
                    parameter.Add("@returnValue", returnValue, DbType.Int32, ParameterDirection.ReturnValue);


                    var result = connection
                        .Execute("upsertShoppingListItems", parameter, commandType: CommandType.StoredProcedure);
                    returnValue = parameter.Get<int>("@returnValue");

                    requestResponse.Success = returnValue >= 0;
                }
            }).ConfigureAwait(true);
            return requestResponse;
        }

        public async Task<RequestResponse<bool>> ShoppingListCheckout(List<ShoppingListTableValueParameter> itemTvp)
        {
            var requestResponse = new RequestResponse<bool>();
            var returnValue = -1;

            await Task.Run(() =>
            {
                var dt = new DataTable("ShoppingListItems");
                dt.Columns.Add("shoppingListID", typeof(int));
                dt.Columns.Add("itemID", typeof(int));
                dt.Columns.Add("subscriptionID", typeof(int));
                dt.Columns.Add("isActive", typeof(bool));
                dt.Columns.Add("isSelected", typeof(bool));
                foreach (ShoppingListTableValueParameter item in itemTvp)
                {
                    dt.Rows.Add(item.ShoppingListId, item.ItemId, item.SubscriptionId, item.IsActive, item.IsSelected);
                }
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var parameter = new DynamicParameters();
                    parameter.Add("@TVP", dt.AsTableValuedParameter("dbo.ShoppingListItemTVP"));
                    parameter.Add("@returnValue", returnValue, DbType.Int32, ParameterDirection.ReturnValue);


                    var result = connection
                        .Execute("shoppingListCheckout", parameter, commandType: CommandType.StoredProcedure);
                    returnValue = parameter.Get<int>("@returnValue");

                    requestResponse.Success = returnValue >= 0;
                }
            }).ConfigureAwait(true);
            return requestResponse;
        }

        public async Task<RequestResponse<List<ShoppingListTableValueParameter>>> InitShoppingListCheckout(List<ShoppingListTableValueParameter> itemTvp)
        {
            var requestResponse = new RequestResponse<List<ShoppingListTableValueParameter>>();
            var returnValue = -1;
            var shoppingListCollection = new List<ShoppingListTableValueParameter>();

            await Task.Run(() =>
            {
                var dt = new DataTable("ShoppingListItems");
                dt.Columns.Add("shoppingListID", typeof(int));
                dt.Columns.Add("itemID", typeof(int));
                dt.Columns.Add("subscriptionID", typeof(int));
                dt.Columns.Add("isActive", typeof(bool));
                dt.Columns.Add("isSelected", typeof(bool));
                foreach (ShoppingListTableValueParameter item in itemTvp)
                {
                    dt.Rows.Add(item.ShoppingListId, item.ItemId, item.SubscriptionId, item.IsActive, item.IsSelected);
                }
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var parameter = new DynamicParameters();
                    parameter.Add("@TVP", dt.AsTableValuedParameter("dbo.ShoppingListItemTVP"));
                    parameter.Add("@returnValue", returnValue, DbType.Int32, ParameterDirection.ReturnValue);

                    shoppingListCollection = connection
                        .Query<ShoppingListTableValueParameter>("shoppingListItemCheckoutInitialize", parameter, commandType: CommandType.StoredProcedure).ToList();
                    returnValue = parameter.Get<int>("@returnValue");

                    requestResponse.Success = returnValue >= 0;
                    requestResponse.Item = shoppingListCollection;
                }
            }).ConfigureAwait(true);
            return requestResponse;
        }

        public async Task<RequestResponse<List<ShoppingListTableValueParameter>>> UpdateShoppingListItemCheckoutStatus(ShoppingListItem item)
        {
            var requestResponse = new RequestResponse<List<ShoppingListTableValueParameter>>();
            var shoppingListCollection = new List<ShoppingListTableValueParameter>();
            var returnValue = -1;

            await Task.Run(() =>
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var parameter = new DynamicParameters();
                    parameter.Add("@itemId", item.ItemId, DbType.Int32, ParameterDirection.Input);
                    parameter.Add("@shoppingListId", item.ShoppingListId, DbType.Int32, ParameterDirection.Input);
                    parameter.Add("@selected", item.IsSelected, DbType.Boolean, ParameterDirection.Input);
                    parameter.Add("@returnValue", returnValue, DbType.Int32, ParameterDirection.ReturnValue);



                    shoppingListCollection = connection
                        .Query<ShoppingListTableValueParameter>("shoppingListItemCheckoutUpdateSelection", parameter, commandType: CommandType.StoredProcedure).ToList();
                    returnValue = parameter.Get<int>("@returnValue");

                    requestResponse.Success = returnValue >= 0;
                    requestResponse.Item = shoppingListCollection;
                }
            }).ConfigureAwait(true);
            return requestResponse;
        }

        public async Task<RequestResponse<List<ShoppingListTableValueParameter>>> SyncShoppingListItems(int shoppingListId)
        {
            var requestResponse = new RequestResponse<List<ShoppingListTableValueParameter>>();
            var shoppingListCollection = new List<ShoppingListTableValueParameter>();

            await Task.Run(() =>
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var parameter = new DynamicParameters();
                    parameter.Add("@shoppingListId", shoppingListId, DbType.Int32, ParameterDirection.Input);



                    shoppingListCollection = connection
                        .Query<ShoppingListTableValueParameter>("shoppingListItemCheckoutSync", parameter, commandType: CommandType.StoredProcedure).ToList();

                    requestResponse.Success = shoppingListCollection.Count > 0;
                    requestResponse.Item = shoppingListCollection;
                }
            }).ConfigureAwait(true);
            return requestResponse;
        }

        public async Task<RequestResponse<List<ShoppingList>>> GetShoppingListCollectionWithItemCounts(int subscriptionId, SearchRequest searchRequest)
        {
            var requestResponse = new RequestResponse<List<ShoppingList>>();
            var shoppingListCollection = new List<ShoppingList>();

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

                    shoppingListCollection = connection
                        .Query<ShoppingList>("getShoppingListsWithItemCounts", parameter, commandType: CommandType.StoredProcedure).ToList();


                    var collectionTotal = parameter.Get<int>("@collectionTotal");
                    requestResponse.Item = shoppingListCollection;
                    requestResponse.PagedCollection = new PagedCollection { CollectionTotal = collectionTotal, PageNumber = searchRequest.PagedCollection.PageNumber, PageSize = searchRequest.PagedCollection.PageSize };
                    requestResponse.Success = true;
                }
            }).ConfigureAwait(true);

            return requestResponse;
        }
    }
}
