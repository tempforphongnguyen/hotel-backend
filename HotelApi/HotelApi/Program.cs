using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Infrastructure.Entities;
using Infrastructure.Context;
using Domain.Service;
using Domain.IService;
using Microsoft.Extensions.FileProviders;
using System.Text.Json.Serialization;
using HotelApi.Controller;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();  

builder.Services.AddIdentity<User, ApplicationRole>().AddEntityFrameworkStores<ApplicationDBContext>();

builder.Services.AddScoped<IAuthService,AuthService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IVnpayService, VnpayService>();

builder.Services.AddScoped<IRoomTypeService, RoomTypeService>();
builder.Services.AddScoped<IExtraServiceService, ExtraServiceService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<ISendMailService, SendMailService>();
builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IImageRoomTypeService, ImageRoomTypeService>();
builder.Services.AddScoped<IUploadFileService, UploadFileService>();
builder.Services.AddScoped<IOrderDetailService, OrderDetailService>();
builder.Services.AddScoped<IHistoryService, HistoryService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped(typeof(IHelperService<>), typeof(HelperService<>));


builder.Services.AddControllers(options => options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true);
builder.Services.AddControllers().AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);


builder.Services.AddDbContext<ApplicationDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(o =>
    {
        o.RequireHttpsMetadata = false;
        o.SaveToken = false;
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = builder.Configuration["JWT:Issuer"],
            ValidAudience = builder.Configuration["JWT:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"])),
            ClockSkew = TimeSpan.Zero,
            //LifetimeValidator = (DateTime? notBefore, DateTime? expires, SecurityToken token, TokenValidationParameters parameters) =>
            //{
            //    return expires != null && expires > DateTime.UtcNow.AddHours(5); // Add 5 hours to the expiration time
            //}
        };
    });

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(p=> p.AddPolicy("corsPolicy", build => 
    build.WithOrigins("*").AllowAnyMethod().AllowAnyHeader()
));

var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API v1"));
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "API Publish");
        options.RoutePrefix = string.Empty;
    });
}

app.UseCors("corsPolicy");

app.UseHttpsRedirection();
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

StaticFileOptions staticFileOptions = new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine
          (app.Environment.ContentRootPath, "images")),
    RequestPath = "/images"
};

app.UseStaticFiles(staticFileOptions);
