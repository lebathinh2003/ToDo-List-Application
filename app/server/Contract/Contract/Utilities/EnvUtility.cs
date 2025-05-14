﻿namespace Contract.Utilities;

public class EnvUtility
{
    public static void LoadEnvFile()
    {
        var rootEnvPath = GetEnvFilePath("ToDo-List-Application", ".env");
        if (string.IsNullOrEmpty(rootEnvPath))
        {
            return;
        }

        string solutionPath = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.FullName ?? "";

        if (IsProduction())
        {
            DotNetEnv.Env.Load(rootEnvPath);
            DotNetEnv.Env.Load(Path.Combine(solutionPath, ".env.production"));
        }
        else if (IsDevelopment())
        {
            LoadEnvWithoutOverriding(rootEnvPath);
            LoadEnvWithoutOverriding(Path.Combine(solutionPath, ".env"));
        }
    }
    public static bool IsDevelopment()
    {
        return DotNetEnv.Env.GetString("ASPNETCORE_ENVIRONMENT", "") == "Development";
    }
    public static bool IsProduction()
    {
        return DotNetEnv.Env.GetString("ASPNETCORE_ENVIRONMENT", "") == "Production";
    }

    /// <summary>
    /// Get the postgresql connection string from env
    /// </summary>
    /// <returns></returns>
    public static string GetConnectionString()
    {
        LoadEnvFile();
        var host = DotNetEnv.Env.GetString("DB_HOST", "localhost").Trim();
        var port = DotNetEnv.Env.GetString("DB_PORT", "1433").Trim();
        var db = DotNetEnv.Env.GetString("DB", "Not found").Trim();
        var user = DotNetEnv.Env.GetString("SQLSERVER_USER", "Not found").Trim();
        var pwd = DotNetEnv.Env.GetString("SQLSERVER_PASSWORD", "Not found").Trim();
        var connectionString = $"Server={host},{port};Database={db};User Id={user};Password={pwd};TrustServerCertificate=True;";
        return connectionString;
    }

    /// <summary>
    /// </summary>
    /// <param name="folderName"></param>
    /// <param name="envFileName"></param>
    /// <returns>absolute env file path, if not found return null</returns>
    private static string? GetEnvFilePath(string folderName, string envFileName)
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        int rootIndex = currentDirectory.IndexOf(folderName);
        if (rootIndex != -1)
        {
            return Path.Combine(currentDirectory.Substring(0, rootIndex + folderName.Length), envFileName);
        }
        return null;
    }


    /// <summary>
    ///     Intended for use in the development environment only.
    /// </summary>
    /// <remarks>
    /// WARNING: This method is designed for running all service scripts during development. 
    /// Using it in production may lead to unexpected behavior.
    /// </remarks>

    private static void LoadEnvWithoutOverriding(string envPath)
    {
        if (!File.Exists(envPath))
        {
            Console.WriteLine("The .env file does not exist at the specified path.");
            return;
        }

        var envVariables = ParseEnvFile(envPath);

        foreach (var (key, value) in envVariables)
        {
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(key)))
            {
                Environment.SetEnvironmentVariable(key, value);
            }
        }
    }

    private static Dictionary<string, string> ParseEnvFile(string envPath)
    {
        var envVariables = new Dictionary<string, string>();

        foreach (var line in File.ReadLines(envPath))
        {
            // Skip comments and empty lines
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                continue;

            var parts = line.Split(['='], 2);
            if (parts.Length == 2)
            {
                var key = parts[0].Trim();
                var value = parts[1].Trim();

                if (!envVariables.ContainsKey(key))
                {
                    envVariables.Add(key, value);
                }
            }
        }

        return envVariables;
    }

}
