using InventoryManagement.API.DataProvider;
using System;
using System.Diagnostics.CodeAnalysis;

namespace InventoryManagement.API.Models
{
    [ExcludeFromCodeCoverage]
    public class RequestResponse<T> : IRequestResponse
    {
        public T Item { get; set; }
        public bool Success { get; set; }
        public PagedCollection PagedCollection { get; set; }

        object IRequestResponse.Item
        {
            get { return Item; }
            set { Item = (T)value; }
        }

        public RequestResponse<TOutput> Convert<TOutput>(Converter<T, TOutput> converter) => new RequestResponse<TOutput>
        {
            Item = converter(Item)
        };
    }
}
