using System.Security.Cryptography;
using System.Text;
using DocumentStorage.Authentication;
using DocumentStorage.Document;
using DocumentStorage.Infrastructure.FileServer;
using DocumentStorage.Infrastructure.Jwt;
using DocumentStorage.Infrastructure.PostgreSql;
using DocumentStorage.User;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
    
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
     c.AddSecurityDefinition(
        name: "Bearer",
        securityScheme: new OpenApiSecurityScheme
        {
            Description = "Provide a JWT issued by oak9 B2C. (Do NOT added the `Bearer` prefix!)",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "bearer"
        });

    // Pass authentication along to all endpoints.
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Reference = new OpenApiReference
                    {
                        Id = "Bearer",
                        Type = ReferenceType.SecurityScheme
                    }
                },
                new string[] { }
            }
        });
});

builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IAuthenticationRepository, AuthenticationRepository>();
builder.Services.AddTransient<IAuthenticationService, AuthenticationService>();
builder.Services.AddTransient<IGroupRepository, GroupRepository>();
builder.Services.AddTransient<IGroupService, GroupService>();
builder.Services.AddTransient<IFileStorage, LocalFileStorage>();
builder.Services.AddTransient<IDocumentRepository, DocumentRepository>();
builder.Services.AddTransient<IDocumentService, DocumentService>();
builder.Services.AddTransient<IAuthenticationKeyRepository, AuthenticationKeyRepository>();

var secret = Convert.FromBase64String(builder.Configuration?.GetValue<string>("jwt:secretKey"));
var symmetricKey = Convert.FromBase64String(builder.Configuration?.GetValue<string>("jwt:encryptionKey"));

using Aes aes = Aes.Create();

aes.Key = symmetricKey;
aes.IV = secret.Take(16).ToArray();

byte[] ciphertext = secret.Skip(16).ToArray();

using ICryptoTransform decryptor = aes.CreateDecryptor();
byte[] decryptedKey = decryptor.TransformFinalBlock(ciphertext, 0, ciphertext.Length);
string jwtSecretKey = Encoding.UTF8.GetString(decryptedKey);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
    options.TokenValidationParameters = new TokenValidationParameters
    {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSecretKey ?? string.Empty)),
            ClockSkew = TimeSpan.Zero
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Documents Storage API v1");
        c.RoutePrefix = string.Empty;
    });
}

// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
