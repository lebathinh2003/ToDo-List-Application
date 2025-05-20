using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Hosting.Server;
using UserService.API;

try
{
    var app = await WebApplication.CreateBuilder(args)
                .AddAPIServices()
                .Build()
                .UseAPIServicesAsync();

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
