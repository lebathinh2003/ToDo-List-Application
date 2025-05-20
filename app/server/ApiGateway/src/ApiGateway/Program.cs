using ApiGateway;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Hosting.Server;

try {
    var app = WebApplication.CreateBuilder(args)
                 .AddApiGateway()
                 .Build()
                 .UseApiGateway();

    app.Start();

    var server = app.Services.GetService<IServer>();
    var addresses = server?.Features.Get<IServerAddressesFeature>()?.Addresses;

    if (addresses != null)
    {
        foreach (var address in addresses)
        {
            Console.WriteLine($"Chat hub is listening on: {address}");
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
