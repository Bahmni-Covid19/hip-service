using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using In.ProjectEKA.HipService.OpenMrs;
using Moq;
using Moq.Protected;
using Xunit;
namespace In.ProjectEKA.HipServiceTest.OpenMrs
{
    [Collection("OpenMrs Gateway Client Tests")]
    public class OpenMrsclientTest
    {
        [Fact]
        public async Task ShouldPropagateStatusWhenCredentialsFailed()
        {
            //Given
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            var httpClient = new HttpClient(handlerMock.Object);
            var openmrsConfiguration = new OpenMrsConfiguration
            {
                Url = "https://someurl/openmrs/",
                Username = "someusername",
                Password = "somepassword"
            };
            var openmrsClient = new OpenMrsClient(httpClient, openmrsConfiguration);
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync( new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest
                } )
                .Verifiable();

            //When
            var response = await openmrsClient.GetAsync("path/to/resource");

            //then
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task ShouldThrowExceptionAndLogIfAnyExceptionIsThrown()
        {
            //Given
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            var httpClient = new HttpClient(handlerMock.Object);
            var openmrsConfiguration = new OpenMrsConfiguration
            {
                Url = "https://someurl/openmrs/",
                Username = "someusername",
                Password = "somepassword"
            };
            var openmrsClient = new OpenMrsClient(httpClient, openmrsConfiguration);
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ThrowsAsync(new Exception("some message here"))
                .Verifiable();

            //When
            Func<Task> getAsyncMethod = async () => { await openmrsClient.GetAsync("path/to/resource"); };
            //Then
            getAsyncMethod.Should().Throw<Exception>();
        }

        [Theory]
        [InlineData("path/to/resource")]
        [InlineData("/path/to/resource")]
        public async Task ShouldInterrogateTheRightDataSource(string patientDiscoveryPath)
        {
            //Given
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Loose);
            var httpClient = new HttpClient(handlerMock.Object);
            var openmrsConfiguration = new OpenMrsConfiguration
            {
                Url = "https://someurl/openmrs/",
                Username = "someusername",
                Password = "somepassword"
            };
            var openmrsClient = new OpenMrsClient(httpClient, openmrsConfiguration);
            var wasCalledWithTheRightUri = false;
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                    .Callback<HttpRequestMessage, CancellationToken>((response, token) =>
                    {
                        if (response.RequestUri.AbsoluteUri == "https://someurl/openmrs/path/to/resource" ||
                        response.RequestUri.AbsoluteUri == "https://someurl/openmrs//path/to/resource")
                        {
                        wasCalledWithTheRightUri = true;
                        }
                    })
                    .ReturnsAsync(new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK
                    })
                    .Verifiable();

                //When
                await openmrsClient.GetAsync(patientDiscoveryPath);
                wasCalledWithTheRightUri.Should().BeTrue();
        }

    }
}