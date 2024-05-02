using Employee_API_JWT_1035.Identity;
using Login_Register.Repository.IRepository;
using Login_Register.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Login_Register;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using Login_Register.Models.ViewModel;
using FluentValidation.AspNetCore;
using System.Reflection;
using FluentValidation;
using Login_Register.DTO_s;
using Microsoft.Extensions.DependencyInjection;
using System;
using Project_Ecomm_App_1035_Untility;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
string cs = builder.Configuration.GetConnectionString("Cons");
builder.Services.AddEntityFrameworkSqlServer().AddDbContext<ApplicationDbContext>
 (option =>  option.UseSqlServer(cs, b =>
 b.MigrationsAssembly("Login-Register")));

builder.Services.AddScoped<ITokenRepo, TokenRepo>();
builder.Services.AddScoped<IApplyjob, ApplyJob>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IEmployee, EmployeeRepository>();
builder.Services.AddScoped<IDesignationRepository, DesignationRepository>();
builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
builder.Services.AddScoped<IEmailSender, EmailSender>();
//identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>()
   .AddDefaultTokenProviders();

//builder.Services.AddTransient<AppSettings>();
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

//builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

//FluentValidation



builder.Services.AddControllers()
            .AddFluentValidation(v =>
            {
                v.ImplicitlyValidateChildProperties = true;
            //    v.ImplicitlyValidateRootCollectionElements = true;
            // v.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            });

builder.Services.AddTransient<IValidator<Employeedto>, EmployeeValidator>();

////*******


builder.Services.AddAuthorization();

//JWT Authentication
var appsettingSection = builder.Configuration.GetSection("JWTSetting");
builder.Services.Configure<JWTSetting>(appsettingSection);
var appsetting = appsettingSection.Get<JWTSetting>();
var key = Encoding.ASCII.GetBytes(appsetting.SecretKey);

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAutoMapper(typeof(MappingProfile));
// Add Cors
builder.Services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
{
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader();
}));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


//Data
IServiceScopeFactory serviceScopeFactory=app.Services.GetRequiredService<IServiceScopeFactory>();
using (IServiceScope scope = serviceScopeFactory.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManger = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    //create role admin
    if (!await roleManager.RoleExistsAsync(SD.role_Admin))
    {
        var role = new IdentityRole();
        role.Name = SD.role_Admin;
        await roleManager.CreateAsync(role);
    }
    //create role Company
    if (!await roleManager.RoleExistsAsync(SD.role_Company))
    {
        var role = new IdentityRole();
        role.Name = SD.role_Company;
        await roleManager.CreateAsync(role);
    }
    //create role employee
    if (!await roleManager.RoleExistsAsync(SD.role_Employee))
    {
        var role = new IdentityRole();
        role.Name = SD.role_Employee;
        await roleManager.CreateAsync(role);
    }

}
//app.UseCors(policy => policy.AllowAnyHeader().
//AllowAnyMethod().SetIsOriginAllowed(origin => true).AllowCredentials());

app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Images")),
    RequestPath = new PathString("/Images")
});
app.UseCors("MyPolicy");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();