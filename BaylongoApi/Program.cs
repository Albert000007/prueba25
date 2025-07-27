using Baylongo.Data;
using Baylongo.Data.Data.MsSql.Contexts.SqlServer.BaylongoContext;
using BaylongoApi.Services.Interfaces;
using BaylongoApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using BaylongoApi.Extensions;
using SendGrid;
using System.Text.Json.Serialization;
using Microsoft.OpenApi.Models;
using System.Reflection;
using BaylongoApi.Services.Email.Setting;
using BaylongoApi.Services.Email.Contracts;
using BaylongoApi.Templates.Contracts;
using BaylongoApi.Services.Email.Providers;
using BaylongoApi.Services.Email;
using BaylongoApi.Templates;
using Stripe;
using BaylongoApi.Middleware;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Text.Json;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ISendGridClient>(new SendGridClient(builder.Configuration["EmailSettings:SendGrid:ApiKey"]));

builder.Services.ConfigureCors();
builder.Services.ConfigureIISIntegration();
builder.Services.ConfigureSwagger();

// Agrega esta línea para registrar IHttpContextAccessor
builder.Services.AddHttpContextAccessor();
// Agrega el servicio de cache si no está ya agregado
builder.Services.AddMemoryCache();


// Add services to the container.
// Configuración de Entity Framework (la cadena de conexión se agregará después)
builder.Services.AddDbContext<BaylongoContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});


StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

// Agrega el servicio de cache si no está ya agregado
builder.Services.AddMemoryCache();

// Agrega esto en tu Program.cs
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

// Registro de proveedores de email
builder.Services.AddScoped<OrganizationContentService>();
builder.Services.AddScoped<StripeService>();
builder.Services.AddScoped<IEmailProvider, BrevoEmailProvider>();
builder.Services.AddScoped<IOrganizationContentService, OrganizationContentService>();
builder.Services.AddScoped<IEmailProvider, SendGridEmailProvider>();
builder.Services.AddScoped<IEmailProvider, GmailSmtpProvider>();
builder.Services.AddScoped<IErrorLoggerService, ErrorLoggerService>();
builder.Services.AddScoped<IMaintenanceService, MaintenanceService>();
builder.Services.AddScoped<IDanceCategoryService, DanceCategoryService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IDanceService, DanceService>();
builder.Services.AddScoped<ILocationUrlService, LocationUrlService>();
builder.Services.AddScoped<ITemplateService, TemplateService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<ITemplateHelper, TemplateHelper>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, BaylongoApi.Services.TokenService>();
builder.Services.AddScoped<ICatalogService, CatalogService>();
builder.Services.AddScoped<IOrganizationService, OrganizationService>();
builder.Services.AddScoped<IEventService, BaylongoApi.Services.EventService>();
builder.Services.AddScoped<IOrganizationInvitationService, OrganizationInvitationService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                var tokenService = context.HttpContext.RequestServices.GetRequiredService<ITokenService>();
                var token = context.HttpContext.Request.Headers["Authorization"]
                    .ToString()
                    .Replace("Bearer ", "", StringComparison.OrdinalIgnoreCase);

                if (string.IsNullOrEmpty(token) || !await tokenService.IsTokenValid(token))
                {
                    context.Fail("Token invalidado");
                    // Esto debería ser suficiente para rechazar la petición
                    return;
                }
            },
            OnChallenge = context =>
            {
                // Personalizar la respuesta de error
                context.HandleResponse();
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                return context.Response.WriteAsync(JsonSerializer.Serialize(new
                {
                    StatusCode = StatusCodes.Status401Unauthorized,
                    Message = "Token inválido o expirado"
                }));
            }
        };
    });

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
//    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//})
//.AddCookie()
//.AddGoogle(options =>
//{
//    options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
//    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
//});


// Configuración de CORS (ajusta según tus necesidades)
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowAll", builder =>
//    {
//        builder.AllowAnyOrigin()
//               .AllowAnyMethod()
//               .AllowAnyHeader();
//    });
//});

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy
                .WithOrigins("http://192.168.100.167:3000","http://192.168.100.170:3000", "http://www.baylongo.com","https://www.baylongo.com", "https://baylongo.com", "http://localhost:3000", "https://tudominio.com", "http://c7153103-001-site1.ltempurl.com") // tus frontends
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});


var app = builder.Build();

app.UseCors(MyAllowSpecificOrigins); // ?? Importante: antes de UseAuthorization
// Agrega el middleware personalizado aquí
app.UseCustomExceptionMiddleware();

// Habilitar servicio de archivos estáticos
app.UseStaticFiles(); // Esto sirve archivos desde wwwroot

// Opcional: si quieres una ruta prefix para archivos estáticos
app.UseStaticFiles(new StaticFileOptions
{
    RequestPath = "/content"
});


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //descomentar cuando sea productivo
    //app.UseSwagger();
    //app.UseSwaggerUI();
}
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
//app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<BaylongoContext>();
    DbInitializer.Initialize(context);
}

app.Run();
