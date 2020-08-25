using System;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using FluentAssertions;
using In.ProjectEKA.HipService.OpenMrs;
using Moq;
using Xunit;
using System.IO;
using System.Collections.Generic;

namespace In.ProjectEKA.HipServiceTest.OpenMrs
{

    [Collection("DataFlowRepository Tests")]
    public class OpenMrsDataFlowRepositoryProgramsTest
    {
        private readonly Mock<IOpenMrsClient> openmrsClientMock;
        private readonly OpenMrsDataFlowRepository dataFlowRepository;

        public OpenMrsDataFlowRepositoryProgramsTest()
        {
          openmrsClientMock = new Mock<IOpenMrsClient>();
          dataFlowRepository = new OpenMrsDataFlowRepository(openmrsClientMock.Object);
        }

        [Fact]
        public async Task LoadObservationsForPrograms_ShouldReturnListOfObservations()
        {
            //Given
            string programEnrollmentUuid = "12345678-1234-1234-1234-123456789ABC";

            var path = $"{Endpoints.OpenMrs.OnProgramObservations}{programEnrollmentUuid}";

            var patientProgramsWithObservationsResponse =
                    File.ReadAllText("../../../OpenMrs/sampleData/PatientProgramsWithObservations.json");
            SetupOpenMrsClient(path, patientProgramsWithObservationsResponse);
            var expectedObservations = new Dictionary<string, string> {
                { "15d8c427-9f31-4049-9509-07ab7b599f1b", "Mental Health: Treatment complete, test drug, test chief complaint, test diagnosis" }
                , { "3b319244-4946-44f9-a271-4df830968b93", "Mental Health, Disposition: Treatment complete" }
                , { "f4c20284-3cfb-45d2-8ba5-ac23bfe7299f", "Mental Health, Drugs Provided: test drug" }
                , { "549d33ad-ff78-404c-8021-19a16a6f539b", "Mental Health, Chief Complaint: test chief complaint" }
                , { "995597e2-9daf-45d6-b382-1a7f950c2179", "Mental Health, Diagnosis: test diagnosis" }
            };
            foreach (var obsUuid in expectedObservations.Keys)
            {
                SetupOpenMrsClient(
                    $"{Endpoints.OpenMrs.OnObs}/{obsUuid}",
                    File.ReadAllText($"../../../OpenMrs/sampleData/Observations/Obs_{obsUuid}.json")
                );
            }

            //When
            var observations = await dataFlowRepository.LoadObservationsForPrograms(programEnrollmentUuid);

            //Then
            observations.Should().NotBeNullOrEmpty();
            observations.Should().HaveCount(expectedObservations.Count);
            foreach (var observation in observations)
            {
                expectedObservations.Values.Should().Contain(observation.Display);
            }
            foreach (var expectedObservation in expectedObservations)
            {
                observations.Select(o => o.Display).Should().Contain(expectedObservation.Value);
            }
        }

    //public static IEnumerable<object[]> GetPatientVisitsWithNoObservation()
    //{
    //  var PatientVisitsWithoutVisits = File.ReadAllText("../../../OpenMrs/sampleData/EmptyData.json");
    //  var PatientVisitsWithoutEncounters = File.ReadAllText("../../../OpenMrs/sampleData/PatientVisitsWithoutEncounters.json");
    //  var PatientVisitsWithoutObservation = File.ReadAllText("../../../OpenMrs/sampleData/PatientVisitsWithoutObservation.json");
    //  var PatientVisitsWithoutVisitType = File.ReadAllText("../../../OpenMrs/sampleData/PatientVisitWithoutVisitType.json");


    //  yield return new object[] { PatientVisitsWithoutVisits };
    //  yield return new object[] { PatientVisitsWithoutEncounters };
    //  yield return new object[] { PatientVisitsWithoutObservation };
    //  yield return new object[] { PatientVisitsWithoutVisitType };

    //}

    //[Theory]
    //[MemberData(nameof(GetPatientVisitsWithNoObservation))]
    //public async Task LoadObservationsForVisits_ShouldReturnEmptyList_WhenNoObservationFound(string patientVisits)
    //{
    //  //Given
    //  var patientReferenceNumber = "123";
    //  var path = $"{Endpoints.OpenMrs.OnVisitPath}?patient={patientReferenceNumber}&v=full";

    //  SetupOpenMrsClient(path, patientVisits);

    //  //When
    //  var observations = await dataFlowRepository.LoadObservationsForVisits(patientReferenceNumber, "OPD");

    //  //Then
    //  Assert.Empty(observations);
    //}

    //[Fact]
    //public async Task LoadObservationVisits_ShouldReturnEmptyList_WhenVisitTypeIsIncorect()
    //{
    //  //Given
    //  var patientReferenceNumber = "123";

    //  var path = $"{Endpoints.OpenMrs.OnVisitPath}?patient={patientReferenceNumber}&v=full";
    //  var PatientVisitsWithObservations = File.ReadAllText("../../../OpenMrs/sampleData/PatientVisitWithObservations.json");
    //  SetupOpenMrsClient(path, PatientVisitsWithObservations);

    //  var observations = await dataFlowRepository.LoadObservationsForVisits(patientReferenceNumber, "");

    //  //Then
    //  Assert.Empty(observations);
    //}

    //[Fact]
    //public void LoadObservationsForVisits_ShouldReturnError_WhenNoPatientReferenceNumber()
    //{
    //  //Given
    //  var patientReferenceNumber = string.Empty;

    //  //When
    //  Func<Task> loadObservationsForVisits = async () =>
    //  {
    //    await dataFlowRepository.LoadObservationsForVisits(patientReferenceNumber, "OPD");
    //  };

    //  //Then
    //  loadObservationsForVisits.Should().Throw<OpenMrsFormatException>();
    //}

    //[Fact]
    //public async Task LoadDiagnosticReportForVisits_ShouldReturnVisitDiagnoses()
    //{
    //  //Given
    //  var patientReferenceNumber = "123";

    //  var path = $"{Endpoints.OpenMrs.OnVisitPath}?patient={patientReferenceNumber}&v=full";
    //  var PatientVisitsWithDiagnosis = File.ReadAllText("../../../OpenMrs/sampleData/PatientVisitsWithDiagnosis.json");

    //  SetupOpenMrsClient(path, PatientVisitsWithDiagnosis);

    //  //When
    //  var diagnosis = await dataFlowRepository.LoadDiagnosticReportForVisits(patientReferenceNumber, "OPD");

    //  //Then
    //  var firstDiagnosis = diagnosis[0];
    //  firstDiagnosis.Display.Should().Be("Visit Diagnoses: Primary, Confirmed, Hypertension, unspec., a5e9f749-4bd5-43b4-b5e5-886f0eccc09f, false");
    //}

    //public static IEnumerable<object[]> GetPatientVisitsWithNoDiagnosis()
    //{
    //  var PatientVisitsWithoutVisitType = File.ReadAllText("../../../OpenMrs/sampleData/PatientVisitWithoutVisitType.json");
    //  var PatientVisitsWithoutDiagnosis = File.ReadAllText("../../../OpenMrs/sampleData/PatientVisitsWithoutDiagnosis.json");
    //  var PatientVisitsWithoutVisits = File.ReadAllText("../../../OpenMrs/sampleData/EmptyData.json");
    //  var PatientVisitsWithoutEncounters = File.ReadAllText("../../../OpenMrs/sampleData/PatientVisitsWithoutEncounters.json");
    //  var PatientVisitsWithoutObservation = File.ReadAllText("../../../OpenMrs/sampleData/PatientVisitsWithoutObservation.json");

    //  yield return new object[] { PatientVisitsWithoutVisitType };
    //  yield return new object[] { PatientVisitsWithoutDiagnosis };
    //  yield return new object[] { PatientVisitsWithoutVisits };
    //  yield return new object[] { PatientVisitsWithoutEncounters };
    //  yield return new object[] { PatientVisitsWithoutObservation };
    //}

    //[Theory]
    //[MemberData(nameof(GetPatientVisitsWithNoDiagnosis))]
    //public async Task LoadDiagnosticReportForVisits_ShouldReturnEmptyList_WhenNoDiagnosisFound(string patientVisits)
    //{
    //  //Given
    //  var patientReferenceNumber = "123";

    //  var path = $"{Endpoints.OpenMrs.OnVisitPath}?patient={patientReferenceNumber}&v=full";
    //  SetupOpenMrsClient(path, patientVisits);

    //  var diagnosis = await dataFlowRepository.LoadDiagnosticReportForVisits(patientReferenceNumber, "OPD");

    //  //Then
    //  Assert.Empty(diagnosis);
    //}

    //[Fact]
    //public async Task LoadDiagnosticReportForVisits_ShouldReturnEmptyList_WhenVisitTypeIsIncorect()
    //{
    //  //Given
    //  var patientReferenceNumber = "123";

    //  var path = $"{Endpoints.OpenMrs.OnVisitPath}?patient={patientReferenceNumber}&v=full";
    //  var PatientVisitsWithDiagnosis = File.ReadAllText("../../../OpenMrs/sampleData/PatientVisitsWithDiagnosis.json");
    //  SetupOpenMrsClient(path, PatientVisitsWithDiagnosis);

    //  var diagnosis = await dataFlowRepository.LoadDiagnosticReportForVisits(patientReferenceNumber, "");

    //  //Then
    //  Assert.Empty(diagnosis);
    //}

    //[Fact]
    //public void LoadDiagnosticReportForVisits_ShouldReturnError_WhenNoPatientReferenceNumber()
    //{
    //  //Given
    //  var patientReferenceNumber = string.Empty;

    //  //When
    //  Func<Task> loadDiagnosticReportForVisits = async () =>
    //  {
    //    await dataFlowRepository.LoadDiagnosticReportForVisits(patientReferenceNumber, "OPD");
    //  };

    //  //Then
    //  loadDiagnosticReportForVisits.Should().Throw<OpenMrsFormatException>();
    //}

    //[Fact]
    //public async Task LoadMedicationForVisits_ShouldReturnVisitMedication()
    //{
    //  //Given
    //  var patientReferenceNumber = "123";

    //  var path = $"{Endpoints.OpenMrs.OnVisitPath}?patient={patientReferenceNumber}&v=full";
    //  var PatientVisitsWithMedication = File.ReadAllText("../../../OpenMrs/sampleData/PatientVisitsWithMedication.json");
    //  SetupOpenMrsClient(path, PatientVisitsWithMedication);

    //  //When
    //  var medication = await dataFlowRepository.LoadMedicationForVisits(patientReferenceNumber, "OPD");

    //  //Then
    //  var firstOrder = medication[0];
    //  firstOrder.Display.Should().Be("(NEW) Losartan 50mg: null");
    //}

    //public static IEnumerable<object[]> GetPatientVisitsWithNoOrders()
    //{
    //  var PatientVisitsWithoutVisitType = File.ReadAllText("../../../OpenMrs/sampleData/PatientVisitWithoutVisitType.json");
    //  var PatientVisitsWithoutOrder = File.ReadAllText("../../../OpenMrs/sampleData/PatientVisitsWithoutOrders.json");
    //  var PatientVisitsWithoutVisits = File.ReadAllText("../../../OpenMrs/sampleData/EmptyData.json");
    //  var PatientVisitsWithoutEncounters = File.ReadAllText("../../../OpenMrs/sampleData/PatientVisitsWithoutEncounters.json");

    //  yield return new object[] { PatientVisitsWithoutVisitType };
    //  yield return new object[] { PatientVisitsWithoutOrder };
    //  yield return new object[] { PatientVisitsWithoutVisits };
    //  yield return new object[] { PatientVisitsWithoutEncounters };

    //}

    //[Theory]
    //[MemberData(nameof(GetPatientVisitsWithNoOrders))]
    //public async Task LoadMedicationForVisits_ShouldReturnEmptyList_WhenNoOrdersFound(string patientVisits)
    //{
    //  //Given
    //  var patientReferenceNumber = "123";

    //  var path = $"{Endpoints.OpenMrs.OnVisitPath}?patient={patientReferenceNumber}&v=full";

    //  SetupOpenMrsClient(path, patientVisits);

    //  //When
    //  var medications = await dataFlowRepository.LoadMedicationForVisits(patientReferenceNumber, "OPD");

    //  //Then
    //  Assert.Empty(medications);
    //}

    //[Fact]
    //public async Task LoadMedicationForVisits_ShouldReturnEmptyList_WhenVisitTypeIsIncorect()
    //{
    //  //Given
    //  var patientReferenceNumber = "123";

    //  var path = $"{Endpoints.OpenMrs.OnVisitPath}?patient={patientReferenceNumber}&v=full";
    //  var PatientVisitsWithMedication = File.ReadAllText("../../../OpenMrs/sampleData/PatientVisitsWithMedication.json");
    //  SetupOpenMrsClient(path, PatientVisitsWithMedication);

    //  var medications = await dataFlowRepository.LoadObservationsForVisits(patientReferenceNumber, "");

    //  //Then
    //  Assert.Empty(medications);

    //}

    //[Fact]
    //public void LoadMedicationForVisits_ShouldReturnError_WhenNoPatientReferenceNumber()
    //{
    //  //Given
    //  var patientReferenceNumber = string.Empty;

    //  //When
    //  Func<Task> loadMedicationForVisits = async () =>
    //  {
    //    await dataFlowRepository.LoadMedicationForVisits(patientReferenceNumber, "OPD");
    //  };

    //  //Then
    //  loadMedicationForVisits.Should().Throw<OpenMrsFormatException>();
    //}

    //[Fact]
    //public async Task LoadConditionForVisits_ShouldReturnVisitCondition()
    //{
    //  //Given
    //  var patientReferenceNumber = "123";

    //  var path = $"{Endpoints.EMRAPI.OnConditionPath}?patientUuid={patientReferenceNumber}";
    //  var PatientVisitsWithCondition = File.ReadAllText("../../../OpenMrs/sampleData/PatientVisitsWithCondition.json");


    //  SetupOpenMrsClient(path, PatientVisitsWithCondition);

    //  var conditions = await dataFlowRepository.LoadConditionsForVisit(patientReferenceNumber);

    //  //Then
    //  var firstCondition = conditions[0];
    //  firstCondition.ConditionNonCoded.Should().Be("Former smoker");
    //  firstCondition.OnSetDate.Should().Be(DateTimeOffset.FromUnixTimeMilliseconds(1597602600000).UtcDateTime);
    //}

    //[Fact]
    //public async Task LoadConditionForVisits__ShouldReturnEmptyList_WhenNoConditionsFound()
    //{
    //  //Given
    //  var patientReferenceNumber = "123";

    //  var path = $"{Endpoints.EMRAPI.OnConditionPath}?patientUuid={patientReferenceNumber}";
    //  var PatientVisitsNoCondition = File.ReadAllText("../../../OpenMrs/sampleData/PatientVisitWithNoCondition.json");
    //  SetupOpenMrsClient(path, PatientVisitsNoCondition);

    //  var conditions = await dataFlowRepository.LoadConditionsForVisit(patientReferenceNumber);

    //  //Then
    //  Assert.Empty(conditions);
    //}

    //[Fact]
    //public void LoadConditionsForVisit_ShouldReturnError_WhenNoPatientReferenceNumber()
    //{
    //  //Given
    //  var patientReferenceNumber = string.Empty;

    //  //When
    //  Func<Task> loadConditionsForVisit = async () =>
    //   {
    //     await dataFlowRepository.LoadConditionsForVisit(patientReferenceNumber);
    //   };

    //  //Then
    //  loadConditionsForVisit.Should().Throw<OpenMrsFormatException>();
    //}

    //[Fact]
    //public async Task LoadConditionForVisits_ShouldReturnVisitCodedCondition()
    //{
    //  //Given
    //  var patientReferenceNumber = "123";
    //  var path = $"{Endpoints.EMRAPI.OnConditionPath}?patientUuid={patientReferenceNumber}";
    //  var PatientVisitsWithCondition = File.ReadAllText("../../../OpenMrs/sampleData/PatientVisitWithCodedCondition.json");


    //  SetupOpenMrsClient(path, PatientVisitsWithCondition);

    //  var conditions = await dataFlowRepository.LoadConditionsForVisit(patientReferenceNumber);

    //  //Then
    //  var firstCondition = conditions[0];
    //  firstCondition.ConditionNonCoded.Should().Be(null);
    //}

        private void SetupOpenMrsClient(string path, string response)
        {
            openmrsClientMock
                .Setup(x => x.GetAsync(path))
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(response)
                })
                .Verifiable();
        }
    }
}
