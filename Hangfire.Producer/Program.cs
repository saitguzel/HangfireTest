using Hangfire;
using Hangfire.PostgreSql;
using Hangfire.Producer;
using HangfireBasicAuthenticationFilter;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHangfire(opt =>
{
    opt.UsePostgreSqlStorage(builder.Configuration.GetConnectionString("DbConnectionString"))
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings();
});
builder.Services.AddHangfireServer();
builder.Services.AddSingleton<IJobService, JobService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[]
    {
        new HangfireCustomBasicAuthenticationFilter
        {
            User = app.Configuration.GetSection("HangfireSettings:UserName").Value,
            Pass = app.Configuration.GetSection("HangfireSettings:Password").Value
        }
    }
});

RecurringJob.AddOrUpdate<IJobService>(Guid.NewGuid().ToString(), a => a.HealthCheck(), Cron.Minutely);

app.UseAuthorization();

app.MapControllers();

app.Run();