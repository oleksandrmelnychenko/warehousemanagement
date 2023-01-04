using Microsoft.Identity.Client;

namespace TestApp.Models
{
    public sealed class AuthenticationMessage
    {
        public AuthenticationResult AuthenticationResult { get; set; }

        public AuthenticationMessage()
        {
            AuthenticationResult = new AuthenticationResult("", true, "", DateTimeOffset.Now, DateTimeOffset.Now, "", null, "", new string[] { }, Guid.NewGuid());
        }

        public AuthenticationMessage(AuthenticationResult result)
        {
            AuthenticationResult = result;
        }
    }
}
