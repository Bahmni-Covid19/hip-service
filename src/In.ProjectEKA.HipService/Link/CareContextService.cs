using System;
using In.ProjectEKA.HipLibrary.Patient.Model;
using In.ProjectEKA.HipService.Common.Model;

namespace In.ProjectEKA.HipService.Link.Model
{
    public class CareContextService : ICareContextService
    {
        public Tuple<GatewayAddContextsRequestRepresentation, ErrorRepresentation> AddContextsResponse(
            AddContextsRequest addContextsRequest)
        {
            var accessToken = addContextsRequest.AccessToken;
            var referenceNumber = addContextsRequest.ReferenceNumber;
            var careContexts = addContextsRequest.CareContexts;
            var display = addContextsRequest.Display;
            var patient = new AddCareContextsPatient(referenceNumber, display, careContexts);
            var link = new AddCareContextsLink(accessToken, patient);
            var timeStamp = DateTime.Now.ToUniversalTime();
            var requestId = Guid.NewGuid();
            return new Tuple<GatewayAddContextsRequestRepresentation, ErrorRepresentation>
                (new GatewayAddContextsRequestRepresentation(requestId, timeStamp, link), null);
        }

        public Tuple<GatewayNotificationContextRepresentation, ErrorRepresentation> NotificationContextResponse(
            NotifyContextRequest notifyContextRequest)
        {
            var id = notifyContextRequest.PatientId;
            var patientReference = notifyContextRequest.PatientReference;
            var careContextReference = notifyContextRequest.CareContextReference;
            var hiTypes = notifyContextRequest.HiTypes;
            var hipId = notifyContextRequest.HipId;
            var patient = new Patient(id);
            var careContext = new NotificationCareContext(patientReference, careContextReference);
            var hip = new HIPReference(hipId);
            var date = notifyContextRequest.Date;
            var timeStamp = DateTime.Now.ToUniversalTime();
            var requestId = Guid.NewGuid();
            var notification = new NotificationContext(patient, careContext, hiTypes, date, hip);
            return new Tuple<GatewayNotificationContextRepresentation, ErrorRepresentation>
                (new GatewayNotificationContextRepresentation(requestId, timeStamp, notification), null);
        }
    }
}