using Microsoft.AspNetCore.Mvc;

namespace AppCollect.Api.Infrastructure.ActionResults;

public class InternalServerErrorObjectResult : ObjectResult
{
    public InternalServerErrorObjectResult(object? value) : base(value)
    {
        StatusCode = StatusCodes.Status500InternalServerError;
    }
}