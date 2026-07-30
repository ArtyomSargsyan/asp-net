using FluentValidation;
using MediatR;
using ToDoApi.Behaviors;
using ToDoApi.Infrastructure.Storage;
using ToDoApi.Repositories;
using ToDoApi.Repositories.Categories;
using ToDoApi.Repositories.Products;
using ToDoApi.Repositories.RefreshTokens;
using ToDoApi.Repositories.Users;
using ToDoApi.Services.Auth;
using ToDoApi.Services.Categories;
using ToDoApi.Services.Articles;
using ToDoApi.Services.Contacts;
using ToDoApi.Services.ProductModel;
using ToDoApi.Services.Products;

namespace ToDoApi.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // ── Infrastructure

        // Real clock for production; tests inject FakeTimeProvider instead.
        services.AddSingleton(TimeProvider.System);

        // JwtService reads config once at startup — safe as a singleton.
        // Depends on IConfiguration (singleton) and TimeProvider (singleton above).
        services.AddSingleton<JwtService>();

        // ── Repositories ──────────────────────────────────────────────────────

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        // ── Domain services ───────────────────────────────────────────────────

        services.AddScoped<IArticleService, ArticleService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IProductModelService, ProductModelService>();
        services.AddScoped<IEmailService, SmtpEmailService>();
        services.AddScoped<IContactService, ContactService>();
        // ── Strategies / Factories / Managers ─────────────────────────────────

        services.AddScoped<EmailSendStrategy>();
        services.AddScoped<ISendStrategyFactory, SendStrategyFactory>();
        services.AddScoped<INotificationManager, NotificationManager>();
        // ---- Orders
        services.AddScoped<IOrderRepository, OrderRepository>();

        // --- Coupons
        services.AddScoped<ICouponRepository, CouponRepository>();

        // ── Storage ───────────────────────────────────────────────────────────
        services.AddSingleton<IBookStore, InMemoryBookStore>();

        // ── Cross-cutting concerns ─────────────────────────────────────────────

        services.AddErrorHandling();

        services.AddValidatorsFromAssembly(typeof(Program).Assembly);

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);

            // C# 12 collection expression for the open-generic behavior type args.
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });

        return services;
    }
}
