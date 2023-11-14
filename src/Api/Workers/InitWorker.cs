
using CloudDrive.Persistence;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace CloudDrive.Api.Workers
{
	public class InitWorker : IHostedService
	{
		private readonly IServiceProvider _services;

		public InitWorker(IServiceProvider services)
		{
			_services = services;
		}

		public async Task StartAsync(CancellationToken cancellationToken)
		{
			var scope = _services.CreateScope();

			var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

			if (await manager.FindByClientIdAsync("clouddrive") == null)
			{
				await manager.CreateAsync(new OpenIddictApplicationDescriptor
				{
					ClientId = "clouddrive",
					ClientSecret = "901564A5-E7FE-42CB-B10D-61EF6A8F3654",
					ConsentType = ConsentTypes.Implicit,
					DisplayName = "Cloud drive",
					RedirectUris =
					{
						new Uri("https://localhost:44338/callback/login/local")
					},
					PostLogoutRedirectUris =
					{
						new Uri("https://localhost:44338/callback/logout/local")
					},
					Permissions =
					{
						Permissions.Endpoints.Authorization,
						Permissions.Endpoints.Logout,
						Permissions.Endpoints.Token,
						Permissions.GrantTypes.AuthorizationCode,
						Permissions.GrantTypes.Password,
						Permissions.GrantTypes.RefreshToken,
						Permissions.ResponseTypes.Code,
						Permissions.Scopes.Email,
						Permissions.Scopes.Profile,
						Permissions.Scopes.Roles
					},
					Requirements =
					{
						Requirements.Features.ProofKeyForCodeExchange
					}
				});
			}
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			return Task.CompletedTask;
		}
	}
}