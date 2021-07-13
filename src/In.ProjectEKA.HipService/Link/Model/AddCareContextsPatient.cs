using System.Collections.Generic;
using In.ProjectEKA.HipLibrary.Patient.Model;

namespace In.ProjectEKA.HipService.Link.Model
{
    public class AddCareContextsPatient
    {
        private string ReferenceNumber { get; }
        private string Display { get; }
        private List<CareContextRepresentation> CareContexts { get; }

        public AddCareContextsPatient(string referenceNumber, string display, List<CareContextRepresentation> careContexts)
        {
            ReferenceNumber = referenceNumber;
            Display = display;
            CareContexts = careContexts;
        }
    }
}