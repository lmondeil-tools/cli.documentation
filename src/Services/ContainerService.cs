using Docker.DotNet;
using Docker.DotNet.Models;

using Microsoft.Extensions.Logging;

using System.Runtime.InteropServices;

namespace cli.slndoc.Services;
internal class ContainerService
{
    private readonly ILogger _logger;

    public ContainerService(ILogger logger)
    {
        _logger = logger;
    }

    private DockerClient CreateClient()
    {
        return new DockerClientConfiguration(new Uri(GetClientUri())).CreateClient();
    }

    private string GetClientUri()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return "npipe://./pipe/docker_engine";
        }
        else
        {
            string podmanPath = $"/run/user/{geteuid()}/podman/podman.sock";
            if (File.Exists(podmanPath))
            {
                return $"unix:{podmanPath}";
            }
            return "unix:/var/run/docker.sock";
        }
    }

    [DllImport("libc")]
    internal static extern uint geteuid();

    internal async Task RunAsync(string imageName, string containerName, List<string> command, HostConfig? hostConfig)
    {
        // Pull image
        using DockerClient client = CreateClient();
        await client.Images.CreateImageAsync(new ImagesCreateParameters { FromImage = imageName }, null, new Progress<JSONMessage>());

        // Create the container
        var container = await client.Containers.CreateContainerAsync(new CreateContainerParameters
        {
            Image = imageName,
            Cmd = command,
            Name = containerName,
            HostConfig = hostConfig ?? new HostConfig()
        });

        // Start the container
        try
        {
            await client.Containers.StartContainerAsync(container.ID, new ContainerStartParameters());

            var waitResponse = await client.Containers.WaitContainerAsync(container.ID);
            int exitCode = (int)waitResponse.StatusCode;

            var logStream = await client.Containers.GetContainerLogsAsync(container.ID, false, new ContainerLogsParameters { ShowStdout = true, ShowStderr = true });
            (string stdout, string stderr) = await logStream.ReadOutputToEndAsync(default);

            string output = (exitCode == 0 ? stdout : stderr).Trim();
            _logger?.LogInformation("Container {ContainerName} exited with code {ExitCode} - {Output}", containerName, exitCode, output);
        }
        catch (Exception)
        {

            throw;
        }
        finally
        {
            await client.Containers.RemoveContainerAsync(container.ID, new ContainerRemoveParameters());
        }
    }
}
