﻿using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus.Core;

namespace Elsa.Activities.AzureServiceBus.Services
{
    public interface IMessageReceiverFactory
    {
        Task<IMessageReceiver> GetReceiverAsync(string queueName, CancellationToken cancellationToken = default);
    }
}