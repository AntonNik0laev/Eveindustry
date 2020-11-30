﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Eveindustry.Sde.Models;

namespace Eveindustry.Sde.Repositories
{
    public interface ISdeDataRepository
    {
        Task Init();
        IDictionary<long, SdeType> GetAll();
        IEnumerable<SdeType> GetAllTradeable();
    }
}