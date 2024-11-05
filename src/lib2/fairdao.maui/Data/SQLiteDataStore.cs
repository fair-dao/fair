using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using SQLite;

namespace clientApp.Data
{
    public class SQLiteDataStore : fairdao.extensions.shared.IDataStore
    {
        private readonly SQLiteAsyncConnection _database;
        /// <summary>
        /// 操作锁
        /// </summary>
        private static object dbLock = new object();

        string dbPath;

        public const SQLite.SQLiteOpenFlags Flags =
            // open the database in read/write mode
            SQLite.SQLiteOpenFlags.ReadWrite |
            // create the database if it doesn't exist
            SQLite.SQLiteOpenFlags.Create |
            // enable multi-threaded database access
            SQLite.SQLiteOpenFlags.SharedCache;
        public SQLiteDataStore()
        {
            this.dbPath = Path.Combine(FileSystem.AppDataDirectory, "wechat2.db3"); ;
            //if (!File.Exists(dbPath))
            //{
            //    File.WriteAllText(dbPath, "");
            //}

            //}else
            //{
            //    File.Delete(dbPath);
            //}
            //   dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "userinfo.db3");
            // dbPath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "db.sqlite");


            _database = new SQLiteAsyncConnection(dbPath, Flags);

           
            //创建配置表
            _database.CreateTableAsync<Data.CliConfig>().Wait();
 

            // _database.InsertAsync(new Data.CliConfig { Id = "abc", Config = "aaaa" }).Wait();

            // var config = _database.GetAsync<CliConfig>("abc").Result;

        }



        public Task<T> GetConfig<T>(string configId)
        {
            try
            {

                Console.WriteLine($"读取配置{configId}");


                // var config = await _database.GetAsync<CliConfig>("abc");
                var config = _database.Table<CliConfig>().FirstOrDefaultAsync(m => m.Id == configId).Result;

                //config =  _database.GetAsync<CliConfig>(configId).Result;
                if (config != null)
                {
                    var data = System.Text.Json.JsonSerializer.Deserialize<T>(config.Config);
                    return Task.FromResult(data);
                }

                return Task.FromResult<T>(default(T));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw ex;
            }
        }



        public Task SetConfig(string configId, object config)
        {
            CliConfig c = new CliConfig();
            c.Id = configId;
            try
            {
                c.Config = System.Text.Json.JsonSerializer.Serialize(config);

            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }


            Console.WriteLine("写入配置");
            _database.InsertOrReplaceAsync(c).Wait();
            return Task.CompletedTask;
        }

        /// <summary>
        /// 移除配置
        /// </summary>
        /// <param name="configId"></param>
        /// <returns></returns>
        public Task RemoveConfig(string configId)
        {
            _database.Table<CliConfig>().DeleteAsync(m => m.Id == configId).Wait();
            return Task.CompletedTask;
        }

    }
}
