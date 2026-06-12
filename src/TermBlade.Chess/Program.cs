using System.Text;
using Microsoft.Extensions.Hosting;
using TermBlade.Razor.Hosting;

Console.OutputEncoding = Encoding.UTF8;

var host = Host.CreateDefaultBuilder(args)
    .UseTermBladeRazor<TermBlade.Chess.ChessApp>()
    .Build();

await host.RunAsync().ConfigureAwait(false);
