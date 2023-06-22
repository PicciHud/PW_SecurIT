using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SecurITPW.Data;

namespace SecurITPW
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Configurazione del database
            services.AddDbContext<SecurITPWContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            // Configurazione di Identity
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<SecurITPWContext>();


            // INIZIO CONFIGURAZIONE per assegnare il ruolo Admin
            services.AddIdentity<IdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            // Registro del ruolo di amministratore nel servizio di gestione dei ruoli
            services.Configure<IdentityOptions>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedAccount = false;
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;
            });

            // Configurazione dell'autenticazione e dell'autorizzazione
            services.AddAuthentication()
                .AddCookie(options =>
                {
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    options.Cookie.SameSite = SameSiteMode.Strict;
                });

            // Configurazione dei requisiti di autorizzazione per il ruolo di amministratore
            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy =>
                    policy.RequireRole("Admin"));
            });
            // FINE CONFIGURAZIONE per assegnare il ruolo Admin

            // Altre configurazioni e servizi
            // ...

            services.AddRazorPages();
        }

        public async void Configure(IApplicationBuilder app, IWebHostEnvironment env, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Configurazione dell'ambiente di sviluppo o di produzione
            // ...

            app.UseRouting();

            // Middleware per l'autenticazione e l'autorizzazione
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });

            // INIZIO CONFIGURAZIONE per assegnare il ruolo Admin
            string adminRole = "Admin";
            if (!await roleManager.RoleExistsAsync(adminRole))
            {
                await roleManager.CreateAsync(new IdentityRole(adminRole));
            }

            // Assegnazione del ruolo di amministratore a un utente specifico
            string adminEmail = "alessandro.bonaldo@stud.itsaltoadriatico.it";
            IdentityUser user = await userManager.FindByEmailAsync(adminEmail);
            if (user != null)
            {
                await userManager.AddToRoleAsync(user, adminRole);
            }
            // FINE CONFIGURAZIONE per assegnare il ruolo Admin
        }
    }
}

