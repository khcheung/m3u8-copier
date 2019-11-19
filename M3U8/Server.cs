using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.StaticFiles;

namespace M3U8
{
    public class Server
    {
        public Server()
        {


        }

        public void Start()
        {
            Console.WriteLine(System.Environment.CurrentDirectory);
            var webHostBuilder = WebHost.CreateDefaultBuilder();

            webHostBuilder.UseKestrel(o =>
            {
                o.Listen(IPAddress.Any, 5000);
            });

            webHostBuilder.UseContentRoot(System.Environment.CurrentDirectory);
            webHostBuilder.UseWebRoot(System.Environment.CurrentDirectory);

            webHostBuilder.ConfigureServices(s =>
            {

            });

            webHostBuilder.Configure(c =>
            {
                var fileServerOptions = new FileServerOptions()
                {
                    EnableDirectoryBrowsing = true,

                };

                fileServerOptions.StaticFileOptions.ServeUnknownFileTypes = true;
                var contentTypeProvider = new FileExtensionContentTypeProvider();
                contentTypeProvider.Mappings.Add(".m3u8", "application/vnd.apple.mpegurl");// "application /x-mpegURL");                
                contentTypeProvider.Mappings[".ts"] = "video/mp2t";
                fileServerOptions.StaticFileOptions.ContentTypeProvider = contentTypeProvider;

                c.UseFileServer(fileServerOptions);

            });

            var webHost = webHostBuilder.Build();
            webHost.Run();
        }

        public void Stop()
        {

        }

    }
}
