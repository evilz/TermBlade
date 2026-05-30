using System.Text;
using Microsoft.Extensions.Hosting;
using TermBlade.Razor.Hosting;
using TermBlade.Gallery.Components;

Console.OutputEncoding = Encoding.UTF8;

var host = Host.CreateDefaultBuilder(args)
    .UseTermBladeRazor<GalleryApp>()
    .Build();

await host.RunAsync();
