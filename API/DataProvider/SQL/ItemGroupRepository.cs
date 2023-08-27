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
    public class ItemGroupRepository : IItemGroupRepository
    {
        private readonly string _connectionString;

        public ItemGroupRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("InventoryManagementAPI_ConnectionString");
        }
        public async Task<RequestResponse<List<ItemGroup>>> GetItemGroups(int itemId)
        {
            var requestResponse = new RequestResponse<List<ItemGroup>>();
            var itemGroups = new List<ItemGroup>();

            await Task.Run(() =>
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var parameter = new DynamicParameters();
                    parameter.Add("@itemID", itemId, DbType.Int32, ParameterDirection.Input);

                    itemGroups = connection
                        .Query<ItemGroup>("getItemGroups", parameter, commandType: CommandType.StoredProcedure).ToList();

                    requestResponse.Item = itemGroups;
                    requestResponse.Success = true;
                }
            }).ConfigureAwait(true);

            return requestResponse;
        }

        public async Task<RequestResponse<bool>> UpsertItemGroups(List<ItemAttributeTableValueParameter> itemTvp)
        {
            var requestResponse = new RequestResponse<bool>();
            var returnValue = -1;

            await Task.Run(() =>
            {
                var dt = new DataTable("ItemGroups");
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


                    connection
                        .Execute("upsertItemGroups", parameter, commandType: CommandType.StoredProcedure);
                    returnValue = parameter.Get<int>("@returnValue");

                    requestResponse.Success = returnValue >= 0;
                }
            }).ConfigureAwait(true);
            return requestResponse;
        }
    }
}
