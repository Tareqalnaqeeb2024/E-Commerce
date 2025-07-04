using E_Commerce.Basic;
using E_Commerce.DataAccess.Repositories;
using E_Commerce.Extenstion;
using E_Commerce.MappingProfile;
using E_CommerceDataAccess.Data;
using E_CommerceDataAccess.Interfaces;
using E_CommerceDataAccess.Models;
using E_CommerceDataAccess.Repositories;
using E_CommerceDataBusiness.Interfaces;
using E_CommerceDataBusiness.Services;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using E_CommerceDataBusiness;
using E_CommerceDataBusiness.BackgroundServices;
using E_Commerce.Business.Services;
using StackExchange.Redis;
using E_CommerceDataBusiness.Basic;
using E_CommerceDataBusiness.Interfaces.ExternalInterface;
using E_CommerceDataBusiness.Services.ExternalServices;
using E_Commerce.Hubs;
using E_CommerceDataBusiness.Hubs;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGenJWTAuth();


builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionString")),
    ServiceLifetime.Scoped);
builder.Services.AddIdentity<UserAccount, IdentityRole>() // Specify your custom user class
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JWT"));
builder.Services.AddCustomJwtAuth(builder.Configuration);
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderItemRepository, OrderItemRepository>();
builder.Services.AddScoped<ICartSRepository,CartSRepository>();


// Register services
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IFileStorageService, LocalFileStorageService>();
builder.Services.AddScoped<IOrderItemService, OrderItemService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<IRedisService, RedisService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<ICartService,CartService>();

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("MailSettings"));
builder.Services.AddSingleton<IEmailService, EmailService>();

builder.Services.AddSingleton<IRabbitMQService, RabbitMQService>();
builder.Services.AddHostedService<OrderCreatedConsumer>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
    ConnectionMultiplexer.Connect("localhost:6379"));
builder.Services.AddSingleton<RabbitMQ.Client.IConnectionFactory>(sp =>
    new ConnectionFactory
    {
        HostName = builder.Configuration["RabbitMQ:HostName"],
        UserName = builder.Configuration["RabbitMQ:UserName"],
        Password = builder.Configuration["RabbitMQ:Password"]
    });

//Auto Mapper
builder.Services.AddAutoMapper(typeof(MappingProfile));


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins("https://localhost:7284", "http://127.0.0.1:5501") // Or .WithOrigins("http://127.0.0.1:5500/")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

//after Cors
builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


//app.MapHub<NotificationHub>("/notificationHub");


app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting(); // ÌÃ» √‰ ÌﬂÊ‰ √Ê·«
app.UseCors("AllowAll"); // À„ CORS
app.UseAuthentication();
app.UseAuthorization();

// «·¬‰ Ì„ﬂ‰  ⁄ÌÌ‰ «· hubs Ê«· controllers
app.MapHub<ProductHub>("/productHub");
app.MapControllers();

app.Run();
