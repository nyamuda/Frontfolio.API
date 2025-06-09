using Frontfolio.API.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;



var builder = WebApplication.CreateBuilder(args);

//Register services
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<EmailSenderService>();
builder.Services.AddScoped<HtmlTemplateService>();
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<OtpService>();
builder.Services.AddScoped<UserService>();



//add database connection string
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new KeyNotFoundException("Connection string not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

//Configure JWT Authentication
string jwtIssuer = builder.Configuration.GetValue<string>("Authentication:Jwt:Issuer") ?? throw new KeyNotFoundException("Jwt issuer field not found.");
string jwtAudience = builder.Configuration.GetValue<string>("Authentication:Jwt:Audience") ?? throw new KeyNotFoundException("Jwt audience field not found.");
string jwtKey = builder.Configuration.GetValue<string>("Authentication:Jwt:Key") ?? throw new KeyNotFoundException("Jwt key field not found.");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});
builder.Services.AddAuthorization();

//Add CORS policy
var AllowAnyOrigin = "_allowAnyOrigin";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: AllowAnyOrigin, policy =>
    {
        policy
        .AllowAnyOrigin()
        .AllowAnyHeader();

    });
});



builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// Redirect HTTP to HTTPS
app.UseHttpsRedirection();

// CORS middleware (should come before any auth)
app.UseCors(AllowAnyOrigin);

// Authentication middleware
app.UseAuthentication();

// Authorization middleware
app.UseAuthorization();

app.MapControllers();

//root API route and its handler
app.MapGet("/api", () => "Welcome to the Frontfolio API.");

app.Run();
