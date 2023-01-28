using Project_Radon.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yttrium_browser;
using CommunityToolkit.Mvvm;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Net;

namespace Project_Radon.Helpers
{
    public class ContributersLoader
    {
        private async Task<string> Get(string path)
        {
           return await new WebClient().DownloadStringTaskAsync(new Uri(path));
        }
        public async Task<List<Models.Contributer>> GetContributers()
        {
            try
            {
               var str = await Get("https://api.github.com/repos/itzbluebxrry/Project-Radon/contributors");
                return JsonConvert.DeserializeObject<List<Models.Contributer>>(str);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
