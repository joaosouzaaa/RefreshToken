﻿using RefreshTokenAuthentication.API.Constants;
using RefreshTokenAuthentication.API.Factories;

namespace RefreshTokenAuthentication.API.Factories;

public static class ConnectionStringFactory
{
    public static string GetConnectionString(this IConfiguration configuration)
    {
        if (Environment.GetEnvironmentVariable("DOCKER_ENVIROMENT") is "DockerDevelopment")
        {
            return configuration.GetConnectionString(OptionsConstants.ContainerConnectionSection)!;
        }

        return configuration.GetConnectionString(OptionsConstants.LocalConnectionSection)!;
    }
}
