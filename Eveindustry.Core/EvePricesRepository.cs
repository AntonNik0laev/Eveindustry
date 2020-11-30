using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Eveindustry.Core.Models;
using Eveindustry.Core.Models.Config;
using MessagePack;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RestSharp;

namespace Eveindustry.Core
{
    /// <inheritdoc cref="IEvePricesRepository" />
    public class EvePricesRepository : IEvePricesRepository, IDisposable
    {
        private readonly TimeSpan updateInterval;
        private readonly ITypeIdsSource typeIdsSource;
        private readonly ILogger<EvePricesRepository> logger;
        private SortedList<long, EvePriceInfo> prices;
        private Timer updateTimer;
        private string cacheFileName = Path.Join(AppDomain.CurrentDomain.BaseDirectory, "prices.bin");

        /// <summary>
        /// Initializes a new instance of the <see cref="EvePricesRepository"/> class.
        /// </summary>
        /// <param name="typeIdsSource">data source to get required type ids for market data. </param>
        /// <param name="config">update prices configuration, including update interval. </param>
        /// <param name="logger">logger instance</param>
        public EvePricesRepository(ITypeIdsSource typeIdsSource, IOptions<EvePricesUdateConfiguration> config, ILogger<EvePricesRepository> logger)
        {
            this.typeIdsSource = typeIdsSource;
            this.logger = logger;
            this.updateInterval = TimeSpan.FromMinutes(config.Value.UpdateIntervalMinutes);
        }

        /// <inheritdoc />
        public async Task Init()
        {
            
            this.updateTimer = new Timer(
                _ => this.UpdatePrices().Wait(),
                null,
                this.updateInterval,
                this.updateInterval);
            
            await TryLoadFromFile();
            if(this.prices != null) return;
            
            await this.UpdatePrices();
            
        }

        private async Task DumpToFile()
        {
            this.logger.LogInformation("Starting to save binary prices cache");
            var fileName = this.cacheFileName;
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
            this.logger.LogInformation("Successfully saved binary cache file with prices. ");
            await using var fs = File.Create(fileName);
            await MessagePackSerializer.SerializeAsync(fs, this.prices);
            fs.Flush();
        }

        private async Task TryLoadFromFile()
        {
            
            var fileName = this.cacheFileName;
            if (!File.Exists(fileName)) return;
            var createdDate = File.GetCreationTimeUtc(fileName);
            var minCreatedTime = DateTime.UtcNow.Add(-this.updateInterval);
            if(createdDate < minCreatedTime)
            {
                this.logger.LogInformation("Found binary cache file, but created date is less then configured minimum . created: {{createdDate}}. min: {minCreatedTime}", createdDate, minCreatedTime);
                return;
            };
            this.logger.LogInformation("Starting to read prices from cache");
            var fs = File.OpenRead(fileName);
            var result = await MessagePackSerializer.DeserializeAsync<SortedList<long, EvePriceInfo>>(fs);
            this.prices = result;

        }
        
        /// <summary>
        /// Return current type prices information.
        /// Data updates each 30 minutes.
        /// </summary>
        /// <param name="typeId">item type id to search price. </param>
        /// <returns>current Jita prices for a type. </returns>
        public EvePriceInfo GetPriceInfo(long typeId)
        {
            return this.prices.ContainsKey(typeId) ? prices[typeId] : new EvePriceInfo(typeId, 0,0);
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
            logger.LogInformation("Starting to update prices for {typeIdsCount} items, 200 items per page. total pages: {totalPages}", typeIdsList.Length, totalPages);
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
                logger.LogInformation("Completed updating prices for page {pageNum} of {totalPages}", pageNum, totalPages);
            }

            this.prices = result;
            await DumpToFile();
        }
    }
}