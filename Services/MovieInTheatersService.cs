using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MoviesAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MoviesAPI.Services
{
    public class MovieInTheatersService : IHostedService, IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private Timer _timer;

        public MovieInTheatersService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public void Dispose()
        {
            _timer?.Dispose();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromDays(1));
            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<movies_apiContext>();
                var today = DateTime.Today;
                var movies = await context.Movies.Where(x => x.ReleaseDate <= today).ToListAsync();
               if (movies.Any())
                {
                    foreach (var movie in movies)
                    {
                        movie.InTheaters = true;
                    }
                    await context.SaveChangesAsync();
                }
               
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }
    }
}
