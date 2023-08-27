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
    public class ItemRepository: IItemRepository
    {
        private readonly string _connectionString;

        public ItemRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("InventoryManagementAPI_ConnectionString");
        }

        public async Task<RequestResponse<ItemCollectionDataCollection>> GetItemCollection(int subscriptionId, SearchRequest searchRequest)
        {
            var requestResponse = new RequestResponse<ItemCollectionDataCollection>();
            var itemGroupCollection = new List<ItemGroup>();
            var itemTagCollection = new List<ItemTag>();
            var itemCollection = new List<Item>();

            await Task.Run(() =>
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var parameter = new DynamicParameters();
                    parameter.Add("@subscriptionID", subscriptionId, DbType.Int32, ParameterDirection.Input);
                    parameter.Add("@pageNum", searchRequest.PagedCollection.PageNumber, DbType.Int32, ParameterDirection.Input);
                    parameter.Add("@pageSize", searchRequest.PagedCollection.PageSize, DbType.Int32, ParameterDirection.Input);
                    parameter.Add("@collectionTotal", searchRequest.PagedCollection.CollectionTotal, DbType.Int32, ParameterDirection.Output);
                    parameter.Add("@sortBy", searchRequest.SortBy, DbType.String, ParameterDirection.Input);
                    parameter.Add("@sortAsc", searchRequest.SortAscending, DbType.Boolean, ParameterDirection.Input);

                    using (var multi = connection.QueryMultiple("getItemCollection", parameter, commandType: CommandType.StoredProcedure))
                    {
                        itemCollection = multi.Read<Item>().ToList();
                        itemGroupCollection = multi.Read<ItemGroup>().ToList();
                        itemTagCollection = multi.Read<ItemTag>().ToList();
                    }

                    var collectionTotal = parameter.Get<int>("@collectionTotal");
                    var resultDataContainer = new ItemCollectionDataCollection { Items = itemCollection, ItemGroups = itemGroupCollection, ItemTags = itemTagCollection };
                    requestResponse.Item = resultDataContainer;
                    requestResponse.PagedCollection = new PagedCollection { CollectionTotal = collectionTotal, PageNumber = searchRequest.PagedCollection.PageNumber, PageSize = searchRequest.PagedCollection.PageSize };
                    requestResponse.Success = true;
                }
            }).ConfigureAwait(true);

            return requestResponse;
        }

        public async Task<RequestResponse<Item>> GetItem(int itemId)
        {
            var requestResponse = new RequestResponse<Item>();
            var item = new List<Item>();
            var itemGroupCollection = new List<Group>();
            var itemTagCollection = new List<Tag>();
            var itemShoppingListCollection = new List<ShoppingList>();

            await Task.Run(() =>
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var parameter = new DynamicParameters();
                    parameter.Add("@id", itemId, DbType.Int32, ParameterDirection.Input);

                    using (var multi = connection.QueryMultiple("getItem", parameter, commandType: CommandType.StoredProcedure))
                    {
                        item = multi.Read<Item>().ToList();
                        itemGroupCollection = multi.Read<Group>().ToList();
                        itemTagCollection = multi.Read<Tag>().ToList();
                        itemShoppingListCollection = multi.Read<ShoppingList>().ToList();
                    }

                    requestResponse.Item = item.FirstOrDefault();
                    requestResponse.Item.Groups = itemGroupCollection;
                    requestResponse.Item.Tags = itemTagCollection;
                    requestResponse.Item.ShoppingLists = itemShoppingListCollection;
                    requestResponse.Success = true;
                }
            }).ConfigureAwait(true);

            return requestResponse;
        }

        public async Task<int> UpsertItem(Item item)
        {
            var result = 0;
            var returnValue = -1;

            await Task.Run(() =>
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var parameter = new DynamicParameters();
                    parameter.Add("@subscriptionID", item.SubscriptionId, DbType.Int32, ParameterDirection.Input);
                    parameter.Add("@name", item.Name, DbType.String, ParameterDirection.Input);
                    parameter.Add("@description", item.Description, DbType.String, ParameterDirection.Input);
                    parameter.Add("@amountValue", item.AmountValue, DbType.Int32, ParameterDirection.Input);
                    parameter.Add("@isActive", item.IsActive, DbType.Boolean, ParameterDirection.Input);
                    if (item.Id > 0)
                    {
                        parameter.Add("@id", item.Id, DbType.Int32, ParameterDirection.Input);
                    }
                    parameter.Add("@returnValue", returnValue, DbType.Int32, ParameterDirection.ReturnValue);


                    result = connection
                        .Execute("upsertitem", parameter, commandType: CommandType.StoredProcedure);
                    returnValue = parameter.Get<int>("@returnValue");
                }
            }).ConfigureAwait(true);
            return returnValue;
        }

        public async Task<RequestResponse<ItemCollectionDataCollection>> GetLowQuantityItemCollection(int subscriptionId, SearchRequest searchRequest)
        {
            var requestResponse = new RequestResponse<ItemCollectionDataCollection>();
            var itemGroupCollection = new List<ItemGroup>();
            var itemTagCollection = new List<ItemTag>();
            var itemCollection = new List<Item>();

            await Task.Run(() =>
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var parameter = new DynamicParameters();
                    parameter.Add("@subscriptionID", subscriptionId, DbType.Int32, ParameterDirection.Input);
                    parameter.Add("@pageNum", searchRequest.PagedCollection.PageNumber, DbType.Int32, ParameterDirection.Input);
                    parameter.Add("@pageSize", searchRequest.PagedCollection.PageSize, DbType.Int32, ParameterDirection.Input);
                    parameter.Add("@collectionTotal", searchRequest.PagedCollection.CollectionTotal, DbType.Int32, ParameterDirection.Output);
                    parameter.Add("@sortBy", searchRequest.SortBy, DbType.String, ParameterDirection.Input);
                    parameter.Add("@sortAsc", searchRequest.SortAscending, DbType.Boolean, ParameterDirection.Input);

                    using (var multi = connection.QueryMultiple("getLowQuantityItems", parameter, commandType: CommandType.StoredProcedure))
                    {
                        itemCollection = multi.Read<Item>().ToList();
                        itemGroupCollection = multi.Read<ItemGroup>().ToList();
                        itemTagCollection = multi.Read<ItemTag>().ToList();
                    }

                    //itemCollection = connection
                    //    .Query<Item>("getLowQuantityItems", parameter, commandType: CommandType.StoredProcedure).ToList();

                    var collectionTotal = parameter.Get<int>("@collectionTotal");
                    var resultDataContainer = new ItemCollectionDataCollection { Items = itemCollection, ItemGroups = itemGroupCollection, ItemTags = itemTagCollection };
                    requestResponse.Item = resultDataContainer;
                    requestResponse.PagedCollection =  new PagedCollection { CollectionTotal = collectionTotal, PageNumber = searchRequest.PagedCollection.PageNumber, PageSize = searchRequest.PagedCollection.PageSize };
                    requestResponse.Success = true;
                }
            }).ConfigureAwait(true);

            return requestResponse;
        }

        public async Task<RequestResponse<ItemByAttributeSearchDataCollection>> GetItemsByIdCollection(int subscriptionId, SearchRequest searchRequest)
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

                    using (var multi = connection.QueryMultiple("getItemsByIds", parameter, commandType: CommandType.StoredProcedure))
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

        public async Task<RequestResponse<List<StockItem>>> GetStockItems()
        {
            var requestResponse = new RequestResponse<List<StockItem>>();
            var categories = new List<StockItem>();
            var subCategories = new List<StockItem>();
            var items = new List<StockItem>();

            await Task.Run(() =>
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    using (var multi = connection.QueryMultiple("stock.getItems ", commandType: CommandType.StoredProcedure))
                    {
                        categories = multi.Read<StockItem>().ToList();
                        subCategories = multi.Read<StockItem>().ToList();
                        items = multi.Read<StockItem>().ToList();
                    }
                    foreach (StockItem i in items)
                    {
                        i.Children = new List<StockItem>();
                    }
                    foreach (StockItem sc in subCategories)
                    {
                        sc.Children = items.FindAll(i => i.SubCategoryId == sc.SubCategoryId);
                    }
                    foreach (StockItem c in categories)
                    {
                        c.Children = subCategories.FindAll(sc => sc.CategoryId == c.CategoryId);
                    }
                    requestResponse.Item = categories;
                    requestResponse.Success = true;
                }
            }).ConfigureAwait(true);

            return requestResponse;
        }

        public async Task<int> UpsertStockItems(StockItemImport stockItemImport)
        {
            var result = 0;
            var returnValue = -1;

            await Task.Run(() =>
            {
                var dt = new DataTable("ItemIds");
                dt.Columns.Add("id", typeof(int));
                foreach (StockItem stockItem in stockItemImport.Items)
                {
                    dt.Rows.Add(stockItem.ItemId);
                }

                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    var parameter = new DynamicParameters();
                    parameter.Add("@subscriptionID", stockItemImport.SubscriptionId, DbType.Int32, ParameterDirection.Input);
                    parameter.Add("@importAttributeValues", stockItemImport.ImportAttributeValues, DbType.Boolean, ParameterDirection.Input);
                    parameter.Add("@ids", dt.AsTableValuedParameter("dbo.IDTVP"));
                    parameter.Add("@returnValue", returnValue, DbType.Int32, ParameterDirection.ReturnValue);

                    result = connection
                        .Execute("stock.importItems", parameter, commandType: CommandType.StoredProcedure);
                    returnValue = parameter.Get<int>("@returnValue");
                }
            }).ConfigureAwait(true);

            return returnValue;
        }
    }
}
