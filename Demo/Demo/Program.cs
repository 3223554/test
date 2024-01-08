using Demo;
using Demo.Service;
using Microsoft.AspNetCore;

public class Program
{
    public static void Main(string[] args)
    {
        CreateWebHostBuilder(args).Build().Run();
    }



    public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
        WebHost.CreateDefaultBuilder(args)
        .UseUrls("http://*:8080").UseKestrel()
        .ConfigureAppConfiguration((hostingContext, builder) =>
        {
 
        })
        .UseStartup<Startup>();
}

