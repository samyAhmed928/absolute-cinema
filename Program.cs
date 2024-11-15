using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MoviesApi.Data;
using MoviesApi.Services;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddTransient<IGenreService, GenreService>();
builder.Services.AddTransient<IMoviesService, MoviesService>();

builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddDbContext<ApplictionDbContext>(options =>
	options.UseSqlServer(
		 builder.Configuration.GetConnectionString("DefultConnection")
	)) ;
builder.Services.AddCors();
builder.Services.AddSwaggerGen(options =>
{
	options.SwaggerDoc("v1", new OpenApiInfo
	{
		Version = "v1",
		Title= "Test Api",
		Description="Mu first Api",
		TermsOfService=new Uri("https://www.google.com"),
		Contact=new OpenApiContact
		{
			Name="Samy",
			Email="Samy@gmail.com"
		},
		License=new OpenApiLicense
		{
			Name = "My License",
			Url=new Uri("https://www.google.com"),
		}
		
	});

	options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
	{
		Name="Authorization",
		Type=SecuritySchemeType.ApiKey,
		Scheme= "Bearer",
		BearerFormat="JWT",
		In=ParameterLocation.Header,
		Description="Enter your jwt key"
	});

	options.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
		{
			new OpenApiSecurityScheme
			{
				Reference=new OpenApiReference
				{
					Type=ReferenceType.SecurityScheme,
					Id="Bearer"
				},
				Name="Bearer",
				In=ParameterLocation.Header
			},
			new List<string>()
		}
	});
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(c=>c.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
app.UseAuthorization();

app.MapControllers();

app.Run();


