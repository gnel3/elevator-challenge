using Elevator.Challenge.Application.Interfaces;
using Elevator.Challenge.Application.Services;
using Elevator.Challenge.Application.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Elevator.Challenge.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        // Binds the "ElevatorSettings" section from appsettings.json
        services.Configure<ElevatorSettings>(configuration.GetSection(nameof(ElevatorSettings)));
        services.AddSingleton<IElevatorService, ElevatorService>();

        return services;
    }
}