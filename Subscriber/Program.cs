var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCap(config =>
{
    //Transport RabbitMQ
    config.UseRabbitMQ(rq =>
    {
        rq.HostName = "localhost";
        rq.ExchangeName = "cap.default.topic";
        rq.UserName = "guest";
        rq.Password = "guest";
        rq.Port = 5672;
        rq.ConnectionFactoryOptions = opt => { };
    });

    //Storage
    config.UseMongoDB(mg =>
    {
        mg.DatabaseName = "CapTest_Subscriber";
        mg.DatabaseConnection = "mongodb://localhost";


    });


});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
