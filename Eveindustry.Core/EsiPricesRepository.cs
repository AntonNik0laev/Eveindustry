using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection.Emit;
using System.Threading;
using System.Threading.Tasks;
using Eveindustry.Core.Models;
using RestSharp;

namespace Eveindustry.Core
{
    /// <inheritdoc cref="IEsiPricesRepository" />
    public sealed class EsiPricesRepository : IEsiPricesRepository, IDisposable
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly TimeSpan updateInterval = TimeSpan.FromMinutes(60);
        private SortedList<long, ESIPriceData> data;
        private Timer updateTimer;

        /// <summary>
        /// Initialize new instance of <see cref="EvePricesRepository"/>
        /// </summary>
        /// <param name="httpClientFactory">http client to use for ESI api</param>
        public EsiPricesRepository(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        /// <inheritdoc />
        public async Task Init()
        {
            await this.UpdateData();
            this.updateTimer = new Timer(_ => this.UpdateData().Wait(), null, this.updateInterval, this.updateInterval);
        }

        /// <inheritdoc />
        public ESIPriceData GetAdjustedPriceInfo(long typeId)
        {
            return this.data.ContainsKey(typeId) ? this.data[typeId] : new ESIPriceData(){TypeId = typeId, AdjustedPrice = 0, AveragePrice = 0};
        }

        /// <inheritdoc />
        public void Dispose()
        {
            this.updateTimer?.Dispose();
        }

        private async Task UpdateData()
        {
            // https://esi.evetech.net/latest/markets/prices/
            // var client = new RestClient("https://esi.evetech.net/latest");
            // var pricesRequest = new RestRequest("/markets/prices/");
            // var prices = await client.GetAsync<List<ESIPriceData>>(pricesRequest);
            // var result = new SortedList<long, ESIPriceData>(prices.Count);
            // foreach (var item in prices)
            // {
            //     result[item.TypeId] = item;
            // }
            //
            // this.data = result;

            using var client = this.httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Get, "https://esi.evetech.net/latest/markets/prices/");
            using var respnse = await client.SendAsync(request);
            var prices = await respnse.Content.ReadFromJsonAsync<List<ESIPriceData>>();
            var result = new SortedList<long, ESIPriceData>(prices.Count);
            foreach (var item in prices)
            {
                result[item.TypeId] = item;
            }
            
            this.data = result;

        }
    }
}