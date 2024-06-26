using CVTool.Data;
using CVTool.Models.AddResume;
using CVTool.Models.DeleteResume;
using CVTool.Models.GetResume;
using CVTool.Models.GetUserResumes;
using CVTool.Services.ResumeService;
using CVTool.Validators;
using CVTool.Validators.Resolver;
using FluentValidation;
using System.Reflection;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using CVTool.Services.ExternalAuthService;
using CVTool.Services.UserService;
using CVTool.Services.JwtUtils;
using CVTool.Models.Users;
using Amazon.S3;
using CVTool.Models.Files;
using Microsoft.EntityFrameworkCore;
using CVTool.Services.FilesService;
using CVTool.Filters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddAuthentication()
    .AddGoogle("google", opt =>
    {
        var googleAuth = builder.Configuration.GetSection("Authentication:Google");
        opt.ClientId = googleAuth["ClientId"];
        opt.ClientSecret = googleAuth["ClientSecret"];
        opt.SignInScheme = IdentityConstants.ExternalScheme;
    });

builder.Services.AddValidatorsFromAssemblyContaining<AddResumeRequestDTOValidator>();
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
builder.Services.AddDbContext<DataContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("WebApiDatabaseProd");

    options.UseSqlServer(connectionString);
});
builder.Services.AddScoped<IResumeService, ResumeService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
builder.Services.AddAWSService<IAmazonS3>();
builder.Services.AddScoped<IValidator<AddResumeRequestDTO>, AddResumeRequestDTOValidator>();
builder.Services.AddScoped<IValidator<GetFileByKeyRequestDTO>, GetFileByKeyRequestDTOValidator>();
builder.Services.AddScoped<IValidator<DeleteResumeRequestDTO>, DeleteResumeRequestDTOValidator>();
builder.Services.AddScoped<IValidator<GetResumeRequestDTO>, GetResumeRequestDTOValidator>();
builder.Services.AddScoped<IValidatorsResolver, ValidatorsResolver>();
builder.Services.AddScoped<IExternalAuthJwtHandler, ExternalAuthJwtHandler>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IJwtUtils, JwtUtils>();
builder.Services.AddScoped<IFilesService, FilesService>();
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
builder.Services.Configure<FileSettings>(builder.Configuration.GetSection("FileSettings"));
builder.Services.AddScoped<IValidator<GetResumeByUserRequestDTO>, GetUserResumesRequestDTOValidator>();
builder.Services.Configure<FormOptions>(o =>
{
    o.ValueLengthLimit = int.MaxValue;
    o.MultipartBodyLengthLimit = int.MaxValue;
    o.MemoryBufferThreshold = int.MaxValue;
});

var app = builder.Build();

app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(option => option
               .AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader());

app.UseStaticFiles();
app.UseMiddleware<ErrorHandlerMiddleware>();
app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }
