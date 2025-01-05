using System.Diagnostics;

namespace Falc.Communications.Api.Tooling;

public static class DockerHelpers
{
    /// <summary>
    /// TODO: move to docker compose file.
    /// </summary>
    public static void UpdateCaCertificates() => "update-ca-certificates".Bash();
    
    private static string Bash(this string cmd)
    {
        var str = cmd.Replace("\"", "\\\"");
        if (!File.Exists("/bin/bash"))
            return string.Empty;
        var process = new Process();
        process.StartInfo = new ProcessStartInfo()
        {
            FileName = "/bin/bash",
            Arguments = "-c \"" + str + "\"",
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        process.Start();
        var end = process.StandardOutput.ReadToEnd();
        process.WaitForExit();
        
        return end;
    }
}