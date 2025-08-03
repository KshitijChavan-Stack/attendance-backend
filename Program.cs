using AttendanceAPI.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel to listen on port 80 (needed for Render deployment)
builder.WebHost.UseUrls("http://*:80");

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Enable CORS globally
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Parse DATABASE_URL from Render environment variable
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

if (string.IsNullOrWhiteSpace(databaseUrl) || !databaseUrl.StartsWith("postgres://"))
    throw new Exception("Invalid or missing DATABASE_URL");

var uri = new Uri(databaseUrl);
var userInfo = uri.UserInfo.Split(':');
var connectionString = $"Host={uri.Host};Port={uri.Port};Username={userInfo[0]};Password={userInfo[1]};Database={uri.AbsolutePath.TrimStart('/')};SSL Mode=Require;Trust Server Certificate=true";

// Register DbContext
builder.Services.AddDbContext<AttendanceDbContext>(options =>
    options.UseNpgsql(connectionString));

var app = builder.Build();

// Enable CORS early
app.UseCors("AllowAll");

// Apply any pending migrations
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AttendanceDbContext>();
    db.Database.Migrate();
}

// Enable HTTPS redirect (safe with Render)
app.UseHttpsRedirection();

// Authorization middleware (if needed)
app.UseAuthorization();

// Enable Swagger in Development only
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Map Controllers
app.MapControllers();

// Default root route
app.MapGet("/", () => "It works from Docker!");

// Start app
app.Run();
