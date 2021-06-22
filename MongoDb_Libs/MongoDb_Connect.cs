using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MongoDb_Core_Mini_Connect.MongoDb_Libs
{
    public interface IMongoDb_Global
    {
        /// <summary>
        /// Получить клиента Интерфейс
        /// </summary>
        IMongoClient Interface_db_client { get; set; }

        /// <summary>
        /// Получить Сервер
        /// </summary>
        MongoServer _Server { get; set; }

        /// <summary>
        /// Получить клиента
        /// </summary>
        MongoClient db_client { get; set; }

        /// <summary>
        /// Получить Базу Интерфейс
        /// </summary>
        IMongoDatabase GetSelect_InterfaceDatabaseDb { get; set; }

        /// <summary>
        /// Получить Базу
        /// </summary>
        MongoDatabase GetSelect_DatabaseDb { get; set; }

        /// <summary>
        /// Подключение базы
        /// </summary>
        /// <param name="ConnectSetting">Настройка сойдинения порт хост время</param>
        /// <param name="NameDb">Имя Базы которую нужно создать, или подключиться</param>
        //MongoDb_Connect(MongoClientSettings ConnectSetting, string NameDb);

        /// <summary>
        /// Проверка на подключение к базе true есть/ false нету
        /// </summary>
        bool GetConnectServer { get; set; }

        /// <summary>
        /// Проверка на подключение к базе true есть/ false нету
        /// <para>Годиться для ASP CORE и Других</para>
        /// </summary>
        /// <returns>Возвращает базе true есть/ false нету</returns>
        bool GetConnectServer2();

        /// <summary>
        /// Получить список колекции документов таблиц
        /// </summary>
        string[] GetCollectionNamesDb { get; set; }

        /// <summary>
        /// Получить Ошибки
        /// </summary>
        string ErrorConnectServer { get; set; }

        /// <summary>
        /// Получить используймый Хост для подключения к базе
        /// </summary>
        MongoClientSettings ConnectSetHost { get; set; }

        /// <summary>
        /// Получить Колекцию Имена Баз
        /// </summary>
        /// <returns></returns>
        Task<string[]> GetDatabaseNames();

        /// <summary>
        /// Вызвать проверить сойдинение
        /// </summary>
        /// <returns></returns>
        bool Сall_Timeout();

    }

 
    public class MongoDb_Connect: IMongoDb_Global
    {
        /// <summary>
        /// Получить клиента Интерфейс
        /// </summary>
        public IMongoClient Interface_db_client { get; set; }

        /// <summary>
        /// Получить Сервер
        /// </summary>
        public MongoServer _Server { get; set; }

        /// <summary>
        /// Получить клиента
        /// </summary>
        public MongoClient db_client { get; set; }

        /// <summary>
        /// Получить Базу Интерфейс
        /// </summary>
        public IMongoDatabase GetSelect_InterfaceDatabaseDb { get; set; }

        /// <summary>
        /// Получить Базу
        /// </summary>
        public MongoDatabase GetSelect_DatabaseDb { get; set; }

        /// <summary>
        /// Подключение базы
        /// </summary>
        /// <param name="ConnectSetting">Настройка сойдинения порт хост время</param>
        /// <param name="NameDb">Имя Базы которую нужно создать, или подключиться</param>
        public MongoDb_Connect(MongoClientSettings ConnectSetting, string NameDb)
        {
            try
            {
                //MongoUrl url_mongo = new MongoUrl(ConnectString);

                db_client = new MongoClient(ConnectSetting);

                TimeSpan timeout = TimeSpan.FromSeconds(1);

                //Подключения
                _Server = db_client.GetServer();
                GetSelect_DatabaseDb = _Server.GetDatabase(NameDb);
                GetSelect_InterfaceDatabaseDb = db_client.GetDatabase(GetSelect_DatabaseDb.Name);

                var pingTask = GetSelect_InterfaceDatabaseDb.RunCommandAsync<BsonDocument>(new BsonDocument("ping", 1));
                pingTask.Wait(timeout);
                if (pingTask.IsCompleted)
                {
                    GetConnectServer = true;
                    //Записываем колекции
                    using (var collCursor = GetSelect_InterfaceDatabaseDb.ListCollectionsAsync().Result)
                    {
                        List<BsonDocument> colls = collCursor.ToListAsync().Result;
                        GetCollectionNamesDb = new string[colls.Count()];
                        for (int i = 0; i < colls.Count(); i++)
                        {
                            GetCollectionNamesDb[i] = colls[i]["name"].ToString();
                        }
                    }
                }
                else
                {
                    GetConnectServer = false;
                }

                ConnectSetHost = ConnectSetting;
            }
            catch (MongoConnectionException ex)
            {
                ConnectSetHost = ConnectSetting;
                ErrorConnectServer += ex.Message + "\n";
            }

        }

        /// <summary>
        /// Проверка на подключение к базе true есть/ false нету
        /// <para>Годиться для ASP CORE и Других</para>
        /// </summary>
        /// <returns>Возвращает базе true есть/ false нету</returns>
        public bool GetConnectServer2()
        {
            bool Flag = false;
            TimeSpan timeout = TimeSpan.FromSeconds(1);
            var pingTask = GetSelect_InterfaceDatabaseDb.RunCommandAsync<BsonDocument>(new BsonDocument("ping", 1));
            pingTask.Wait(timeout);
            if (pingTask.IsCompleted)
            {
                Flag = true;
                //Записываем колекции
                using (var collCursor = GetSelect_InterfaceDatabaseDb.ListCollectionsAsync().Result)
                {
                    List<BsonDocument> colls = collCursor.ToListAsync().Result;
                    GetCollectionNamesDb = new string[colls.Count()];
                    for (int i = 0; i < colls.Count(); i++)
                    {
                        GetCollectionNamesDb[i] = colls[i]["name"].ToString();
                    }
                }
            }
            else
            {
                Flag = false;
            }
            return Flag;
        }

        /// <summary>
        /// Проверка на подключение к базе true есть/ false нету
        /// <para>Работает и хранит сойдинение</para>
        /// <para>Не годиться для ASP CORE сохраняет сойдинения даже если его нет!</para>
        /// </summary>
        /// <returns>Возвращает базе true есть/ false нету</returns>
        public bool GetConnectServer { get; set; }

        /// <summary>
        /// Получить список колекции документов таблиц
        /// </summary>
        public string[] GetCollectionNamesDb { get; set; }

        /// <summary>
        /// Получить Ошибки
        /// </summary>
        public string ErrorConnectServer { get; set; }


        /// <summary>
        /// Получить используймый Хост для подключения к базе
        /// </summary>
        public MongoClientSettings ConnectSetHost { get; set; }


        /// <summary>
        /// Получить Колекцию Имена Баз
        /// </summary>
        /// <returns></returns>
        public async Task<string[]> GetDatabaseNames()
        {
            string[] NameBaseDb = null;
            try
            {
                using (var cursor = await db_client.ListDatabasesAsync())
                {
                    List<BsonDocument> databaseDocuments = await cursor.ToListAsync();
                    NameBaseDb = new string[databaseDocuments.Count()];
                    for (int i = 0; i < databaseDocuments.Count(); i++)
                    {
                        NameBaseDb[i] = databaseDocuments[i]["name"].ToString();
                    }
                }
            }
            catch (MongoException ex)
            {
                ErrorConnectServer += ex.Message + "\n";
            }
            return NameBaseDb;
        }

        /// <summary>
        /// Вызвать проверить сойдинение
        /// </summary>
        /// <returns></returns>
        public bool Сall_Timeout()
        {
            bool flag = false;
            var startTime = DateTime.UtcNow;
            try
            {
                using (var timeoutCancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMilliseconds(500)))
                {
                    //await collection.Find("{ _id : 1     }").ToListAsync(timeoutCancellationTokenSource.Token);
                }
            }
            catch (OperationCanceledException ex)
            {
                var endTime = DateTime.UtcNow;
                var elapsed = endTime - startTime;
                Console.WriteLine("Operation was cancelled after {0} seconds.", elapsed.TotalSeconds);
            }
            return flag;
        }

    }

    public class MongoDb_Autoconnect : IMongoDb_Global
    {
        /// <summary>
        /// Имя базы зарание определенной
        /// </summary>
        public string NameDb = "TestingDb";

        /// <summary>
        /// Получить используймый Хост для подключения к базе
        /// </summary>
        public MongoClientSettings ConnectSetHost { get; set; }

        /// <summary>
        /// Проверка на подключение к базе true есть/ false нету
        /// </summary>
        public bool GetConnectServer { get; set; }

        /// <summary>
        /// Получить список колекции документов таблиц
        /// </summary>
        public string[] GetCollectionNamesDb { get; set; }

        /// <summary>
        /// Получить Ошибки
        /// </summary>
        public string ErrorConnectServer { get; set; }

        /// <summary>
        /// Получить клиента Интерфейс
        /// </summary>
        public IMongoClient Interface_db_client { get; set; }

        /// <summary>
        /// Получить Сервер
        /// </summary>
        public MongoServer _Server { get; set; }

        /// <summary>
        /// Получить клиента
        /// </summary>
        public MongoClient db_client { get; set; }

        /// <summary>
        /// Получить Базу Интерфейс
        /// </summary>
        public IMongoDatabase GetSelect_InterfaceDatabaseDb { get; set; }

        /// <summary>
        /// Получить Базу
        /// </summary>
        public MongoDatabase GetSelect_DatabaseDb { get; set; }


        /// <summary>
        /// Авто Подключение к базе
        /// </summary>
        public MongoDb_Autoconnect()
        {
            MongoClientSettings settingcon = new MongoClientSettings();
            settingcon.Server = new MongoServerAddress("localhost", 27017);
            settingcon.SocketTimeout = new TimeSpan(0, 0, 0, 2);
            settingcon.WaitQueueTimeout = new TimeSpan(0, 0, 0, 2);
            settingcon.ConnectTimeout = new TimeSpan(0, 0, 0, 2);
            try
            {
                //MongoUrl url_mongo = new MongoUrl(ConnectString);

                db_client = new MongoClient(settingcon);

                TimeSpan timeout = TimeSpan.FromSeconds(1);

                //Подключения
                _Server = db_client.GetServer();
                GetSelect_DatabaseDb = _Server.GetDatabase(NameDb);
                GetSelect_InterfaceDatabaseDb = db_client.GetDatabase(GetSelect_DatabaseDb.Name);

                var pingTask = GetSelect_InterfaceDatabaseDb.RunCommandAsync<BsonDocument>(new BsonDocument("ping", 1));
                pingTask.Wait(timeout);
                if (pingTask.IsCompleted)
                {
                    GetConnectServer = true;
                    //Записываем колекции
                    using (var collCursor = GetSelect_InterfaceDatabaseDb.ListCollectionsAsync().Result)
                    {
                        List<BsonDocument> colls = collCursor.ToListAsync().Result;
                        GetCollectionNamesDb = new string[colls.Count()];
                        for (int i = 0; i < colls.Count(); i++)
                        {
                            GetCollectionNamesDb[i] = colls[i]["name"].ToString();
                        }
                    }
                }
                else
                {
                    GetConnectServer = false;
                }

                ConnectSetHost = settingcon;
            }
            catch (MongoConnectionException ex)
            {
                ConnectSetHost = settingcon;
                ErrorConnectServer += ex.Message + "\n";
            }

        }


        /// <summary>
        /// Получить Колекцию Имена Баз
        /// </summary>
        /// <returns></returns>
        public async Task<string[]> GetDatabaseNames()
        {
            string[] NameBaseDb = null;
            try
            {
                using (var cursor = await db_client.ListDatabasesAsync())
                {
                    List<BsonDocument> databaseDocuments = await cursor.ToListAsync();
                    NameBaseDb = new string[databaseDocuments.Count()];
                    for (int i = 0; i < databaseDocuments.Count(); i++)
                    {
                        NameBaseDb[i] = databaseDocuments[i]["name"].ToString();
                    }
                }
            }
            catch (MongoException ex)
            {
                ErrorConnectServer += ex.Message + "\n";
            }
            return NameBaseDb;
        }

        /// <summary>
        /// Вызвать проверить сойдинение
        /// </summary>
        /// <returns></returns>
        public bool Сall_Timeout()
        {
            bool flag = false;
            var startTime = DateTime.UtcNow;
            try
            {
                using (var timeoutCancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMilliseconds(500)))
                {
                    //await collection.Find("{ _id : 1     }").ToListAsync(timeoutCancellationTokenSource.Token);
                }
            }
            catch (OperationCanceledException ex)
            {
                var endTime = DateTime.UtcNow;
                var elapsed = endTime - startTime;
                Console.WriteLine("Operation was cancelled after {0} seconds.", elapsed.TotalSeconds);
            }
            return flag;
        }

        /// <summary>
        /// Проверка на подключение к базе true есть/ false нету
        /// <para>Годиться для ASP CORE и Других</para>
        /// </summary>
        /// <returns>Возвращает базе true есть/ false нету</returns>
        public bool GetConnectServer2()
        {
            bool Flag = false;
            TimeSpan timeout = TimeSpan.FromSeconds(1);
            var pingTask = GetSelect_InterfaceDatabaseDb.RunCommandAsync<BsonDocument>(new BsonDocument("ping", 1));
            pingTask.Wait(timeout);
            if (pingTask.IsCompleted)
            {
                Flag = true;
                //Записываем колекции
                using (var collCursor = GetSelect_InterfaceDatabaseDb.ListCollectionsAsync().Result)
                {
                    List<BsonDocument> colls = collCursor.ToListAsync().Result;
                    GetCollectionNamesDb = new string[colls.Count()];
                    for (int i = 0; i < colls.Count(); i++)
                    {
                        GetCollectionNamesDb[i] = colls[i]["name"].ToString();
                    }
                }
            }
            else
            {
                Flag = false;
            }
            return Flag;
        }
    }
}
