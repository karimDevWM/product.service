using Confluent.Kafka;
using product.service.Consumer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton(new ConsumerConfig
{
    BootstrapServers = Environment.GetEnvironmentVariable("Kafka_BootstrapServers"),
    ClientId = Environment.GetEnvironmentVariable("Kafka_Client_Id"),
    GroupId = Environment.GetEnvironmentVariable("Kafka_groupId"),
    AutoOffsetReset = AutoOffsetReset.Earliest
});
builder.Services.AddSingleton<IHostedService, KafkaConsumerHandler>();

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
