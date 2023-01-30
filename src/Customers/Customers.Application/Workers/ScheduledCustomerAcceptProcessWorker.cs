﻿using Customers.Application.Abstractions;
using Customers.Domain.Models;
using Library.Messaging;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;
using System.Threading.Tasks;

namespace Customers.Application.Workers
{
    public class ScheduledCustomerAcceptProcessWorker : RpcServer<object>
    {
        public const string MessageReceived = "Customers message received";
        private readonly ICustomerRepositoryFactory _repositoryFactory;

        public ScheduledCustomerAcceptProcessWorker(IConnectionFactory factory, ICustomerRepositoryFactory repositoryFactory, ILogger logger) : base(nameof(Customer), factory, logger)
        {
            _repositoryFactory = repositoryFactory;
        }

        public override Task<(object receivedValue, string receivedMessage)> HandleReceivedMessage(BasicDeliverEventArgs ea) => Task.FromResult((default(object), MessageReceived));

        public override async Task<string> WriteResponseMessage(object receivedValue)
        {
            var repository = _repositoryFactory.CreateRepository();
            var customers = await repository.GetAllAsync();
            return JsonSerializer.Serialize(customers);
        }
    }
}
