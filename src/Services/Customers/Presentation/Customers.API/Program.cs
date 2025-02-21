using Customers.Persistence;
using Customers.Application;
using Customers.Infrastructure.Filters;
using MassTransit;
using Customers.Persistence.Messaging.Consumers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options => options.Filters.Add<ValidationFilter>())
                .ConfigureApiBehaviorOptions(options => options.SuppressModelStateInvalidFilter = true);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddApplicationServices();

var rabbitMqConfig = builder.Configuration.GetSection("RabbitMQ");

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<TopUpEventConsumer>();
    x.AddConsumer<PurchaseEventConsumer>();
    x.AddConsumer<RefundEventConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(rabbitMqConfig["Host"], "/", h =>
        {
            h.Username(rabbitMqConfig["Username"]!);
            h.Password(rabbitMqConfig["Password"]!);

            cfg.ReceiveEndpoint("topup-queue", e =>
            {
                e.ConfigureConsumer<TopUpEventConsumer>(context);
                cfg.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
            });

            cfg.ReceiveEndpoint("purchase-queue", e =>
            {
                e.ConfigureConsumer<PurchaseEventConsumer>(context);
                cfg.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
            });

            cfg.ReceiveEndpoint("refund-queue", e =>
            {
                e.ConfigureConsumer<RefundEventConsumer>(context);
                cfg.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
            });
        });
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();