using FormSite.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

var connStr = builder.Configuration.GetConnectionString("Default")
             ?? builder.Configuration["ConnectionStrings:Default"]
             ?? throw new InvalidOperationException("Missing connection string 'ConnectionStrings:Default'");

builder.Services.AddDbContext<AppDbContext>(opt => opt.UseNpgsql(connStr));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapRazorPages();

// aplica migrations + seed adicional se quiser
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();

    // Seed extra (somente se preferir seed aqui, além do HasData)
    if (!await db.Options.AnyAsync())
    {
        db.Options.AddRange(
            new FormSite.Models.Option { Name = "Item A" },
            new FormSite.Models.Option { Name = "Item B" },
            new FormSite.Models.Option { Name = "Item C" }
        );
        await db.SaveChangesAsync();
    }
}

app.Run();
