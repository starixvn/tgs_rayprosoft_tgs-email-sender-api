using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
namespace tgs_emailapi_serverless;

public sealed class ModelBindingException : Exception
{
    public ModelBindingException(string message) : base(message)
    {
    }
}