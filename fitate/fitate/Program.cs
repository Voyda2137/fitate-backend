using FirebaseAdmin;
using fitate.DatabaseUtils;
using fitate.Utils;
using Google.Apis.Auth.OAuth2;

FirebaseApp.Create(new AppOptions
{
    Credential = GoogleCredential.FromFile("pwsw-906cf-firebase-adminsdk-2350p-100d2aa3a6.json")
});

var builder = WebApplication.CreateBuilder(args);

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

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<DatabaseUtils>(sp =>
{
    return new DatabaseUtils();
});

builder.Services.AddScoped<UserUtils>();
builder.Services.AddScoped<DishUtils>();
builder.Services.AddScoped<WorkoutUtils>();

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

app.UseCors("AllowAll");

app.Run();