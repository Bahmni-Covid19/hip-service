using System;
using System.Collections.Generic;
using System.Net.Http;
using Hl7.Fhir.Model;
using In.ProjectEKA.HipLibrary.Patient.Model;
using In.ProjectEKA.HipService.Common.Model;
using In.ProjectEKA.HipService.Gateway;
using In.ProjectEKA.HipService.Link;
using In.ProjectEKA.HipService.Link.Model;
using In.ProjectEKA.HipService.UserAuth.Model;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using static In.ProjectEKA.HipService.Common.Constants;
using Patient = In.ProjectEKA.HipService.Link.Model.Patient;
using Task = System.Threading.Tasks.Task;


namespace In.ProjectEKA.HipServiceTest.Link
{
    public class CareContextControllerTest
    {
        private readonly CareContextController careContextController;

        private readonly Mock<ILogger<CareContextController>> logger =
            new Mock<ILogger<CareContextController>>();

        private readonly Mock<GatewayClient> gatewayClient = new Mock<GatewayClient>(MockBehavior.Strict, null, null);
        private readonly Mock<ICareContextService> careContextService = new Mock<ICareContextService>();

        private readonly GatewayConfiguration gatewayConfiguration = new GatewayConfiguration()
        {
            CmSuffix = "sbx"
        };

        private readonly HttpClient httpClient;

        public CareContextControllerTest()
        {
            careContextController =
                new CareContextController(gatewayClient.Object,
                    logger.Object, gatewayConfiguration, careContextService.Object);
        }

        [Fact]
        private void shouldAddContext()
        {
            var timeStamp = DateTime.Now.ToUniversalTime();
            var requestId = Guid.NewGuid();
            var cmSuffix = "sbx";
            var careContextRepresentation = new CareContextRepresentation("anc", "xyz");
            var careContexts = new List<CareContextRepresentation>();
            careContexts.Add(careContextRepresentation);
            var addCareContextPatient = new AddCareContextsPatient("1234", "qwqwqw", careContexts);
            var careContextLink = new AddCareContextsLink("1222", addCareContextPatient);
            var addContextsAcknowledgement = new AddContextsAcknowledgement("success");
            var error = new Error(ErrorCode.GatewayTimedOut, "Gateway timed out");
            var resp = new Resp("123");
            var correlationId = Uuid.Generate().ToString();

            var gatewayAddContextsRequestRepresentation =
                new GatewayAddContextsRequestRepresentation(requestId, timeStamp, careContextLink);

            var onAddContextRequest =
                new HipLinkContextConfirmation(requestId.ToString(), timeStamp, addContextsAcknowledgement, error,
                    resp);
            var addContextRequest = new AddContextsRequest("123", "abc", careContexts, "pqr");

            careContextService.Setup(a => a.AddContextsResponse(addContextRequest))
                .Returns(new Tuple<GatewayAddContextsRequestRepresentation, ErrorRepresentation>
                    (gatewayAddContextsRequestRepresentation, null));

            gatewayClient.Setup(
                    client =>
                        client.SendDataToGateway(PATH_ADD_PATIENT_CONTEXTS,
                            gatewayAddContextsRequestRepresentation, cmSuffix, correlationId))
                .Returns(Task.CompletedTask)
                .Callback<string, GatewayAddContextsRequestRepresentation, string, string>
                ((path, gr, suffix, corId)
                    => careContextController.Accepted(onAddContextRequest));
        }

        [Fact]
        private void shouldNotify()
        {
            var timeStamp = DateTime.Now.ToUniversalTime();
            var requestId = Guid.NewGuid();
            var error = new Error(ErrorCode.GatewayTimedOut, "Gateway timed out");
            var resp = new Resp("123");
            var addContextsAcknowledgement = new AddContextsAcknowledgement("success");

            var hiTypes = new List<string>();
            hiTypes.Add("Medication");
            var notifyContextRequest = new NotifyContextRequest("123", "swjs", "wew", hiTypes, new DateTime(), "456");
            var onNotifyContextRequest =
                new HipLinkContextConfirmation(requestId.ToString(), timeStamp, addContextsAcknowledgement, error,
                    resp);
            var patient = new Patient("12");
            var notificationCareContext = new NotificationCareContext("abc", "qqwq");
            var hipReference = new HIPReference("1212");
            var gatewayNotificationContextsRequestRepresentation =
                new GatewayNotificationContextRepresentation(requestId, timeStamp,
                    new NotificationContext(patient, notificationCareContext, hiTypes, new DateTime(), hipReference));

            careContextService.Setup(a => a.NotificationContextResponse(notifyContextRequest))
                .Returns(new Tuple<GatewayNotificationContextRepresentation, ErrorRepresentation>
                    (gatewayNotificationContextsRequestRepresentation, null));

            var cmSuffix = "sbx";
            var correlationId = Uuid.Generate().ToString();


            gatewayClient.Setup(
                    client =>
                        client.SendDataToGateway(PATH_NOTIFY_PATIENT_CONTEXTS,
                            gatewayNotificationContextsRequestRepresentation, cmSuffix, correlationId))
                .Returns(Task.CompletedTask)
                .Callback<string, GatewayNotificationContextRepresentation, string, string>
                ((path, gr, suffix, corId)
                    => careContextController.Accepted(onNotifyContextRequest));
        }
    }
}