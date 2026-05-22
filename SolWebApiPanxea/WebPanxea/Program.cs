using Microsoft.AspNetCore.Mvc;
using WebPanxea;

var builder = WebApplication.CreateBuilder(args);

// --- AGREGA ESTO PARA EVITAR BLOQUEOS DE SEGURIDAD (CORS) ---
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
// ------------------------------------------------------------

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddScoped<INominaService, NominaService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// --- CORRE ESTO ANTES DE LOS CONTROLADORES ---
app.UseCors();
// ---------------------------------------------

app.UseAuthorization();
app.MapControllers();
app.Run();