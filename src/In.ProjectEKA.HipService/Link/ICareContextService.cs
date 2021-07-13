using System;
using In.ProjectEKA.HipLibrary.Patient.Model;

namespace In.ProjectEKA.HipService.Link.Model
{
    public interface ICareContextService
    {
        public Tuple<GatewayAddContextsRequestRepresentation, ErrorRepresentation> AddContextsResponse(
            AddContextsRequest addContextsRequest);

        public Tuple<GatewayNotificationContextRepresentation, ErrorRepresentation> NotificationContextResponse(
            NotifyContextRequest notifyContextRequest);
    }
}