namespace Eveindustry
{
    /// <summary>
    /// Generic interface to load data.
    /// </summary>
    /// <typeparam name="T"> type of data to load. </typeparam>
    public interface IDataLoader<out T>
    {
        /// <summary>
        /// Load data.
        /// </summary>
        /// <returns>requested data. </returns>
        public T Load();
    }
}