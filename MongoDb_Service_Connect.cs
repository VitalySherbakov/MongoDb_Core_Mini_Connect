using MongoDB.Driver;
using System;
using MongoDb_Core_Mini_Connect.MongoDb_Libs;

namespace MongoDb_Core_Mini_Connect
{
    /// <summary>
    /// Сервис подключения Базы
    /// </summary>
    public class MongoDb_Service_Connect
    {
        /// <summary>
        /// Менеджер базы и подключения к серверу
        /// </summary>
        public MongoDb_Connect manadgerDb;

        /// <summary>
        /// Файловые методы (отключен)
        /// </summary>
        public MongoDb_GridFs_Files manadgerFilesDb;

        /// <summary>
        /// Подключена База
        /// </summary>
        public string MongoDbDataBase { get; }

        public MongoDb_Service_Connect()
        {
            MongoClientSettings settingcon = new MongoClientSettings();
            settingcon.Server = new MongoServerAddress("localhost", 27017);
            settingcon.SocketTimeout = new TimeSpan(0, 0, 0, 2);
            settingcon.WaitQueueTimeout = new TimeSpan(0, 0, 0, 2);
            settingcon.ConnectTimeout = new TimeSpan(0, 0, 0, 2);

            //Подключаемся к базе TestingDb имя базы
            string NameBase = "MongoDbs";
            manadgerDb = new MongoDb_Connect(settingcon, NameBase);
            MongoDbDataBase = NameBase;

            //ТАБЛИЦЫ (Двойное подключение)
            //Асинхронный результат подключения к базы
            //GetSelect_DatabaseDb возвращает MongoDatabase
            //GetSelect_InterfaceDatabaseDb возвращает IMongoDatabase
            Db_select = manadgerDb.GetSelect_DatabaseDb;
            IDb_select = manadgerDb.GetSelect_InterfaceDatabaseDb;

            //Подключение общих методов таблиц
            DbMethods_Table_Users = new DbMethods_Tables(Db_select, IDb_select);
            DbMethods_Table_Roles = new DbMethods_Tables(Db_select, IDb_select);

            //Подключение таблицы, колекцию документа
            DbMethods_Table_Users.CollectionAddDoc_Tables("Table_Users");
            DbMethods_Table_Roles.CollectionAddDoc_Tables("Table_Roles");
            //Подключение файловох методов отключен нет потребности
            //manadgerFilesDb = new MongoDb_GridFs_Files(settingcon, Db_select);
        }

        //Набор операции с таблиц два API 
        private MongoDatabase Db_select;
        private IMongoDatabase IDb_select;

        //Таблицы Db
        /// <summary>
        /// Набор методов для таблицы Table_User
        /// </summary>
        public DbMethods_Tables DbMethods_Table_Users;

        /// <summary>
        /// Набор методов для таблицы ролей Table_Roles
        /// </summary>
        public DbMethods_Tables DbMethods_Table_Roles;
    }

}