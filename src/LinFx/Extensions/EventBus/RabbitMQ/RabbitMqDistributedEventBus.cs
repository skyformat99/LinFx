﻿using LinFx.Extensions.RabbitMQ;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading.Tasks;

namespace LinFx.Extensions.EventBus.RabbitMQ
{
    public class RabbitMqDistributedEventBus : EventBus
    {
        protected IConnectionPool ConnectionPool { get; }
        protected IConsumerFactory ConsumerFactory { get; }
        protected IRabbitMqConsumer Consumer { get; }
        protected IRabbitMqSerializer Serializer { get; }
        protected RabbitMqOptions RabbitMqOptions { get; }

        public RabbitMqDistributedEventBus(
            IConnectionPool connectionPool, 
            IConsumerFactory consumerFactory,
            IRabbitMqSerializer serializer,
            IEventBusSubscriptionsManager subscriptionsManager,
            IServiceProvider serviceProvider,
            IOptions<EventBusOptions> eventBusOptions,
            IOptions<RabbitMqOptions> rabbitMOptions)
            : base(subscriptionsManager, serviceProvider, eventBusOptions)
        {
            RabbitMqOptions = rabbitMOptions.Value;
            ConnectionPool = connectionPool;
            ConsumerFactory = consumerFactory;
            Serializer = serializer;
            _subsManager.OnEventRemoved += SubsManager_OnEventRemoved;

            Consumer = ConsumerFactory.Create(
                new ExchangeDeclareConfiguration(
                    RabbitMqOptions.BrokerName,
                        type: "direct",
                        durable: true),
                new QueueDeclareConfiguration(
                        RabbitMqOptions.QueueName,
                        durable: true,
                        exclusive: false,
                        autoDelete: false),
                RabbitMqOptions.ConnectionName
            );
            Consumer.OnMessageReceived(ProcessEventAsync);
        }
    
        public override Task PublishAsync(IntegrationEvent evt)
        {
            var eventName = evt.GetType().Name;
            var body = Serializer.Serialize(evt);

            using (var channel = ConnectionPool.Get(RabbitMqOptions.ConnectionName).CreateModel())
            {
                channel.ExchangeDeclare(
                    RabbitMqOptions.BrokerName,
                    "direct",
                    durable: true
                );

                var properties = channel.CreateBasicProperties();
                properties.DeliveryMode = RabbitMqConsts.DeliveryModes.Persistent;

                channel.BasicPublish(
                   exchange: RabbitMqOptions.BrokerName,
                    routingKey: eventName,
                    mandatory: true,
                    basicProperties: properties,
                    body: body
                );
            }

            return Task.CompletedTask;
        }

        public override void Subscribe<TEvent, THandler>()
        {
            var eventName = _subsManager.GetEventKey<TEvent>();
            var containsKey = _subsManager.HasSubscriptionsForEvent(eventName);
            if (!containsKey)
            {
                _subsManager.AddSubscription<TEvent, THandler>();
                Consumer.BindAsync(eventName);
            }
        }

        private async Task ProcessEventAsync(IModel channel, BasicDeliverEventArgs ea)
        {
            var eventName = ea.RoutingKey;
            var eventData = Encoding.UTF8.GetString(ea.Body);
            await TriggerHandlersAsync(eventName, eventData);
        }
    }
}
