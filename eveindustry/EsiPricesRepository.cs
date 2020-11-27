using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Eveindustry.Models;
using RestSharp;

namespace Eveindustry
{
    /// <inheritdoc cref="IEsiPricesRepository" />
    public sealed class EsiPricesRepository : IEsiPricesRepository, IDisposable
    {
        private readonly TimeSpan updateInterval = TimeSpan.FromMinutes(60);
        private SortedList<long, ESIPriceData> data;
        private Timer updateTimer;

        /// <inheritdoc />
        public async Task Init()
        {
            await this.UpdateData();
            this.updateTimer = new Timer(_ => this.UpdateData().Wait(), null, this.updateInterval, this.updateInterval);
        }

        /// <inheritdoc />
        public ESIPriceData GetAdjustedPriceInfo(long typeId)
        {
            return this.data[typeId];
        }

        /// <inheritdoc />
        public void Dispose()
        {
            this.updateTimer?.Dispose();
        }

        private async Task UpdateData()
        {
            var result = new SortedList<long, ESIPriceData>();

            // https://esi.evetech.net/latest/markets/prices/
            var client = new RestClient("https://esi.evetech.net/latest");
            var pricesRequest = new RestRequest("/markets/prices/");
            var prices = await client.GetAsync<List<ESIPriceData>>(pricesRequest);
            foreach (var item in prices)
            {
                result[item.TypeId] = item;
            }

            this.data = result;
        }
    }
}