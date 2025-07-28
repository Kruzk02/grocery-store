using API.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite("Data Source=user.db"));

builder.Services.AddIdentityApiEndpoints<IdentityUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddAuthorization();

builder.Services.AddControllers();

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapIdentityApi<IdentityUser>();
app.UseHttpsRedirection();
app.MapControllers();

app.MapPost("/logout", async (SignInManager<IdentityUser> signInManager) => 
    {
        await signInManager.SignOutAsync();
        return Results.Ok(new { message = "Logged out successfully" });
    })
    .WithOpenApi()
    .RequireAuthorization();

app.Run();