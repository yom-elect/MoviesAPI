using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MoviesAPI.Services
{
    public class WriteToFileHostedService : IHostedService
    {
        private readonly IWebHostEnvironment _env;
        private readonly string fileName = "File1.txt";
        private Timer timer;
        public WriteToFileHostedService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            WriteToFile("Process Started");
            timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            WriteToFile("Process Stopped");
            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            WriteToFile("Process ongoing" + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
        }
        private void WriteToFile(string message)
        {
            var path = $@"{_env.ContentRootPath}\wwwroot\{fileName}";
            using (StreamWriter writer = new StreamWriter(path, append: true))
            {
                writer.WriteLine(message);
            }
        }
    }
}
