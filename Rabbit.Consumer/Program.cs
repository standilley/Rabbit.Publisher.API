﻿using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using Rabbit.Models;
using Rabbit.Models.Entities;

// See https://aka.ms/new-console-template for more information

var factory = new ConnectionFactory()
{
    HostName = "localhost",
    UserName = "admin",
    Password = "123456"
};
using (var connection = factory.CreateConnection())
using (var channel = connection.CreateModel())
{
    channel.QueueDeclare(queue: "rabbitMensagesQueue",
                         durable: false,
                         exclusive: false,
                         autoDelete: false,
                         arguments: null);

    var consumer = new EventingBasicConsumer(channel);
    consumer.Received += (model, ea) =>
    {
        var body = ea.Body.ToArray();
        var json = Encoding.UTF8.GetString(body);

        RabbitMessage message = JsonSerializer.Deserialize<RabbitMessage>(json);

        Thread.Sleep(1000);

        Console.WriteLine($"Titulo: {message.Titulo}; Texto={message.Text}; Id={message.Id}");
    };
    channel.BasicConsume(queue: "rabbitMensagesQueue",
                         autoAck: true,
                         consumer: consumer);

    Console.WriteLine(" Press [enter] to exit.");
    Console.ReadLine();
}