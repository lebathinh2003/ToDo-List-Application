using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Hosting.Server;
using IdentityService.API;

try
{
    var app =  WebApplication.CreateBuilder(args)
                .AddAPIServices()
                .Build()
                .UseAPIServices();

    app.Start();

    var server = app.Services.GetService<IServer>();
    var addresses = server?.Features.Get<IServerAddressesFeature>()?.Addresses;

    if (addresses != null)
    {
        foreach (var address in addresses)
        {
            Console.WriteLine($"API is listening on: {address}");
        }
    }
    else
    {
        Console.WriteLine("Could not retrieve server addresses.");
    }

    app.WaitForShutdown();
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    Console.WriteLine(ex.Message, "Unhandled exception");
}
finally
{
    Console.WriteLine("Shut down complete");
}
