using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Eveindustry.Core.Models;
using Eveindustry.Core.Models.Config;
using RestSharp;

namespace Eveindustry.Core
{
    /// <inheritdoc cref="IEvePricesRepository" />
    public class EvePricesRepository : IEvePricesRepository, IDisposable
    {
        private readonly TimeSpan updateInterval;
        private readonly ITypeIdsSource typeIdsSource;
        private SortedList<long, EvePriceInfo> prices;
        private Timer updateTimer;

        /// <summary>
        /// Initializes a new instance of the <see cref="EvePricesRepository"/> class.
        /// </summary>
        /// <param name="typeIdsSource">data source to get required type ids for market data. </param>
        /// <param name="config">update prices configuration, including update interval. </param>
        public EvePricesRepository(ITypeIdsSource typeIdsSource, IEvePricesUdateConfiguration config)
        {
            this.typeIdsSource = typeIdsSource;
            this.updateInterval = TimeSpan.FromMinutes(config.UpdateIntervalMinutes);
        }

        /// <inheritdoc />
        public async Task Init()
        {
            await this.UpdatePrices();
            this.updateTimer = new Timer(
                _ => this.UpdatePrices().Wait(),
                null,
                this.updateInterval,
                this.updateInterval);
        }

        /// <summary>
        /// Return current type prices information.
        /// Data updates each 30 minutes.
        /// </summary>
        /// <param name="typeId">item type id to search price. </param>
        /// <returns>current Jita prices for a type. </returns>
        public EvePriceInfo GetPriceInfo(long typeId)
        {
            return this.prices[typeId];
        }

        /// <inheritdoc  />
        public void Dispose()
        {
            this.updateTimer?.Dispose();
        }

        private async Task UpdatePrices()
        {
            var pageSize = 200;
            var typeIds = this.typeIdsSource.GetTypeIds();

            var typeIdsList = typeIds as long[] ?? typeIds.ToArray();
            int totalPages = (int)Math.Ceiling((double)typeIdsList.Length / pageSize);

            var jitaId = "30000142";

            var baseUrl = "https://api.evemarketer.com/";
            var client = new RestClient(baseUrl);
            var result = new SortedList<long, EvePriceInfo>();
            for (int pageNum = 0; pageNum < totalPages; pageNum++)
            {
                var request = new RestRequest("ec/marketstat");
                var length = (pageNum * pageSize) + pageSize > typeIdsList.Length
                    ? (typeIdsList.Length - (pageNum * pageSize))
                    : pageSize;
                for (int i = 0; i < length; i++)
                {
                    var currentIndex = (pageNum * pageSize) + i;
                    var typeId = typeIdsList[currentIndex];
                    request.AddQueryParameter("typeid", typeId.ToString());
                }

                request.AddQueryParameter("usesystem", jitaId);

                var response = await client.ExecuteAsync(request);
                var rawText = response.Content;

                var doc = new XmlDocument();
                doc.Load(new StringReader(rawText));
                var items = doc["exec_api"]?["marketstat"];

                if (items?.ChildNodes == null)
                {
                    // TODO Add logging.
                    continue;
                }

                foreach (XmlNode childNode in items?.ChildNodes)
                {
                    var id = long.Parse(childNode.Attributes["id"].Value);
                    var minSell = Decimal.Parse(childNode["sell"]["min"].InnerText);
                    var maxBuy = Decimal.Parse(childNode["buy"]["max"].InnerText);

                    result.Add(id, new EvePriceInfo(id, maxBuy, minSell));
                }
            }

            this.prices = result;
        }
    }
}