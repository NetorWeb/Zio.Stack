using Autofac;

namespace AppCollect.Api.Infrastructure.AutofacModules;

public class ApplicationModule : Autofac.Module
{
    public string QueriesConnectionString { get; }

    public ApplicationModule(string qconstr)
    {
        QueriesConnectionString = qconstr;
    }

    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);
    }
}