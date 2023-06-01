using System.Diagnostics;
using System.Threading.Tasks;

namespace TGS.Core.Infrastructures.Mail
{
    public interface IEmailManager
    {
        Task SendEmailAsync(EmailPackage message, ActivitySource source);
    }
}
