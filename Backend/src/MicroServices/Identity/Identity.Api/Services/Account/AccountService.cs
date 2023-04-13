namespace Identity.Api.Services.Account;

public class AccountService : ServiceBase
{
    [RoutePattern(HttpMethod = "Post")]
    public Task<IResult> Login([FromBody] LoginInputModel model)
    {
        return Task.FromResult(Results.Ok());
    }
}