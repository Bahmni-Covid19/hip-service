using System.Collections.Generic;
using System.Threading.Tasks;
using In.ProjectEKA.HipLibrary.Patient.Model;
using Patient = Hl7.Fhir.Model.Patient;

namespace In.ProjectEKA.HipService.Discovery
{
    public interface IPatientDal
    {
        Task<List<Patient>> LoadPatientsAsync(string name, Gender? gender, ushort? yearOfBirth);
    }
}