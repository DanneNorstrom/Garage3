using Garage3.Data;

namespace Garage3.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static async Task<IApplicationBuilder> SeedDataAsync(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<Garage3Context>();

                try
                {
                    await SeedData.Init(context, services);
                }
                catch (Exception)
                {

                    throw;
                }
            }
            return app;
        }
    }
}