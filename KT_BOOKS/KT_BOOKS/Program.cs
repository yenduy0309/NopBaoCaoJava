using KT_BOOKS.HealthChecks;
using KT_BOOKS.Repository;
using KT_BOOKS.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Sinks.MSSqlServer;
using System.Text;
using System.Text.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<MyDbContext>(options =>
{
	var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
	options.UseSqlServer(connectionString, sqlOptions => sqlOptions.CommandTimeout(180));
});
builder.Services.AddHttpClient<ApiHealthCheck>();
builder.Services.AddHealthChecks()
.AddCheck("SQL Database", new SqlConnectionHealthCheck(builder.Configuration.GetConnectionString("DefaultConnection")))
.AddCheck<ApiHealthCheck>(nameof(ApiHealthCheck))
.AddDbContextCheck<MyDbContext>()
.AddCheck<SystemHealthCheck>(nameof(SystemHealthCheck));
builder.Host.UseSerilog((context, config) =>
{
	config.MinimumLevel.Information()
	.MinimumLevel.Override("Microsoft.AspNetCore", Serilog.Events.LogEventLevel.Warning)
	.MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
	.WriteTo.Console()
	.WriteTo.Debug()
	.WriteTo.File("Logs\\log-.txt",
	rollingInterval: RollingInterval.Day,
	rollOnFileSizeLimit: true,
	buffered: false,
	restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information)
	.WriteTo.MSSqlServer(
	connectionString: builder.Configuration.GetConnectionString("DefaultConnection"),
	sinkOptions: new MSSqlServerSinkOptions { TableName = "Logs", AutoCreateSqlTable = true },
	restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Warning);
});
builder.Host.UseSerilog((context, config) => config.ReadFrom.Configuration(context.Configuration));
// Cau hình JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true, //kiểm tra token đã hết hạng hay chưa
        ValidateIssuerSigningKey = true,
		ValidIssuer = builder.Configuration["Jwt:Issuer"],
		ValidAudience = builder.Configuration["Jwt:Audience"],
		IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SecretKeyHuyThailendthichcodedaoyeucuocsong12345")),
    };
});
builder.Services.AddScoped<AuthService>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ADMIN", policy =>
        policy.RequireClaim("RoleName", "ADMIN")); // Yêu cầu roleID là ADMIN
});
// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//them dich vu Cross origin request sharing vào tất cả các url
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowAll",
		builder =>
		{
			builder.AllowAnyOrigin()
				   .AllowAnyMethod()
				   .AllowAnyHeader();
		});
});

// them dich vu CORS cho chỉ cho phép url duy nhất truy câp
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowSpecificOrigin",
//        builder =>
//        {
//            builder.WithOrigins("http://localhost:3000")
//                   .AllowAnyMethod()
//                   .AllowAnyHeader();
//        });
//});
var app = builder.Build();
app.MapHealthChecks("/health", new HealthCheckOptions
{
	ResponseWriter = async (context, report) =>
	{
		context.Response.ContentType = "application/json";
		var result = JsonSerializer.Serialize(new
		{
			status = report.Status.ToString(),
			checks = report.Entries.Select(entry => new
			{
				name = entry.Key,
				status = entry.Value.Status.ToString(),

				exception = entry.Value.Exception?.Message,

				duration = entry.Value.Duration.ToString()

			})

		});

		await context.Response.WriteAsync(result);

	}

});
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<MyDbContext>();
    SeedData.SeedDingData(dbContext);
}
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// Su dung CORS
app.UseCors("AllowAll");

app.UseHttpsRedirection();
app.UseSerilogRequestLogging();
app.UseAuthentication(); // Kích hoat Authentication Middleware
app.UseAuthorization(); // Kích hoat Authorization Middleware

app.MapControllers();

app.Run();
