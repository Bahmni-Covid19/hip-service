namespace In.ProjectEKA.HipServiceTest.OpenMrs {
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Http;
    using System.Net;
    using System.Threading.Tasks;
    using System.Threading;
    using System;
    using FluentAssertions;
    using In.ProjectEKA.HipService.OpenMrs;
    using In.ProjectEKA.HipService.OpenMrs.HealthCheck;
    using In.ProjectEKA.HipService;
    using Microsoft.AspNetCore.Http;
    using Moq.Protected;
    using Moq;
    using Newtonsoft.Json;
    using Xunit;
    using Microsoft.AspNetCore.Hosting;

    [Collection ("Health Check Middleware Tests")]
    public class HealthCheckMiddlewareTest {

        private Mock<IHealthCheckStatus> healthCheckStatus;
        private Mock<IHealthCheckClient> healthCheckClient;
        private HealthCheckMiddleware healthCheckMiddleWare;
        private Mock<IServiceProvider> serviceProvider;

        public HealthCheckMiddlewareTest () {
            Environment.SetEnvironmentVariable("HEALTH_CHECK_DURATION", "5000");
            healthCheckStatus=new Mock<IHealthCheckStatus>();
            serviceProvider=new Mock<IServiceProvider>();
            healthCheckMiddleWare = new HealthCheckMiddleware (async (innerHttpContext) => {
                innerHttpContext.Response.StatusCode = 200;
                await innerHttpContext.Response.WriteAsync ("Success");
            },healthCheckStatus.Object,serviceProvider.Object);
        }

        [Fact]
        private async void ShouldReturnStatus500WithDetailsEvenIfOneServiceIsUnhealthy () {

            var sampleServiceData = new Dictionary<string, string> () {{ "Service1", "Unhealthy" }, { "Service2", "Healthy" } };
            var context = new DefaultHttpContext ();
            context.Response.Body = new MemoryStream ();
            healthCheckClient = new Mock<IHealthCheckClient> ();
            healthCheckClient.Setup (x => x.CheckHealth ())
                .Returns (Task.FromResult (sampleServiceData));
            var expectedResult = JsonConvert.SerializeObject (sampleServiceData);
            
            Dictionary<string,string> unhealthyresponse=new Dictionary<string,string>{{ "Service1", "Unhealthy" }, { "Service2", "Healthy" }};
            healthCheckStatus.Setup(x => x.GetStatus("health"))
                .Returns (unhealthyresponse);

            await healthCheckMiddleWare.Invoke (context);
            context.Response.Body.Seek (0, SeekOrigin.Begin);
            var responseBody = new StreamReader (context.Response.Body).ReadToEnd ();

            responseBody
                .Should ()
                .BeEquivalentTo (expectedResult);
            context.Response.StatusCode
                .Should ()
                .Be (500);
        }

        [Fact]
        private async void ShouldReturnStatus500WithDetailsIfMoreThanOneServicesAreUnhealthy () {

            var sampleServiceData = new Dictionary<string, string> () {{ "Service1", "Unhealthy" }, { "Service2", "Unhealthy" } };
            var context = new DefaultHttpContext ();
            context.Response.Body = new MemoryStream ();
            var expectedResult = JsonConvert.SerializeObject (sampleServiceData);

            healthCheckClient = new Mock<IHealthCheckClient> ();

            healthCheckClient.Setup (x => x.CheckHealth ())
                .Returns (Task.FromResult (sampleServiceData));
            
            Dictionary<string,string> unhealthyresponse=new Dictionary<string,string>{{ "Service1", "Unhealthy" }, { "Service2", "Unhealthy" }};
            healthCheckStatus.Setup(x => x.GetStatus("health"))
                .Returns (unhealthyresponse);

            await healthCheckMiddleWare.Invoke (context);
            context.Response.Body.Seek (0, SeekOrigin.Begin);
            var responseBody = new StreamReader (context.Response.Body).ReadToEnd ();

            responseBody
                .Should ()
                .BeEquivalentTo (expectedResult);
            context.Response.StatusCode
                .Should ()
                .Be (500);
        }

        [Fact]
        private async void ShouldReturnStatus200WithDetailsIfAllTheServicesAreHealthy () {
            var sampleServiceData = new Dictionary<string, string> () { { "Service1", "Healthy" }, { "Service2", "Healthy" } };
            healthCheckClient = new Mock<IHealthCheckClient> ();
            healthCheckClient.Setup (x => x.CheckHealth ())
                .Returns (Task.FromResult (sampleServiceData));
            
            var context = new DefaultHttpContext ();
            context.Response.Body = new MemoryStream ();
            var expectedResult = "Success";

            await healthCheckMiddleWare.Invoke (context);
            context.Response.Body.Seek (0, SeekOrigin.Begin);
            var responseBody = new StreamReader (context.Response.Body).ReadToEnd ();

            responseBody
                .Should ()
                .BeEquivalentTo (expectedResult);
            context.Response.StatusCode
                .Should ()
                .Be (200);
        }
    }
}