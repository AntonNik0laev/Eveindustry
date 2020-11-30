using System.Threading.Tasks;

namespace Eveindustry.Sde.Loaders
{
    /// <summary>
    /// Generic interface to load data.
    /// </summary>
    /// <typeparam name="T"> type of data to load. </typeparam>
    public interface IDataLoader<T>
    {
        /// <summary>
        /// Load data.
        /// </summary>
        /// <returns>requested data. </returns>
        public Task<T> Load();
    }
}