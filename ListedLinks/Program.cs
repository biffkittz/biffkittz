using ListedLinks.Models;
using ListedLinks.Pages;
using ListedLinks.Hubs;
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
using Microsoft.AspNetCore.Mvc;

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
//builder.Services.AddSingleton<IHostEnvironment>(new HostingEnvironment());

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

builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        builder =>
        {
            builder.WithOrigins("http://127.0.0.1:8080", "https://biffkittz.com", "http://localhost:8080")
                .AllowAnyHeader()
                .WithMethods("GET", "POST", "OPTIONS")
                .AllowCredentials();
        });
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

app.MapPost("/ingest", async ([FromBody] string data, ListedLinksContext db, HttpContext context) =>
{
    var clientIP = context.Connection.RemoteIpAddress?.ToString();
    await db.Comments.AddAsync(new Comment { Text = $"{data} [clientIP: {clientIP}]", CreatedAt = DateTime.Now });
    await db.SaveChangesAsync();

    return Results.Ok("success");
});

//app.UseCors();
app.MapHub<SaaSActivityHub>("/saasactivityhub");

//app.Use(async (context, next) =>
//{
//    if (context.Request.Path == "/loginstanceevent")
//    {
//        var hubContext = context.RequestServices
//            .GetRequiredService<IHubContext<InstanceEventHub>>();
//        var instanceWebHostNameKey = context.Request.Query["wk"].ToString();
//        var instanceEventDatum = context.Request.Query["d"].ToString();

//        // TODO: Make configurable
//        //if ((new List<string> { "biffkittz1234", "transientpermanence5267" }).Where(_ => instanceWebHostNameKey
//        //        .ToLower()
//        //        .Contains(_)
//        //    ).Count() > 0
//        //)
//        //{
//        //await hubContext.Clients.All.SendAsync("ReceiveInstanceEvent", instanceWebHostNameKey, instanceEventDatum);

//        if (!instanceEventDatum.Contains("ai_Echo")) // don't re-analyze an event we already analyzed and posted back to the instance
//        {
//            var bot = new ChatGpt(TinkerModel.ApiKey ?? "");
//            var prompt = @"Please start your response to this task in a funny, snarky way. The use of puns is encouraged.
//                    You are a sophisticated translator of json-formatted ScreenConnect instance event data into plain English sentences. You love hot dogs and the smell of swamp.
//                    Please review the following json-formatted ScreenConnect instance event datum and summarize its most important aspects in plain English;
//                    include the participant or host name and IP address or network address in your summary when available: " + instanceEventDatum;
//            var funPrompt = "You are a comedienne specializing in remote support software technology humor. Your favorite bird is red and you dip your bananas in coffee. Tell a silly tech joke for the slightly edgy audience.";

//            if (instanceEventDatum.IndexOf("QueuedCommand") == -1 && instanceEventDatum.IndexOf("peanuthamper") > -1)
//            {
//                prompt = funPrompt;
//                var aiResponse = await bot.Ask(prompt);
//                await hubContext.Clients.All.SendAsync(
//                    "ClientHasReceivedALittleSomething",
//                    "GPT",
//                    $"{aiResponse}|||-||||RawEventData:{instanceEventDatum}|||-||||ai_Echo"
//                );
//            }
//            else if (instanceEventDatum.StartsWith("SOSSOSSOS"))
//            {
//                await hubContext.Clients.All.SendAsync(
//                    "ClientHasReceivedALittleSomething",
//                    "SOS",
//                    instanceEventDatum.Replace("SOSSOSSOS", "")
//                );
//            }
//            else if (instanceEventDatum.StartsWith("GUESTGUEST"))
//            {
//                await hubContext.Clients.All.SendAsync(
//                    "ClientHasReceivedALittleSomething",
//                    "SOS",
//                    instanceEventDatum.Replace("GUESTGUEST", "")
//                );
//            }

//            using (var httpClient = new HttpClient())
//            {
//                try
//                {
//                    await httpClient.PostAsJsonAsync(
//                        "https://biffkittz.screenconnect.com/App_Extensions/afe875d2-f636-408a-a02d-43db12626c9e/Service.ashx/RecordNote",
//                        "[\"aiResponse\"]"
//                    );
//                }
//                catch (Exception ex)
//                {
//                    await hubContext.Clients.All.SendAsync(
//                        "ClientHasReceivedALittleSomething",
//                        "GPT",
//                        $"Exception|||^||||<<RawEventData: {ex.Message}>>|||^||||<<ai_Echo>>"
//                    );
//                }
//            }
//            ;

//            // POST to instance
//        }

//        context.Response.StatusCode = StatusCodes.Status200OK;
//        return;
//    }

//    if (next != null)
//    {
//        await next.Invoke();
//    }
//});

app.Run();
