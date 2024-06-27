using System.Net;
using SurveySystem.Utils;
using Serilog;
using SurveySystem;


var builder = WebApplication.CreateBuilder(args);
builder.Host.ConfigureCustomHost(args);
ServiceCollection(builder);
BuildWebApplication(builder);


void ServiceCollection(WebApplicationBuilder builder)
{   
    // Add services to the container.
    builder.Services.AddControllersWithViews();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddMemoryCache();
    builder.Services.AddScoped<Activity.Activity>()
    .AddLogging(options  =>
    {
        options.ClearProviders();
        options.AddSerilog(dispose: true);
    })
    .AddControllers()
        .ConfigureApiBehaviorOptions(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.OK;

                var property = context.ModelState.FirstOrDefault().Key.Split("$.").LastOrDefault();
                var errorMsg = context.ModelState.FirstOrDefault().Value?.Errors.FirstOrDefault()?.ErrorMessage;
                if (errorMsg != null && errorMsg.StartsWith("The JSON value could not be converted to "))
                    throw new SurveyException(FailureReason.InvalidProperty, $"Invalid {property}");
                
                throw new SurveyException(FailureReason.InvalidRequestBodyFormat);
            };
        });

    builder.Services.AddRouting(options =>
        {
            options.LowercaseUrls = true;
        });
    
    builder.Services.AddCors(option =>
        {
            option.AddPolicy("SurveyCorsPolicy", corsPolicyBuilder =>
            {
                corsPolicyBuilder
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        }); 
}


void BuildWebApplication(WebApplicationBuilder builder)
{
    Log.Information("Building the Web Application Started");
    
    var webApplication = builder.Build();
    if (builder.Environment.IsDevelopment()) 
        webApplication.UseDeveloperExceptionPage();
    
    webApplication.UsePathBase("/")
        .UseRouting()
        .UseCors()
        .UseForwardedHeaders()
        .UseHttpsRedirection()
        .UseAuthorization()
        .UseMiddleware<ExceptionMiddleware>()
        .UseMiddleware<ActivityWriterMiddleware>();
        
    Log.Information("*** all middlewares created ***");
    webApplication.UseStaticFiles();
    webApplication.MapControllers();
    webApplication.MapControllerRoute(
        name: "default",
        pattern: "{action=Index}/");
    Log.Information("*** start running webApplication ***");
    webApplication.Run();
    Log.Information($"Application Has Been Shutdown At '{DateTime.Now}'");
}





/*
// Add services to the container.
builder.Services.AddControllersWithViews();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();*/