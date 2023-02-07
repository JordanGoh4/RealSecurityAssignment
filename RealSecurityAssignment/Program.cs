using AspNetCore.ReCaptcha;
using Microsoft.AspNetCore.Identity;
using RealSecurityAssignment.Model;
using RealSecurityAssignment.Services;
using Microsoft.AspNetCore.Identity.UI.Services;
using SendGrid.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);
var provider = builder.Services.BuildServiceProvider();
var configuration = provider.GetRequiredService<IConfiguration>();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDbContext<AuthDbContext>();
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddScoped<AuditServices>();
builder.Services.AddScoped<EmailSender>();
builder.Services.AddScoped<UserServices>();
builder.Services.AddControllersWithViews();
builder.Services.AddAuthentication()
        .AddGoogle(googleOptions =>
        {
            googleOptions.ClientId = configuration["Google:clientID"];
            googleOptions.ClientSecret = configuration["Google:clientSecret"];
            //googleOptions.ClaimActions.MapJsonKey("image", "picture", "url");
            //googleOptions.Scope.Add("https://www.googleapis.com/auth/user.birthday.read");
            //googleOptions.SaveTokens = true;
        });
builder.Services.AddSendGrid(options =>
    options.ApiKey = configuration["SendGrid:Key"]
                        ?? throw new Exception("The 'SendGridApiKey' is not configured")
);


//builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<AuthDbContext>();
builder.Services.AddIdentity<Class, IdentityRole>().AddEntityFrameworkStores<AuthDbContext>().AddDefaultTokenProviders();

builder.Services.AddAuthentication("MyCookieAuth").AddCookie("MyCookieAuth", options
=>
{
    options.Cookie.Name = "MyCookieAuth";
    options.AccessDeniedPath = "/Account/AccessDenied";
});
builder.Services.Configure<SecurityStampValidatorOptions>(x =>
{
    x.ValidationInterval = TimeSpan.Zero;
});
builder.Services.AddAuthorization(options =>
{
   
});
//Session Timeout
builder.Services.ConfigureApplicationCookie(Config =>
{
    Config.LoginPath = "/Login";
    Config.LogoutPath = "/Login";
    Config.ExpireTimeSpan = TimeSpan.FromMinutes(30);
});
//Recaptcha
builder.Services.AddReCaptcha(builder.Configuration.GetSection("ReCaptcha"));
//Lockout
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 3;
    options.Lockout.AllowedForNewUsers = true;
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseStatusCodePagesWithRedirects("/errors/{0}");
app.UseExceptionHandler("/errors/500");

app.UseRouting();
//app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

app.MapRazorPages();

app.Run();
