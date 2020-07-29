using System.Linq;
using System.Net;
using System.Net.Http;
using FluentAssertions;
using In.ProjectEKA.HipService.OpenMrs;
using Moq;
using Xunit;
using In.ProjectEKA.HipLibrary.Patient.Model;
using System.Collections.Generic;


namespace In.ProjectEKA.HipServiceTest.OpenMrs
{
    public static class ExpectedOpenMrsDiscoveryPathConstants
    {
        public const string OnProgramEnrollmentPath = "ws/rest/v1/bahmniprogramenrollment";
        public const string OnVisitPath = "ws/rest/v1/visit";
    }

    [Collection("OpenMrs Discovery Data Source Tests")]
    public class OpenMrsDiscoveryDataSourceTest
    {
        [Fact]
        public async System.Threading.Tasks.Task ShouldReturnListOfProgramEnrollments()
        {
            //Given
            var openmrsClientMock = new Mock<IOpenMrsClient>();
            var discoveryDataSource = new OpenMrsDiscoveryDataSource(openmrsClientMock.Object);

            openmrsClientMock
                .Setup(x => x.GetAsync(ExpectedOpenMrsDiscoveryPathConstants.OnProgramEnrollmentPath))
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(ProgramEnrollmentSample)
                })
                .Verifiable();

            //When
            var programenrollments = await discoveryDataSource.LoadProgramEnrollments(null);

            //Then
            var program = programenrollments[0];
            program.ReferenceNumber.Should().Be("12345");
            program.Display.Should().Be("HIV Program");
        }

        [Fact]
        public async System.Threading.Tasks.Task ShouldReturnListOfVisits()
        {
            //Given
            var openmrsClientMock = new Mock<IOpenMrsClient>();
            var discoveryDataSource = new OpenMrsDiscoveryDataSource(openmrsClientMock.Object);
            openmrsClientMock
                .Setup(x => x.GetAsync(ExpectedOpenMrsDiscoveryPathConstants.OnVisitPath))
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(VisitSample)
                })
                .Verifiable();

            //When
            var visits = await discoveryDataSource.LoadVisits(null);

            //Then
            var visit = visits[0];
            visit.Display.Should().Be("OPD");
        }

        [Fact]
        public async System.Threading.Tasks.Task ShouldReturnListOfVisitsGroupedByType()
        {
            //Given
            var openmrsClientMock = new Mock<IOpenMrsClient>();
            var discoveryDataSource = new OpenMrsDiscoveryDataSource(openmrsClientMock.Object);
            openmrsClientMock
                .Setup(x => x.GetAsync(ExpectedOpenMrsDiscoveryPathConstants.OnVisitPath))
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(VisitSample)
                })
                .Verifiable();

            //When
            var visits = await discoveryDataSource.LoadVisits(null);

            //Then
            var numberOfVisitTypes = visits.Count();
            numberOfVisitTypes.Should().Be(2);
            var firstVisitType = visits[0];
            firstVisitType.Display.Should().Be("OPD");
            var secondVisitType = visits[1];
            secondVisitType.Display.Should().Be("Emergency");
        }

        [Fact]
        public async System.Threading.Tasks.Task ShouldReturnCombinedListOfCareContexts()
        {
            //Given
            var openmrsClientMock = new Mock<IOpenMrsClient>();
            Mock<OpenMrsDiscoveryDataSource> discoveryDataSource = new Mock<OpenMrsDiscoveryDataSource>(openmrsClientMock.Object);
            discoveryDataSource.CallBase = true;
            discoveryDataSource
                .Setup(x => x.LoadProgramEnrollments(It.IsAny<string>()))
                .ReturnsAsync(
                    new List<CareContextRepresentation>
                    {
                        new CareContextRepresentation("12345", "HIV Program")
                    })
                .Verifiable();


            discoveryDataSource
                .Setup(x => x.LoadVisits(It.IsAny<string>()))
                .ReturnsAsync(
                    new List<CareContextRepresentation>
                    {
                        new CareContextRepresentation(null, "OPD"),
                        new CareContextRepresentation(null, "Emergency")
                    })
                .Verifiable();

            //When
            var combinedCareContexts = await discoveryDataSource.Object.LoadCombinedCareContexts(null);

            //Then
            combinedCareContexts.Count().Should().Be(3);
            combinedCareContexts[0].ReferenceNumber.Should().Be("12345");
            combinedCareContexts[0].Display.Should().Be("HIV Program");

            combinedCareContexts[1].Display.Should().Be("OPD");
            combinedCareContexts[2].Display.Should().Be("Emergency");
        }


        private const string ProgramEnrollmentSample = @"{
            ""results"": [
                {
                    ""uuid"": ""c1720ca0-8ea3-4ef7-a4fa-a7849ab99d87"",
                    ""patient"": {
                        ""uuid"": ""b5e712bc-9472-41c0-a11f-500deac452d2"",
                        ""display"": ""1234 - John Doe"",
                        ""identifiers"": [
                            {
                                ""uuid"": ""af8996cb-e94b-463c-9ba0-17630dc12e0b"",
                                ""display"": ""Patient Identifier = 1234"",
                                ""links"": [
                                    {
                                        ""rel"": ""self"",
                                        ""uri"": ""http://192.168.33.10/openmrs/ws/rest/v1/patient/b5e712bc-9472-41c0-a11f-500deac452d2/identifier/af8996cb-e94b-463c-9ba0-17630dc12e0b""
                                    }
                                ]
                            }
                        ]
                    },
                    ""program"": {
                        ""name"": ""HIV Program"",
                        ""uuid"": ""5789a170-c020-4879-ae39-06b1de26cb5f"",
                        ""retired"": false,
                        ""description"": ""HIV Program"",
                        ""concept"": {
                            ""uuid"": ""ec41264d-e82e-4356-a7d2-7c1ff6c90abe"",
                            ""display"": ""HIV"",
                            ""links"": [
                                {
                                    ""rel"": ""self"",
                                    ""uri"": ""http://192.168.33.10/openmrs/ws/rest/v1/concept/ec41264d-e82e-4356-a7d2-7c1ff6c90abe""
                                }
                            ]
                        }
                    },
                    ""display"": ""HIV Program"",
                    ""dateEnrolled"": ""2020-07-13T14:00:00.000+0000"",
                    ""dateCompleted"": null,
                    ""location"": null,
                    ""voided"": false,
                    ""outcome"": null,
                    ""attributes"": [
                        {
                            ""display"": ""ID_Number: 12345"",
                            ""uuid"": ""11d5bc55-b94c-480c-ac0b-e5c9a7e40c20"",
                            ""attributeType"": {
                                ""uuid"": ""c41f844e-a707-11e6-91e9-0800270d80ce"",
                                ""display"": ""ID_Number"",
                                ""description"": ""ID Number"",
                                ""retired"": false,
                                ""links"": [
                                    {
                                        ""rel"": ""self"",
                                        ""uri"": ""http://192.168.33.10/openmrs/ws/rest/v1/programattributetype/c41f844e-a707-11e6-91e9-0800270d80ce""
                                    }
                                ]
                            },
                            ""value"": ""12345"",
                            ""voided"": false,
                            ""links"": [
                                {
                                    ""rel"": ""self"",
                                    ""uri"": ""http://192.168.33.10/openmrs/ws/rest/v1/bahmniprogramenrollment/57f76d2d-358e-4135-8160-247a94c49535/attribute/11d5bc55-b94c-480c-ac0b-e5c9a7e40c20""
                                },
                                {
                                    ""rel"": ""full"",
                                    ""uri"": ""http://192.168.33.10/openmrs/ws/rest/v1/bahmniprogramenrollment/57f76d2d-358e-4135-8160-247a94c49535/attribute/11d5bc55-b94c-480c-ac0b-e5c9a7e40c20?v=full""
                                }
                            ],
                            ""resourceVersion"": ""1.9""
                        }
                    ],
                    ""links"": [
                        {
                            ""rel"": ""self"",
                            ""uri"": ""http://192.168.33.10/openmrs/ws/rest/v1/bahmniprogramenrollment/c1720ca0-8ea3-4ef7-a4fa-a7849ab99d87""
                        }
                    ],
                    ""resourceVersion"": ""1.8""
                }
            ]
        }";

        private const string VisitSample = @"{
            ""results"": [
                {
                    ""uuid"": ""fd377423-c804-4df2-b340-1ef844206769"",
                    ""display"": ""OPD @ Odisha - 07/22/2020 11:28 AM"",
                    ""patient"": {
                        ""uuid"": ""2ceb3abb-1724-4f4f-b969-43b63f73545e"",
                        ""display"": ""OD100012 - Anshul Test One"",
                        ""links"": [
                            {
                                ""rel"": ""self"",
                                ""uri"": ""http://bahmni-0.92.bahmni-covid19.in/openmrs/ws/rest/v1/patient/2ceb3abb-1724-4f4f-b969-43b63f73545e""
                            }
                        ]
                    },
                    ""visitType"": {
                        ""uuid"": ""96c49059-9af1-4f63-a8be-7554984fda02"",
                        ""display"": ""OPD"",
                        ""links"": [
                            {
                                ""rel"": ""self"",
                                ""uri"": ""http://bahmni-0.92.bahmni-covid19.in/openmrs/ws/rest/v1/visittype/96c49059-9af1-4f63-a8be-7554984fda02""
                            }
                        ]
                    }
                },
                {
                    ""uuid"": ""fd377423-c804-4df2-b340-1ef844206769"",
                    ""display"": ""OPD @ Odisha - 07/22/2020 11:28 AM"",
                    ""patient"": {
                        ""uuid"": ""2ceb3abb-1724-4f4f-b969-43b63f73545e"",
                        ""display"": ""OD100012 - Anshul Test One"",
                        ""links"": [
                            {
                                ""rel"": ""self"",
                                ""uri"": ""http://bahmni-0.92.bahmni-covid19.in/openmrs/ws/rest/v1/patient/2ceb3abb-1724-4f4f-b969-43b63f73545e""
                            }
                        ]
                    },
                    ""visitType"": {
                        ""uuid"": ""96c49059-9af1-4f63-a8be-7554984fda02"",
                        ""display"": ""OPD"",
                        ""links"": [
                            {
                                ""rel"": ""self"",
                                ""uri"": ""http://bahmni-0.92.bahmni-covid19.in/openmrs/ws/rest/v1/visittype/96c49059-9af1-4f63-a8be-7554984fda02""
                            }
                        ]
                    }
                },
                {
                    ""uuid"": ""fd377423-c804-4df2-b340-1ef844206769"",
                    ""display"": ""OPD @ Odisha - 07/22/2020 11:28 AM"",
                    ""patient"": {
                        ""uuid"": ""2ceb3abb-1724-4f4f-b969-43b63f73545e"",
                        ""display"": ""OD100012 - Anshul Test One"",
                        ""links"": [
                            {
                                ""rel"": ""self"",
                                ""uri"": ""http://bahmni-0.92.bahmni-covid19.in/openmrs/ws/rest/v1/patient/2ceb3abb-1724-4f4f-b969-43b63f73545e""
                            }
                        ]
                    },
                    ""visitType"": {
                        ""uuid"": ""96c49059-9af1-4f63-a8be-7554984fda02"",
                        ""display"": ""Emergency"",
                        ""links"": [
                            {
                                ""rel"": ""self"",
                                ""uri"": ""http://bahmni-0.92.bahmni-covid19.in/openmrs/ws/rest/v1/visittype/96c49059-9af1-4f63-a8be-7554984fda02""
                            }
                        ]
                    }
                }
            ]
        }";
    }
}