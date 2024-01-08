using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Demo.Service;
using Demo.Test;
using Microsoft.AspNetCore.Builder;

namespace Demo
{
    public class Startup
    {
        private readonly LeaderboardService leaderboardService = new LeaderboardService();

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddSingleton<TestMain>();
            services.AddSingleton(leaderboardService);
        }

        public async void Configure(IApplicationBuilder app)
        {

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapPost("/customer/{customerid}/score/{score}", async context =>
                {
                    var customerId = long.Parse(context.Request.RouteValues["customerid"].ToString());
                    var score = decimal.Parse(context.Request.RouteValues["score"].ToString());

                    var updatedScore = leaderboardService.UpdateScore(customerId, score);

                    await context.Response.WriteAsync(updatedScore.ToString());
                });

                endpoints.MapGet("/leaderboard", async context =>
                {
                    var start = int.Parse(context.Request.Query["start"]);
                    var end = int.Parse(context.Request.Query["end"]);

                    var customers = leaderboardService.GetCustomersByRank(start, end);

                    await context.Response.WriteAsJsonAsync(customers);
                });

                endpoints.MapGet("/leaderboard/{customerid}", async context =>
                {
                    var customerId = long.Parse(context.Request.RouteValues["customerid"].ToString());
                    var high = int.Parse(context.Request.Query["high"]);
                    var low = int.Parse(context.Request.Query["low"]);

                    var customers = leaderboardService.GetCustomersByCustomerId(customerId, high, low);

                    await context.Response.WriteAsJsonAsync(customers);
                });
            });

            var testMain = app.ApplicationServices.GetService<TestMain>();
            //await testMain.RunConcurrentTest();

        }
    }

}

