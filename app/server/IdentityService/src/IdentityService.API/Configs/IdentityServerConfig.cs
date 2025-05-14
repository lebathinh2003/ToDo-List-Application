using Duende.IdentityServer.Models;
using Microsoft.Extensions.Configuration;
namespace IdentityService.API.Configs;

public static class IdentityServerInMemoryConfig
{
    // InMemory Clients
    public static IEnumerable<Client> GetClients() =>
        new List<Client>
        {
        new Client
        {
            ClientId = "my-client",
            AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,

            ClientSecrets = { new Secret("secret".Sha256()) },

            AllowedScopes = {"api1", "openid", "profile", "roles"}, 
        }
        };


    // InMemory IdentityResources
    public static IEnumerable<IdentityResource> GetIdentityResources() =>
        new List<IdentityResource>
        {
        new IdentityResources.OpenId(),
        new IdentityResource("roles", "Your roles", new[] { "role" })
        };
}
