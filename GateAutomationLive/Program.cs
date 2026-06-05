using GateAutomationLive.Infrastructure;
using GateAutomationLive.Services;

var builder = WebApplication.CreateBuilder(args);

// ============================================================
// MVC / Web API setup
// ============================================================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ============================================================
// Application services
//
// GatePassService is registered as Singleton because it's a thin
// facade over the in-memory store and holds no per-request state.
// ============================================================
builder.Services.AddSingleton<GatePassStore>();
builder.Services.AddSingleton<IGatePassService, GatePassService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.Run();
