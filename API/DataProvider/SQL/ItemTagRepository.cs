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
    public class ItemTagRepository : IItemTagRepository
    {
        private readonly string _connectionString;

        public ItemTagRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("InventoryManagementAPI_ConnectionString");
        }

        public async Task<RequestResponse<List<ItemTag>>> GetItemTags(int itemId)
        {
            var requestResponse = new RequestResponse<List<ItemTag>>();
            var ItemTags = new List<ItemTag>();

            await Task.Run(() =>
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var parameter = new DynamicParameters();
                    parameter.Add("@itemID", itemId, DbType.Int32, ParameterDirection.Input);

                    ItemTags = connection
                        .Query<ItemTag>("getItemTags", parameter, commandType: CommandType.StoredProcedure).ToList();

                    requestResponse.Item = ItemTags;
                    requestResponse.Success = true;
                }
            }).ConfigureAwait(true);

            return requestResponse;
        }

        public async Task<RequestResponse<bool>> UpsertItemTags(List<ItemAttributeTableValueParameter> itemTvp)
        {
            var requestResponse = new RequestResponse<bool>();
            var returnValue = -1;

            await Task.Run(() =>
            {
                var dt = new DataTable("ItemTags");
                dt.Columns.Add("attributeID", typeof(int));
                dt.Columns.Add("itemID", typeof(int));
                dt.Columns.Add("isActive", typeof(bool));
                dt.Columns.Add("isSelected", typeof(bool));
                foreach (ItemAttributeTableValueParameter item in itemTvp)
                {
                    dt.Rows.Add(item.AttributeId, item.ItemId, item.IsActive, item.IsSelected);
                }
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var parameter = new DynamicParameters();
                    parameter.Add("@TVP", dt.AsTableValuedParameter("dbo.ItemAttributeTVP"));
                    parameter.Add("@returnValue", returnValue, DbType.Int32, ParameterDirection.ReturnValue);


                    var result = connection
                        .Execute("upsertItemTags", parameter, commandType: CommandType.StoredProcedure);
                    returnValue = parameter.Get<int>("@returnValue");

                    requestResponse.Success = returnValue >= 0;
                }
            }).ConfigureAwait(true);
            return requestResponse;
        }
    }
}
