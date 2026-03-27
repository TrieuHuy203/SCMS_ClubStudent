using SCMS.BusinessService.Services;
using SCMS.DAL.Repositories;
using SCMS.DomainEntities.Entities;
using SCMS.Contracts.Interfaces.iService;
using SCMS.Contracts.Interfaces.iRepositores;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<SCMSDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// TODO: Đăng ký DI cho service/repository ở đây nếu cần
 builder.Services.AddScoped<IUserService, UserService>();
 builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IClubService, ClubService>();
builder.Services.AddScoped<IClubRepository, ClubRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();
