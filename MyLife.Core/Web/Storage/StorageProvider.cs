using System.Configuration.Provider;

namespace MyLife.Web.Storage
{
    public abstract class StorageProvider : ProviderBase
    {
        /// <summary>
        /// Save data into data house
        /// </summary>
        /// <param name="key">Key of file</param>
        /// <param name="data">Data file</param>
        /// <returns>Key of file</returns>
        public abstract void Save(string key, byte[] data);

        /// <summary>
        /// Update data of file
        /// </summary>
        /// <param name="key">Key of file</param>
        /// <param name="data">Data of file</param>
        public abstract void Update(string key, byte[] data);

        /// <summary>
        /// Delete data file
        /// </summary>
        /// <param name="key">Key of file</param>
        public abstract void Delete(string key);

        /// <summary>
        /// Load data from store house
        /// </summary>
        /// <param name="key">Key of file</param>
        /// <returns>Data file</returns>
        public abstract byte[] Load(string key);
    }
}