using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ShopApp.Business.Abstract;
using ShopApp.Business.Concrete;
using ShopApp.DataAccess.Abstract;
using ShopApp.DataAccess.Concrete.SQL;
using ShopApp.WebUI.EmailService;
using ShopApp.WebUI.Identity;
using Stripe;

namespace ShopApp.WebUI
{
    public class Startup
    {
        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddDbContextPool<ShopContext>(options =>
            //{
            //    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            //});
            //optionsBuilder.UseSqlServer("server=localhost;database=ShopDB;Trusted_Connection=True");

            services.AddDbContext<ApplicationContext>(options => options.UseSqlServer("server=localhost;database=ShopDB;Trusted_Connection=True;MultipleActiveResultSets=true"));
            services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationContext>()
                .AddDefaultTokenProviders();
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;
            });

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/account/login";
                options.LogoutPath = "/account/logout";
                options.AccessDeniedPath = "/account/accessdenied";
                options.SlidingExpiration = true;
                options.ExpireTimeSpan = TimeSpan.FromDays(30);
                options.Cookie = new CookieBuilder
                {
                    HttpOnly = true,
                    Name=".shopapp.security.cookie",
                    SameSite=SameSiteMode.Strict
                };
            });
            services.AddScoped<ICategoryService, CategoryManager>();
            services.AddScoped<IProductService, ProductManager>();
            services.AddScoped<ICardService, CardManager>();
            services.AddScoped<IOrderService, OrderManager>();

            services.AddScoped<IOrderRepository, SQLOrderRepository>();
            services.AddScoped<IProductRepository, SQLProductRepository>();
            services.AddScoped<ICategoryRepository, SQLCategoryRepository>();
            services.AddScoped<ICardRepository, SQLCardRepository>();

            services.AddScoped<IEmailSender, SmtpEmailSender>(email =>
                new SmtpEmailSender(
                    configuration["EmailSender:Host"],
                    configuration.GetValue<int>("EmailSender:Port"),
                    configuration.GetValue<bool>("EmailSender:EnableSSL"),
                    configuration["EmailSender:Username"],
                    configuration["EmailSender:Password"]
                    )
                );
            services.AddControllersWithViews();
            //StripeConfiguration.ApiKey = configuration.GetSection("Stripe")["SecretKey"];
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IConfiguration configuration,UserManager<ApplicationUser> user, RoleManager<IdentityRole> roleManager)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();
            app.UseStaticFiles();
            app.UseEndpoints(endpoints =>
            {
                //  endpoints.MapControllerRoute(
                //  name: "adminProducts",
                //  pattern: "admin/products",
                //  defaults: new { controller = "Admin", action = "ProductList" }
                //);

                //  endpoints.MapControllerRoute(
                //     name: "adminProducts",
                //      pattern: "admin/products/{id?}",
                //      defaults: new { controller = "Admin", action = "EditProduct" }
                //  );


                //  endpoints.MapControllerRoute(
                //   name: "products",
                //   pattern: "products/{category?}",
                //   defaults: new { controller = "Shop", action = "List" }
                //           );


                //  endpoints.MapControllerRoute(
                //      name: "search",
                //      pattern: "search",
                //      defaults: new { controller = "Shop", action = "Search" }
                //      );

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}"
                    );
            });
            SeedIdentity.Seed(user,roleManager,configuration).Wait();
        }
    }
}
