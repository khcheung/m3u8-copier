using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace M3U8
{
    public class Parser
    {
        private Uri mUrl = null;
        private String mContent = "";
        private Dictionary<String, DownloadInfo> mDownloadInfo = null;


        public Parser(Uri m3u8Url)
        {
            mUrl = m3u8Url;

            DownloadM3U8().Wait();

            ParseContent();
        }

        private async Task DownloadM3U8()
        {
            HttpClient client = new HttpClient();
            using (var fileStream = await client.GetStreamAsync(this.mUrl))
            {
                using (var sr = new StreamReader(fileStream))
                {
                    this.mContent = await sr.ReadToEndAsync();
                    sr.Close();
                }
                fileStream.Close();
            }
        }

        public List<DownloadInfo> GetDownloadInfo()
        {
            return this.mDownloadInfo.Values.ToList();
        }

        public String GetM3U8Content()
        {
            var line = mContent.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var allLine = line.Select(l => l.StartsWith("#") ? l : mDownloadInfo[l].SaveFileName).ToArray();
            var content  = String.Join("\n", allLine);
            return content;
        }

        public void ParseContent()
        {
            var line = mContent.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var path = mUrl.ToString().Substring(0, mUrl.ToString().LastIndexOf("/") + 1);
            this.mDownloadInfo = (from l in line
                                  where !l.StartsWith("#")
                                  select new DownloadInfo()
                                  {
                                      Key = l,
                                      FullUrl = l.StartsWith("http") ? l : $"{path}{l}",
                                      SaveFileName = l.Contains("/") ? l.Substring(l.LastIndexOf("/") + 1) : l
                                  })
                       .ToDictionary(l => l.Key, l => l);
        }

    }

    public class DownloadInfo
    {
        public String Key { get; set; }
        public String FullUrl { get; set; }
        public String SaveFileName { get; set; }
    }
}
