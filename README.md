# MongoDb_Core_Mini_Connect
Auntification and Mango Convergence Library

------------Reference(Зависимости)---------------
Microsoft.Extensions.Caching.Memory
Microsoft.AspNetCore.Session
Microsoft.AspNetCore.Diagnostics 
Microsoft.AspNetCore.Mvc          
Microsoft.AspNetCore.StaticFiles
Microsoft.Extensions.Logging.Debug
Microsoft.VisualStudio.Web.BrowserLink
mongocsharpdriver    


Назначение
Asp_Core_MongoDb.cs - Для Asp Core (манго с сессиями и подключением)
Если пользоваться Консолю , или WindowsForm, или WPF, и Asp Net Mvc, и ин..
то Asp_Core_MongoDb.cs не нужен его лучше удалить
достаточно и MongoDb_Service_Connect.cs для роботы.

	   
	   Добавление
Models/MongoDb_Core_Mini_Connect

------------------Startup.cs-------------------------------
-------ConfigureServices(IServiceCollection services)-------
public void ConfigureServices(IServiceCollection services)
{            
services.AddDistributedMemoryCache();
services.AddSession();
			
//Подключить Сервис Базы 
services.Add_MongoDbConnect_Service();
           
services.AddMvc();
}


---------Configure(IApplicationBuilder app-------------------
public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
{
if (env.IsDevelopment())
{
app.UseDeveloperExceptionPage();
app.UseBrowserLink();
}
else
{
app.UseExceptionHandler("/Home/Error");
}
app.UseSession();
//Перенаправление если Сервер MongoDb выключен
app.UseMongoDb_RedirectToError("/Error/ErrorMongoDb.html");
//Перенаправление если не удалось передать сессию
app.Use_SessionMongoDB_Core("/Error/ErrorSessionMongoDb.html");
			
app.UseStaticFiles();
			
app.UseMvc(routes =>
{
routes.MapRoute(
 name: "default",
template: "{controller=Home}/{action=Index}/{id?}");
});
}
        



        
Применение

public IActionResult Index()
{
   BsonDocument bson=new BsonDocument();
   Session_MongoDb.MongoDb_AddEdit_Object("user",bson);
   return View();
}

public IActionResult Test1()
{
    BsonDocument bson = Session_MongoDb.MongoDb_GetObject("user");

    return View();
}
