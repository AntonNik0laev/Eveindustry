using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Eveindustry.Sde.Models;

namespace Eveindustry.Sde.Repositories
{
    public class SdeDataRepository : ISdeDataRepository
    {
        private readonly ISdeDataLoader loader;
        private SortedList<long, SdeType> allItems;

        public SdeDataRepository(ISdeDataLoader loader)
        {
            this.loader = loader;
        }

        public async Task Init()
        {
            this.allItems = await this.loader.Load();
        }
        
        public SortedList<long, SdeType> GetAll()
        {
            return this.allItems;
        }

        public IEnumerable<SdeType> GetAllTradeable()
        {
            return this.allItems.Values.Where(i => i.MarketGroupId != 0);
        }
    }
}