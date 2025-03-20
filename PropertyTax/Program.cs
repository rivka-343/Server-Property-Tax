using Amazon.S3;
using Microsoft.EntityFrameworkCore;
using PropertyTax.Core;
using PropertyTax.Core.Repositories;
using PropertyTax.Core.Services;
using PropertyTax.Data;
using PropertyTax.Data.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using PropertyTax.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using PropertyTax.Servise;

using Amazon.Runtime;
using Microsoft.Extensions.Configuration;


namespace PropertyTax
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,


                    ValidIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER"),
                    ValidAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_KEY") ?? throw new ArgumentNullException("JWT_KEY")))
                //ValidIssuer = builder.Configuration["Jwt:Issuer"],
                //    ValidAudience = builder.Configuration["Jwt:Audience"],
                //    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                };
            });
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("ResidentOnly", policy => policy.RequireRole("Resident"));
                options.AddPolicy("EmployeeOrManager", policy => policy.RequireRole("Employee", "Manager"));
                options.AddPolicy("ManagerOnly", policy => policy.RequireRole("Manager"));
            });
           // builder.Services.AddAWSService<IAmazonS3>(builder.Configuration.GetAWSOptions());
            //builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
            builder.Services.AddAWSService<IAmazonS3>();

            builder.Services.AddControllers();
            builder.Services.AddScoped<IRequestService, RequestService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IS3Service, S3Service>();
            builder.Services.AddScoped<IUsersService, UsersService>();
            builder.Services.AddScoped<IOpenAiService, OpenAiService>();
            builder.Services.AddHttpClient<OpenAiService>();

            builder.Services.AddScoped<IRequestRepository, RequestRepository>();
            builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();
            builder.Services.AddScoped<IUsersRepository, UsersRepository>();
            builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

            //builder.Services.AddDbContext<ApplicationDbContext>(
            //        options => options.UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Database=MyDB"));
            //builder.Services.AddDbContext<ApplicationDbContext>(options =>
            //        options.UseSqlServer("mysql://uyevotthdfrhj5kk:MAA9kQJwXSvw7tRIXhMA@bv3fmicofty9tcfcxbov-mysql.services.clever-cloud.com:3306/bv3fmicofty9tcfcxbov"));
            //   var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL");
            //  var connectionString = "mysql://uyevotthdfrhj5kk:MAA9kQJwXSvw7tRIXhMA@bv3fmicofty9tcfcxbov-mysql.services.clever-cloud.com:3306/bv3fmicofty9tcfcxbov";
            var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL") ?? throw new Exception("Missing DATABASE_URL in environment variables.");

            // var connectionString = "server=bv3fmicofty9tcfcxbov-mysql.services.clever-cloud.com;database=bv3fmicofty9tcfcxbov;user=uyevotthdfrhj5kk;password=MAA9kQJwXSvw7tRIXhMA;port=3306;";
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
            builder.Services.AddAutoMapper(typeof(Mapping));
            builder.Services.AddSwaggerGen();
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' [space] and your token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {{new OpenApiSecurityScheme{
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },new string[] { }  }});
            });
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                  builder => builder.WithOrigins("http://localhost:5173", "https://server-property-tax.onrender.com") // מקור ה-React שלך
                          .AllowAnyMethod()
                    .AllowAnyHeader());
            });
            builder.Services.AddEndpointsApiExplorer();
            var app = builder.Build();
            app.UseCors();
            //if (app.Environment.IsDevelopment())
            //{
            //    app.UseSwagger();
            //    app.UseSwaggerUI();
            //}
            if (app.Environment.IsDevelopment() || Environment.GetEnvironmentVariable("ENABLE_SWAGGER") == "true") {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
