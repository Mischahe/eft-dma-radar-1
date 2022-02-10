using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.IO;

namespace eft_dma_radar
{
    internal static class TarkovMarketManager
    {
        private static Dictionary<string, TarkovMarketItem> _filteredItems = new Dictionary<string, TarkovMarketItem>();

        public static Dictionary<string, TarkovMarketItem> ItemFilter
        {
            get
            {
                return _filteredItems;
            }
        }
        public static void UpdateMarketItems()
        {
            var marketItems = new List<TarkovMarketItem>();
            if (!File.Exists("market.json") ||
File.GetLastWriteTime("market.json").AddHours(24) < DateTime.Now)
            {
                using (WebClient client = new WebClient())
                {
                    WebRequest request = WebRequest.Create(@"https://market_master.filter-editor.com/data/marketData_en.json");
                    using WebResponse response = request.GetResponse();
                    string json = client.DownloadString(response.ResponseUri);
                    marketItems = JsonSerializer.Deserialize<List<TarkovMarketItem>>(json);
                    File.WriteAllText("market.json", json);
                }
            }
            else
            {
                var json = File.ReadAllText("market.json");
                marketItems = JsonSerializer.Deserialize<List<TarkovMarketItem>>(json);
            }

#pragma warning disable CS8604 // Possible null reference argument.
            var items = marketItems.Where(x => x.avg24hPrice > 50000 || x.traderPrice > 50000);
            foreach (var item in items)
            {
                _filteredItems.TryAdd(item.bsgId, item);
            }
#pragma warning restore CS8604 // Possible null reference argument.
        }
    }

    public class TarkovMarketItem
    {
        public string uid { get; set; }
        public string name { get; set; }
        public List<string> tags { get; set; }
        public string shortName { get; set; }
        public int price { get; set; }
        public int basePrice { get; set; }
        public int avg24hPrice { get; set; }
        public int avg7daysPrice { get; set; }
        public string traderName { get; set; }
        public int traderPrice { get; set; }
        public string traderPriceCur { get; set; }
        public DateTime updated { get; set; }
        public int slots { get; set; }
        public double diff24h { get; set; }
        public double diff7days { get; set; }
        public string icon { get; set; }
        public string link { get; set; }
        public string wikiLink { get; set; }
        public string img { get; set; }
        public string imgBig { get; set; }
        public string bsgId { get; set; }
        public bool isFunctional { get; set; }
        public string reference { get; set; }
        public string apiKey { get; set; }
    }
}