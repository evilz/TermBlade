using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TermBlade.Razor.Hosting;

namespace TermBlade.FileManager;

internal static class Program
{
  public static async Task Main(string[] args)
  {
    Console.OutputEncoding = Encoding.UTF8;

    var startPath = args.Length > 0 ? args[0] : Environment.CurrentDirectory;
    var fullStartPath = Path.GetFullPath(startPath);

    var host = Host.CreateDefaultBuilder()
        .ConfigureServices(services =>
        {
          services.Configure<FileManagerStartupOptions>(options => options.StartPath = fullStartPath);
          services.Configure<TermBladeRazorOptions>(options => options.ExitOnCtrlC = false);
          services.AddSingleton<IFileSystemOperations, SystemFileSystemOperations>();
        })
        .UseTermBladeRazor<FileManagerApp>()
        .Build();

    await host.RunAsync().ConfigureAwait(false);
  }
}
