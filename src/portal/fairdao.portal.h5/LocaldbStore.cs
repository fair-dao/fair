using fairdao.extensions.shared.data;
using fairdao.extensions.shared.IndexedDB;

namespace fairdao.portal.h5
{

    /// <summary>
    /// localdb存储管理类
    /// </summary>
    public class LocaldbStore : fairdao.extensions.shared.IDataStore
    {
        public FairdaoDatabase Database;


        public LocaldbStore(FairdaoDatabase database)
        {
            this.Database= database;
        }

   

    

        public async Task<T> GetConfig<T>(string configId)
        {

            T val = await Database.ConfigStore.Get<string, T>(configId);
            if (val == null) return default(T);
            return val;
        }

        public Task RemoveConfig(string configId)
        {
            throw new NotImplementedException();
        }


        public async Task SetConfig(string configId, object config)
        {
            try
            {
                if (config == null)
                {
                    await Database.ConfigStore.Delete<string>(configId);
                }
                else
                {
                    string v = System.Text.Json.JsonSerializer.Serialize(config);

                    var old = await Database.ConfigStore.Get<string, object>(configId);
                    if (old == null)
                    {
                        try
                        {
                            await Database.ConfigStore.Add<object, string>(config);
                        }
                        catch(Exception e)
                        {
                            await Database.ConfigStore.Put<object, string>(config);
                        }
                    }
                    else await Database.ConfigStore.Put<object, string>(config);
                }

            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
            }

        }


    }
}
