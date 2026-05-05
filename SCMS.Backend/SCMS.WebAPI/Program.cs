using SCMS.BusinessService.Services;
using SCMS.DAL.Repositories;
using SCMS.DomainEntities.Entities;
using SCMS.Contracts.Interfaces.iService;
using SCMS.Contracts.Interfaces.iRepositores;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Hangfire;


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
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<IMembershipService, MembershipService>();
builder.Services.AddScoped<IMembershipRepository, MembershipRepository>();
builder.Services.AddScoped<IAdminMembershipService, AdminMembershipService>();
builder.Services.AddScoped<IAdminMembershipRepository, AdminMembershipRepository>();
builder.Services.AddScoped<IEventRegistrationService, EventRegistrationService>();
builder.Services.AddScoped<IEventRegistrationRepository, EventRegistrationRepository>();
builder.Services.AddScoped<IClubFavoriteService, ClubFavoriteService>();
builder.Services.AddScoped<IClubFavoriteRepository, ClubFavoriteRepository>();
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<IFeedbackService, FeedbackService>();
builder.Services.AddScoped<IFeedbackRepository, FeedbackRepository>();
builder.Services.AddScoped<IAuthLogService, AuthLogService>();
builder.Services.AddScoped<IAuthLogRepository, AuthLogRepository>();
builder.Services.AddScoped<IAuditLogService, AuditLogService>();
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
builder.Services.AddScoped<IPostImageRepository, PostImageRepository>();
builder.Services.AddScoped<IPostImageService, PostImageService>();
builder.Services.AddScoped<IPostReportRepository, PostReportRepository>();
builder.Services.AddScoped<IPostReportService, PostReportService>();
builder.Services.AddScoped<IAdminDashboardRepository, AdminDashboardRepository>();
builder.Services.AddScoped<IAdminDashboardService, AdminDashboardService>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IUserPermissionRepository, UserPermissionRepository>();
builder.Services.AddScoped<IUserPermissionService, UserPermissionService>();
builder.Services.AddScoped<IUserRoleRepository, UserRoleRepository>();
builder.Services.AddScoped<IUserRoleService, UserRoleService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();


builder.Services.AddScoped<EventStatusJob>();
// CORS - cho phép frontend Vite gọi API
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});




// cấu hình     JWT
// Thêm vào trước builder.Build()
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options => // Cấu hình JWT Bearer
{
    // Cấu hình để xác thực JWT
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false, 
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
});

// Thêm cấu hình Hangfire vào DI container
builder.Services.AddHangfire(x =>
    x.UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection"))); // Kết nối DB
builder.Services.AddHangfireServer(); // Thêm Hangfire Server để chạy background jobs

var app = builder.Build();
// Thêm dashboard để theo dõi job (có thể bỏ nếu không cần)
app.UseHangfireDashboard();
// Đăng ký job chạy mỗi 1 giờ (\"0 * * * *\" là cron expression cho mỗi giờ)
RecurringJob.AddOrUpdate<EventStatusJob>(
    "event-status-job", // recurringJobId
    job => job.UpdateEventStatuses(),
    "* * * * *", // chạy mỗi phút
    new RecurringJobOptions()
);
// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();
app.UseStaticFiles(); // Cho phép frontend truy cập ảnh trong wwwroot
app.UseCors("AllowFrontend");
app.UseAuthentication(); // Thêm middleware xác thực JWT
app.UseAuthorization();

app.MapControllers();




app.Run();
