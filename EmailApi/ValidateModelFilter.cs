using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TGS.EmailApi
{
    public sealed class ValidateModelFilter : IAsyncActionFilter
    {
        async Task IAsyncActionFilter.OnActionExecutionAsync(ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            if (context.ModelState.IsValid)
            {
                await next();
                return;
            }

            var errors = context.ModelState.Values.SelectMany(mse => mse.Errors.Select(e =>
                string.Join(";", string.IsNullOrEmpty(e.ErrorMessage) ? e.Exception?.Message ?? string.Empty : e.ErrorMessage)));

            throw new ModelBindingException($"Error binding model: {errors}");
        }
    }
}
