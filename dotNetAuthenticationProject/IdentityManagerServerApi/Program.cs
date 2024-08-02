using IdentityManagerServerApi.Data;
using IdentityManagerServerApi.Data.Service;
using IdentityManagerServerApi.Repositories;  
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Security.Cryptography.Xml;
using System.Text;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Conncetion string not found");
//starting
builder.Services.AddDbContext<AppDbContext>(options =>
{

    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});

//Add Identity JWT Authentication
//Identity<
builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<AppDbContext>()
                                                             .AddSignInManager()
                                                             .AddRoles<IdentityRole>();

//jwt 

builder.Services.AddAuthentication(options =>   //What it does: It sets up the authentication mechanism that the app will use          //By calling AddAuthentication, you're telling the application which authentication method (or methods) it should use to verify users.
{

    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;            //It's like telling the app, "By default, use JWT tokens to verify and challenge users."

}).AddJwtBearer(options => {

    options.TokenValidationParameters = new TokenValidationParameters       //Purpose: This line starts defining the rules for how the JWT tokens should be validated.What it does: It specifies what parts of the token should be checked to ensure its validity.Simpler Terms: Imagine setting the rules for checking if an ID card is real or fake.
    {
        ValidateIssuer = true,          //Purpose: This ensures that the token was issued by a trusted issuer.What it does: It checks the "issuer" part of the token to make sure it's from the expected source.Simpler Terms: It's like checking who gave out the ID card to make sure it's a trusted authority.
        
        ValidateAudience = true,        //Purpose: This ensures that the token is meant for the intended audience. What it does: It checks the "audience" part of the token to confirm it's for your app. Simpler Terms: It's like checking if the ID card is meant to be used at your venue.

        ValidateIssuerSigningKey = true,    //Purpose: This ensures that the token's signature is valid and was created using a trusted key. What it does: It checks the signature of the token to ensure it hasn't been tampered with. Simpler Terms: It's like checking the special watermark or seal on the ID card to make sure it's genuine.

        ValidateLifetime = true,            //Purpose: This ensures that the token has not expired. What it does: It checks the "expiration" date of the token to make sure it's still valid. Simpler Terms: It's like checking if the ID card is still within its valid date range.

        ValidIssuer = builder.Configuration["Jwt:Issuer"],          //Purpose: This sets the expected issuer value. What it does: It gets the expected issuer value from the app's configuration (usually in appsettings.json). Simpler Terms: It's like saying, "Only accept ID cards issued by this specific authority."

        ValidAudience = builder.Configuration["Jwt: Audience"],    //Purpose: This sets the expected audience value. What it does: It gets the expected audience value from the app's configuration. Simpler Terms: It's like saying, "This ID card should only be valid for people coming to this specific venue." 

        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))      //Purpose: This sets the key used to sign the token. What it does: It gets the signing key from the app's configuration and creates a security key from it.Simpler Terms: It's like getting the secret seal used to verify the authenticity of the ID cards.
    };
});

//add authenticaton to swagger ui
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth", new OpenApiSecurityScheme
    {

        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
    });
    options.OperationFilter<SecurityRequirementsOperationFilter>();

});

builder.Services.AddScoped<IUserAccount, AccountRepository>();

//Ending

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

//meka add krnnath one
app.UseAuthentication();

app.UseAuthorization();

//start swagger ui eka setup krgnna widiyai meka 
app.UseSwagger();

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    c.RoutePrefix = string.Empty; // Serve Swagger UI at the app's root
});

//end swagger ui eka setup krn iwai methanin

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();


