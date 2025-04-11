using Microsoft.EntityFrameworkCore;

namespace Thunders.TechTest.OutOfBox.Database
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseAutomaticMigrations<TContext>(this IApplicationBuilder app)
            where TContext : DbContext
        {
            using var scope = app.ApplicationServices.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<TContext>();
            db.Database.Migrate();

            return app;
        }
    }
}
