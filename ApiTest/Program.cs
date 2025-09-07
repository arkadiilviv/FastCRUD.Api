using FasterAPI;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Server.IISIntegration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FastCRUD.Api;

var builder = WebApplication.CreateBuilder(args);
const string jwtKey = "arkasha_and_serhii_vlakh_gay_lovers_fr_fr";
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger to use the JWT Bearer token
builder.Services.AddSwaggerGen(setup =>
{
	var jwtSecurityScheme = new OpenApiSecurityScheme
	{
		BearerFormat = "JWT",
		Name = "JWT Authentication",
		In = ParameterLocation.Header,
		Type = SecuritySchemeType.Http,
		Scheme = JwtBearerDefaults.AuthenticationScheme,
		Description = "Put **_ONLY_** your JWT Bearer token on textbox below!",

		Reference = new OpenApiReference
		{
			Id = JwtBearerDefaults.AuthenticationScheme,
			Type = ReferenceType.SecurityScheme
		}
	};

	setup.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

	setup.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
		{ jwtSecurityScheme, Array.Empty<string>() }
	});
});

// Add JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options =>
	{
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = true,
			ValidateAudience = true,
			ValidateLifetime = true,
			ValidateIssuerSigningKey = true,
			ValidIssuer = "fast.com",
			ValidAudience = "fast.com",
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
		};
	});
builder.Services.AddAuthorization();

WebApplication? app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

// Use the FastCRUD to generate endpoints for HumanModel with authentication
{
	var humanRepository = new HumanRepository();
	app.GenerateCRUDWithAuth(humanRepository);
}
// Use the FastCRUD to generate endpoints for HumanModel without authentication
{
	var humanRepository = new HumanRepositoryWOAuth();
	app.GenerateGet(humanRepository);
	app.GenerateGetById(humanRepository);
	app.GeneratePost(humanRepository);
	app.GeneratePut(humanRepository);
	app.GenerateDelete(humanRepository);
}

app.MapPost("/Login", (string login, string password) =>
{
	if (login == "admin" && password == "admin")
	{
		var claims = new[]
		{
			new Claim(JwtRegisteredClaimNames.Sub, login),
			new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
		};

		var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
		var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

		var token = new JwtSecurityToken(
			issuer: "fast.com",
			audience: "fast.com",
			claims: claims,
			expires: DateTime.Now.AddMinutes(30),
			signingCredentials: creds);
		return Results.Ok(new JwtSecurityTokenHandler().WriteToken(token));
	}
	return Results.Unauthorized();
});

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
