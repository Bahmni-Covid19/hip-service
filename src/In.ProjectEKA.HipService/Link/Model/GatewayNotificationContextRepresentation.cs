using System;
using Newtonsoft.Json;

namespace In.ProjectEKA.HipService.Link.Model
{
    public class GatewayNotificationContextRepresentation
    {
        public Guid RequestId { get; }
        private DateTime Timestamp { get; }
        private NotificationContext NotificationContext { get; }

        public GatewayNotificationContextRepresentation(Guid requestId, DateTime timestamp,
            NotificationContext notificationContext)
        {
            RequestId = requestId;
            Timestamp = timestamp;
            NotificationContext = notificationContext;
        }

        public string dump(Object o)
        {
            return JsonConvert.SerializeObject(o);
        }
    }
}