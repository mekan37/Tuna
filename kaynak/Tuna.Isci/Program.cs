using Tuna.Altyapi;
using Tuna.Isci;
using Tuna.Uygulama;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddTunaUygulama();
builder.Services.AddTunaAltyapi(builder.Configuration);
builder.Services.AddHostedService<Isci>();

var host = builder.Build();
host.Run();
