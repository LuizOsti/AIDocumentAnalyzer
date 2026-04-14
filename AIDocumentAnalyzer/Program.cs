using Library.BLL;
using Library.Util;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// ── Services ──────────────────────────────────────────────────────────────────

builder.Services.AddHttpClient();

builder.Services.AddSingleton<LLMServiceUtil>();
builder.Services.AddScoped<AuthBLL>();
builder.Services.AddScoped<AnalyzerBLL>();
builder.Services.AddScoped<ChatBLL>();
builder.Services.AddScoped<KnowledgeBaseBLL>();

builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
        options.SerializerSettings.Converters.Add(new StringEnumConverter()));

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "AI Document Analyzer API",
        Version = "v1",
        Description = "A .NET 8 REST API that uses GPT-4.1 to perform structured risk analysis on any JSON document."
    });

    // Include XML comments from the Web API project
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        c.IncludeXmlComments(xmlPath);

    // Include XML comments from the Library project
    var libraryXmlPath = Path.Combine(AppContext.BaseDirectory, "Library.xml");
    if (File.Exists(libraryXmlPath))
        c.IncludeXmlComments(libraryXmlPath);

    // Add Token header to all endpoints
    c.AddSecurityDefinition("Token", new OpenApiSecurityScheme
    {
        Name = "Token",
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header,
        Description = "Enter your authentication GUID token."
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Token"
                }
            },
            []
        }
    });
});

// ── Pipeline ──────────────────────────────────────────────────────────────────

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AI Document Analyzer v1"));

// NOTE: AllowAnyOrigin is intentionally permissive for development/demo purposes.
// In production, replace with a specific origin allowlist.
app.UseCors();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
