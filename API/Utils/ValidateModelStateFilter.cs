using AutoWrapper.Extensions;
using Domain.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Utils;
public class ValidateModelStateFilter : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            throw new BadRequestException(context.ModelState.AllErrors());
        }
    }
}
