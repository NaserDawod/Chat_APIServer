using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Messaging_WebApp.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Messaging_WebApp.Hubs;
using Messaging_WebApp.Services;

/*
    in linux: dotnet ef database update
    then:   dotnet run
    
    in windows: Update-Database
    then run via visual studio 
*/
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<Messaging_WebAppContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Messaging_WebAppContext") ?? throw new InvalidOperationException("Connection string 'Messaging_WebAppContext' not found.")));
builder.Services.AddScoped<IUserService, UserService>();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWTParams:Audience"],
        ValidIssuer = builder.Configuration["JWTParams:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWTParams:SecretKey"]))
    };
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("Allow All",
        builder =>
        {
            builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
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

app.UseCors("Allow All");

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Sign}/{action=Login}/{id?}");

app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<MyHub>("/myHub");
});
app.Run();
