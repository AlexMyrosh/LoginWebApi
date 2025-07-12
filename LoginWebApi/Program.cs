
using LoginWebApi.Database;
using Microsoft.EntityFrameworkCore;

namespace LoginWebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlite("Data Source=users.db"));
            builder.Services.AddControllers();
            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
            }

            app.MapControllers();
            app.Run();
        }
    }
}
