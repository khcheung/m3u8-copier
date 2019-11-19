using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace M3U8Console
{
    class Program
    {
        static async Task Main(string[] args)
        {            
            //await Download("");
            Serve();
        }

        static async Task Download(String path)
        {

            M3U8.Parser p = new M3U8.Parser(new Uri(path));
            var url = p.GetDownloadInfo();
            var client = new HttpClient();
            foreach (var u in url)
            {
                Console.WriteLine($"Downloading: {u.FullUrl}");
                using (var fileStream = await client.GetStreamAsync(u.FullUrl))
                {
                    using (var file = File.Create(u.SaveFileName))
                    {
                        await fileStream.CopyToAsync(file);
                        await file.FlushAsync();
                        file.Close();
                    }
                    fileStream.Close();
                }
            }

            var content = p.GetM3U8Content();
            using (var file = File.Create("index.m3u8"))
            {
                Console.WriteLine($"Saving M3U8 File");
                using (var sw = new StreamWriter(file))
                {
                    await sw.WriteAsync(content);
                    sw.Flush();
                    sw.Close();
                }
                file.Close();
            }
            Console.WriteLine("Finish");
        }

        static void Serve()
        {
            M3U8.Server s = new M3U8.Server();
            s.Start();
       
        }
    }
}
