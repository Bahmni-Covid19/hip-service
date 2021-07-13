namespace In.ProjectEKA.HipService.Link.Model
{
    public class AddCareContextsLink
    {
        private string AccessToken { get; }
        private AddCareContextsPatient Patient { get; }

        public AddCareContextsLink(string accessToken, AddCareContextsPatient addCareContextsPatient)
        {
            AccessToken = accessToken;
            Patient = addCareContextsPatient;
        }
    }   
}