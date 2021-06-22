using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;

namespace MongoDb_Core_Mini_Connect
{
    /// <summary>
    /// Коментарии
    /// </summary>
    class Asp_Core_MongoDb
    {
        //Asp_Core_MongoDb.cs нужен Для Asp Core это MongoDb и Session
        //В других случаях не нужен
    }

    public static class ServiceProviderExtensions
    {
        /// <summary>
        /// Сервис Подключение Базы
        /// </summary>
        /// <param name="services"></param>
        public static void Add_MongoDbConnect_Service(this IServiceCollection services)
        {
            services.AddTransient<MongoDb_Service_Connect>();
        }
    }

    /// <summary>
    /// Перенаправление при утрате подключения
    /// </summary>
    class Redirect_To_Error
    {
        private readonly RequestDelegate _next;
        private string _Page;
        private MongoDb_Service_Connect mongoDb;
        public Redirect_To_Error(RequestDelegate next, string Page)
        {
            mongoDb = new MongoDb_Service_Connect();
            this._next = next;
            this._Page = Page;
        }

        public async Task Invoke(HttpContext context)
        {

            if (mongoDb.manadgerDb.GetConnectServer2())
            {
                await _next.Invoke(context);
            }
            else
            {
                if (!string.IsNullOrEmpty(_Page))
                {
                    context.Response.Redirect(_Page);
                    return;

                }
                else
                {
                    context.Response.StatusCode = 404;
                    await context.Response.WriteAsync("Ошибка значение пустое!");
                }
            }

        }


    }

    public static class TokenExtensions
    {
        /// <summary>
        /// Проверка сойдинения с базой
        /// <para>В случаи не подключения-</para>
        /// <para>Перенаправляеться на страницу указаной вами ошибки-</para>
        /// <para>Применяеться метод после app.UseStaticFiles(); сразу</para>
        /// <para>app.UseStaticFiles();</para>
        /// <para>app.UseMongoDb_RedirectToError("/Errors/ErrorBase.html");</para>
        /// <para>Папка страниц ошибок wwwroot/Errors/ErrorBase.html</para>
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="Page">Путь к странице ошибки перенаправления, например: /Errors/ErrorBase.html</param>
        /// <returns></returns>
        public static IApplicationBuilder UseMongoDb_RedirectToError(this IApplicationBuilder builder, string Page)
        {
            return builder.UseMiddleware<Redirect_To_Error>(Page);
        }
    }

    class Session_MongoDb_Connect
    {
        private readonly RequestDelegate _next;
        private string _Page;
        public Session_MongoDb_Connect(RequestDelegate next, string Page)
        {
            this._next = next;
            this._Page = Page;
        }

        public async Task Invoke(HttpContext context)
        {
            if (Session_MongoDb.Config(context))
            {
                await _next.Invoke(context);
            }
            else
            {
                if (!string.IsNullOrEmpty(_Page))
                {
                    context.Response.Redirect(_Page);
                    return;

                }
                else
                {
                    context.Response.StatusCode = 404;
                    await context.Response.WriteAsync("Ошибка значение пустое!");
                }
            }

        }


    }

    public static class Session_MongoDb
    {
        //Сессии 
        private static ISession Session;

        /// <summary>
        /// Конфигурация Подключения Startup.cs -> app.Use_SessionMongoDB_Core("/Error/ErrorMongo.html");
        /// </summary>
        /// <param name="context"></param>
        /// <returns>Возвращает<see cref="bool"/> flag</returns>
        internal static bool Config(HttpContext context)
        {
            bool flag = false;
            try
            {
                Session = context.Session;
                flag = true;
            }
            catch
            {
                flag = false;
            }
            return flag;
        }

        /// <summary>
        /// Двойное Выполнение Add и Edit
        /// <para>Если нету обьекта в словаре то добавляет с ключем</para>
        /// <para>Если есть обьект с ключем то редактирует его на новый обьект</para>
        /// </summary>
        /// <param name="key">Ключь искомый в словаре</param>
        /// <param name="str">Обьект новый для добавление или замены старого</param>
        public static void MongoDb_AddEdit_Object(string key, BsonDocument obj)
        {
            string obj_str = string.Empty;
            obj_str = obj.ToJson();
            if (Session.Keys.Contains(key))
            {
                Session.Remove(key);
                Session.SetString(key, obj_str);
            }
            if (!Session.Keys.Contains(key))
            {
                Session.SetString(key, obj_str);
            }
        }

        /// <summary>
        /// Получить обьект по ключу из словаря
        /// </summary>
        /// <param name="key">ключь искомого обьекта</param>
        /// <returns></returns>
        public static BsonDocument MongoDb_GetObject(string key)
        {
            BsonDocument obj_str = null;
            if (Session.Keys.Contains(key))
            {
                //obj_str = BsonSerializer.Deserialize<BsonDocument>((string)TempData[key]);
                obj_str = BsonDocument.Parse((string)Session.GetString(key));
            }
            return obj_str;
        }

        /// <summary>
        /// Удаление Сессии по Ключу
        /// </summary>
        /// <param name="key">Ключь Сессии</param>
        public static void Delete_Session(string key)
        {
            if (Session.Keys.Contains(key))
            {
                Session.Remove(key);
            }
        }

        /// <summary>
        /// Очистка Сессии полностью
        /// </summary>
        public static void Clear_Session()
        {
            Session.Clear();
        }
    }

    public static class TokenMongoDbExtensions
    {
        /// <summary>
        /// Сервис для Сессий MongoDb
        /// </summary>
        /// <param name="services"></param>
        public static IApplicationBuilder Use_SessionMongoDB_Core(this IApplicationBuilder builder, string Page)
        {
            return builder.UseMiddleware<Session_MongoDb_Connect>(Page);
        }
    }
}
