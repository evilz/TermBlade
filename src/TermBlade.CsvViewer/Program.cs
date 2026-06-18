using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TermBlade.Razor.Hosting;

namespace TermBlade.CsvViewer;

internal static class Program
{
  /// <summary>
  /// Main.
  /// </summary>
  /// <param name="args">The args value.</param>
  public static async Task<int> Main(string[] args)
  {
    Console.OutputEncoding = Encoding.UTF8;

    if (args.Length == 0 || args.Contains("--help") || args.Contains("-h"))
    {
      PrintHelp();
      return args.Length == 0 ? 1 : 0;
    }

    CsvViewerOptions options;
    try
    {
      options = CsvViewerOptions.Parse(args);
    }
    catch (ArgumentException ex)
    {
      Console.Error.WriteLine(ex.Message);
      PrintHelp();
      return 1;
    }

    if (options.FilePath is null)
    {
      Console.Error.WriteLine("Missing CSV file path.");
      PrintHelp();
      return 1;
    }

    if (!File.Exists(options.FilePath))
    {
      Console.Error.WriteLine($"CSV file not found: {options.FilePath}");
      return 1;
    }

    var fullPath = Path.GetFullPath(options.FilePath);
    var host = Host.CreateDefaultBuilder()
        .ConfigureServices(services =>
        {
          services.Configure<CsvViewerStartupOptions>(startupOptions =>
          {
            startupOptions.FilePath = fullPath;
            startupOptions.Delimiter = options.Delimiter;
            startupOptions.HasHeader = options.HasHeader;
          });
          services.Configure<TermBladeRazorOptions>(termBladeOptions => termBladeOptions.ExitOnCtrlC = false);
        })
        .UseTermBladeRazor<CsvViewerApp>()
        .Build();

    await host.RunAsync().ConfigureAwait(false);
    return 0;
  }

  private static void PrintHelp()
  {
    Console.WriteLine("TermBlade CSV Viewer");
    Console.WriteLine();
    Console.WriteLine("Usage: tbcsv [options] <file.csv>");
    Console.WriteLine();
    Console.WriteLine("Options:");
    Console.WriteLine("  -d, --delimiter <char>  Force a delimiter instead of auto-detection.");
    Console.WriteLine("  --no-header            Treat the first row as data and generate Column N headers.");
    Console.WriteLine("  -h, --help             Show this help text.");
  }
}
