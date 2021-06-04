using System;
using System.Net.Http;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.Json;

namespace TGS.Core.Infrastructures.Mail
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
