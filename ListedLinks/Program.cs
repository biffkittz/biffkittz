using ListedLinks.Models;
using ListedLinks.Pages;
using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.Hosting;

using Microsoft.AspNetCore.Http;
using JavaScriptEngineSwitcher.V8;
using JavaScriptEngineSwitcher.Extensions.MsDependencyInjection;
using React.AspNet;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

var dbConfig = builder.Configuration.GetSection("ListedLinksContextSettings").Get<ListedLinksContextSettings>();
ListedLinksContext.ConnectionString = dbConfig?.ConnectionString;

var openAIConfig = builder.Configuration.GetSection("OpenAI").Get<TinkerModelSettings>();
TinkerModel.ApiKey = openAIConfig?.ApiKey;

using (var dbContext = new ListedLinksContext())
{
    dbContext.Database.EnsureCreated();
    //dbContext.Database.Migrate();
}

builder.Services.AddRazorPages();

// TODO: Remove after debug
builder.Services.AddSingleton<IHostEnvironment>(new HostingEnvironment());

// Preparing for React
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddReact();
// Make sure a JS engine is registered, or you will get an error!
builder.Services.AddJsEngineSwitcher(options => options.DefaultEngineName = V8JsEngine.EngineName)
  .AddV8();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ListedLinksContext>(/*options => {
    options.UseSqlite(builder.Configuration.GetConnectionString("ListedLinksContext") ??
        throw new InvalidOperationException("Connection string 'ListedLinksContext' not found."));
}*/);

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

// Initialise ReactJS.NET. Must be before static files.
app.UseReact(config =>
{
    // If you want to use server-side rendering of React components,
    // add all the necessary JavaScript files here. This includes
    // your components as well as all of their dependencies.
    // See http://reactjs.net/ for more information. Example:
    //config
    //    .AddScript("~/js/remarkable.min.js")
    //    .AddScript("~/js/tutorial.jsx")
    //    .SetJsonSerializerSettings(new JsonSerializerSettings
    //    {
    //        StringEscapeHandling = StringEscapeHandling.EscapeHtml,
    //        ContractResolver = new CamelCasePropertyNamesContractResolver()
    //    });

    // If you use an external build too (for example, Babel, Webpack,
    // Browserify or Gulp), you can improve performance by disabling
    // ReactJS.NET's version of Babel and loading the pre-transpiled
    // scripts. Example:
    //config
    //  .SetLoadBabel(false)
    //  .AddScriptWithoutTransform("~/js/bundle.server.js");
});

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.UseForwardedHeaders();

app.Run();
