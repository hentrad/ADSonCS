using System.Diagnostics;
using System.Runtime.InteropServices;
using HashLab;

const int port = 5080;
string url = $"http://localhost:{port}/";

var server = new WebServer();
Console.WriteLine(url);
OpenBrowser(url);
try { await server.RunAsync(url); }
catch (Exception ex) { Console.WriteLine($"Не удалось запустить сервер (порт {port} занят?): {ex.Message}"); }

static void OpenBrowser(string url)
{
    try
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) Process.Start("open", url);
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) Process.Start("xdg-open", url);
        else Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
    }
    catch {}
}
