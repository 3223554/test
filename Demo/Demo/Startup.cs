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
                    var customerIdString = context.Request.RouteValues["customerid"].ToString();
                    var scoreString = context.Request.RouteValues["score"].ToString();

                    if (!long.TryParse(customerIdString, out long customerId) || !decimal.TryParse(scoreString, out decimal score))
                    {
                        context.Response.StatusCode = StatusCodes.Status400BadRequest;
                        await context.Response.WriteAsync("Invalid input parameters.");
                        return;
                    }

                    var updatedScore = leaderboardService.UpdateScore(customerId, score);

                    await context.Response.WriteAsync(updatedScore.ToString());
                });

                endpoints.MapGet("/leaderboard", async context =>
                {
                    var startString = context.Request.Query["start"];
                    var endString = context.Request.Query["end"];

                    if (!int.TryParse(startString, out int start) || !int.TryParse(endString, out int end))
                    {
                        context.Response.StatusCode = StatusCodes.Status400BadRequest;
                        await context.Response.WriteAsync("Invalid input parameters.");
                        return;
                    }

                    var customers = leaderboardService.GetCustomersByRank(start, end);

                    await context.Response.WriteAsJsonAsync(customers);
                });

                endpoints.MapGet("/leaderboard/{customerid}", async context =>
                {
                    var customerIdString = context.Request.RouteValues["customerid"].ToString();
                    var highString = context.Request.Query["high"];
                    var lowString = context.Request.Query["low"];

                    if (!long.TryParse(customerIdString, out long customerId) ||
                        !int.TryParse(highString, out int high) ||
                        !int.TryParse(lowString, out int low))
                    {
                        context.Response.StatusCode = StatusCodes.Status400BadRequest;
                        await context.Response.WriteAsync("Invalid input parameters.");
                        return;
                    }

                    var customers = leaderboardService.GetCustomersByCustomerId(customerId, high, low);

                    await context.Response.WriteAsJsonAsync(customers);
                });
            });

            //var testMain = app.ApplicationServices.GetService<TestMain>();
            //await testMain.RunConcurrentTest();

        }
    }

}

