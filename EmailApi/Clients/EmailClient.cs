using System.Net.Http;
using System.Threading.Tasks;
using TGS.EmailApi.Models;
using TGS.EmailApi.Services;

namespace TGS.EmailApi.Clients
{
    public interface IEmailClient
    {
        ValueTask<SendEmailResponse> SendAsync(SendEmailRequest request);
    }

    public class EmailClient : BaseApiClient, IEmailClient
    {
        protected readonly IJsonSerializationProvider _json;

        public EmailClient(HttpClient client, IJsonSerializationProvider json) : base(client, json)
        {
            _json = json;
        }

        ValueTask<SendEmailResponse> IEmailClient.SendAsync(SendEmailRequest request)
        {
            var prefix = $"Tgs.Email";
            return ExecuteJsonRequestAsync<SendEmailRequest, SendEmailResponse>(HttpMethod.Post,"/api/email", request, logPrefix: prefix);
        }
    }
}
