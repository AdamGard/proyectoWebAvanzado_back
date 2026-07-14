using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using proyectoWebAvanzado.Data;
using proyectoWebAvanzado.Exceptions;
using proyectoWebAvanzado.Services.Implementaciones;
using proyectoWebAvanzado.Services.Interfaces;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
const string FrontendDevelopmentPolicy = "FrontendDevelopment";

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDataProtection()
        .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, ".data-protection-keys")));
}

builder.Services.AddCors(options =>
{
    options.AddPolicy(FrontendDevelopmentPolicy, policy =>
    {
        policy.WithOrigins(
                "http://localhost:4200",
                "http://localhost:54481")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("No se encontró la cadena de conexión 'DefaultConnection'.");

builder.Services.AddDbContext<AppDBContext>(options =>
{
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString));
});

builder.Services.AddScoped<IUsuarioServices, UsuarioService>();
builder.Services.AddScoped<IActividadServices, ActividadService>();
builder.Services.AddScoped<IAuthServices, AuthService>();
builder.Services.AddScoped<IRolServices, RolService>();
builder.Services.AddScoped<IProgresoServices, ProgresoService>();

builder.Services.AddAuthorization();

var jwtkey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrWhiteSpace(jwtkey))
{
    throw new InvalidOperationException("No se encontró la clave Jwt:Key.");
}

var keyBytes = Encoding.UTF8.GetBytes(jwtkey);
builder.Services.AddAuthentication(config =>
{
    config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(config =>
{
    config.RequireHttpsMetadata = false;
    config.SaveToken = true;
    config.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exceptionFeature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
        var exception = exceptionFeature?.Error;
        var statusCode = exception is ApiException apiException
            ? apiException.StatusCode
            : StatusCodes.Status500InternalServerError;

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";

        var problem = new
        {
            status = statusCode,
            title = exception is ApiException ? exception.Message : "Ocurrió un error inesperado.",
            traceId = context.TraceIdentifier
        };

        await context.Response.WriteAsJsonAsync(problem);
    });
});

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseCors(FrontendDevelopmentPolicy);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
