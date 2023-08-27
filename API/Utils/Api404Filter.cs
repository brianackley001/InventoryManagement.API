using System;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;

namespace InventoryManagement.API.Utils
{
    public class Api404Filter : ITelemetryProcessor
    {
        private ITelemetryProcessor Next { get; set; }

        public Api404Filter(ITelemetryProcessor next)
        {
            Next = next;
        }

        public void Process(ITelemetry item)
        {
            // To filter out an item, just return
            if (ShouldIgnoreRequest(item))
            {
                return;
            }

            Next.Process(item);
        }

        private static bool ShouldIgnoreRequest(ITelemetry item)
        {
            if (item.Context.Operation.Name != null)
            {
                var operationName = item.Context.Operation.Name.ToLower();

                if (item is RequestTelemetry)
                {
                    var req = item as RequestTelemetry;

                    if (!string.IsNullOrEmpty(req.Url.AbsolutePath) && req.Url.AbsolutePath.Contains("/api/"))
                    {
#pragma warning disable S2589 // Boolean expressions should not be gratuitous
                        return req != null && req.ResponseCode.Equals("404", StringComparison.OrdinalIgnoreCase);
#pragma warning restore S2589 // Boolean expressions should not be gratuitous
                    }
                }

                return false;
            }

            return false;
        }
    }
}
