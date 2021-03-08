using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Net.Client;
using GrpcGreeter;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GrpcBandwagon
{
    public class ClientWorker : BackgroundService
    {
        private readonly ILogger<ClientWorker> _logger;
        private readonly string _url;

        public ClientWorker(ILogger<ClientWorker> logger, IConfiguration configuration)
        {
            _logger = logger;
            _url = configuration["Kestrel:Endpoints:Http:Url"];
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var channel = GrpcChannel.ForAddress(_url);
            var client = new Greeter.GreeterClient(channel);

            while (!stoppingToken.IsCancellationRequested)
            {
                var reply = await client.SayHelloAsync(new HelloRequest
                {
                    Name = "Worker"
                });

                _logger.LogInformation($"Greeting: {reply.Message} -- {DateTime.Now}");
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}