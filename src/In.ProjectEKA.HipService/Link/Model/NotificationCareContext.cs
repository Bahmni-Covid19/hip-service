namespace In.ProjectEKA.HipService.Link.Model
{
    public class NotificationCareContext
    {
        public string PatientReference { get; }
        public string CareContextReference { get; }

        public NotificationCareContext(string patientReference, string careContextReference)
        {
            PatientReference = patientReference;
            CareContextReference = careContextReference;
        }
    }
}