using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;

public class NgrokService
{
    private readonly HttpClient _http = new HttpClient();
    private Process? _ngrokProcess;

    public string? PublicUrl { get; private set; }

    /// <summary>
    /// Starts ngrok on the given local port and waits until the tunnel is established.
    /// </summary>
    public async Task StartAsync(int localPort)
    {
        // Start ngrok process
        _ngrokProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = @"C:\ProgramData\chocolatey\bin\ngrok.exe",
                Arguments = $"http {localPort} --log=stdout",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        _ngrokProcess.Start();

        // Optional: log ngrok output for debugging
        _ = Task.Run(() =>
        {
            while (!_ngrokProcess.StandardOutput.EndOfStream)
            {
                string? line = _ngrokProcess.StandardOutput.ReadLine();
                if (!string.IsNullOrEmpty(line))
                    Console.WriteLine($"[ngrok] {line}");
            }
        });

        // Wait for the tunnel to appear in ngrok API
        int retries = 20; // up to 20 seconds
        while (retries-- > 0)
        {
            try
            {
                var response = await _http.GetStringAsync("http://127.0.0.1:4040/api/tunnels");
                using var doc = JsonDocument.Parse(response);
                var tunnels = doc.RootElement.GetProperty("tunnels");

                if (tunnels.GetArrayLength() > 0)
                {
                    PublicUrl = tunnels[0].GetProperty("public_url").GetString();
                    if (!string.IsNullOrEmpty(PublicUrl))
                        break;
                }
            }
            catch
            {
                // ignore, retry after delay
            }

            await Task.Delay(1000);
        }

        if (string.IsNullOrEmpty(PublicUrl))
            throw new Exception("Ngrok tunnel could not be established. Make sure ngrok is installed, free port is available, and the server is listening.");
    }

    public void Stop()
    {
        if (_ngrokProcess != null && !_ngrokProcess.HasExited)
            _ngrokProcess.Kill();
    }
}
