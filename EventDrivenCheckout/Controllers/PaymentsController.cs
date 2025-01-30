using System;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Threading.Channels;
using Confluent.Kafka;
// opt/kafka/bin
//./kafka-topics.sh --create --topic checkout-topic --bootstrap-server broker:29092
namespace EventDrivenCheckout.Controllers
{

    public class CheckoutMessage
    {
        public List<CheckoutItem> CartItems { get; set; } = new();
        public decimal TotalAmount { get; set; }
    }

    public class CheckoutItem
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }



    [Route("api/payments")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private const string KafkaBootstrapServers = "broker:9092";
        private const string KafkaTopic = "checkout-topic";


        private readonly ILogger _logger;

        public PaymentsController(ILogger<PaymentsController> logger)
        {
            _logger = logger;
        }


        [HttpPost("checkout")]
        public async Task<ActionResult> CheckoutAsync()
        {
            if (!CartController.Cart.Any())
                return BadRequest("Cart is empty.");

            decimal totalAmount = CartController.Cart.Sum(item => item.Products.Price * item.Quantity);
            var checkoutMessage = new CheckoutMessage
            {
                CartItems = CartController.Cart.Select(item => new CheckoutItem
                {
                    ProductId = item.Products.Id,
                    Quantity = item.Quantity
                }).ToList(),
                TotalAmount = totalAmount
            };

            SendCheckoutMessage(checkoutMessage, _logger);
            _logger.LogDebug("Payment sent to task queue");
            return Ok(new { Message = "Checkout initiated. Payment will be processed shortly." });
        }

        private void SendCheckoutMessage(CheckoutMessage message, ILogger _logger)
        {
            var config = new ProducerConfig { BootstrapServers = KafkaBootstrapServers };

            using var producer = new ProducerBuilder<Null, string>(config).Build();
            var messageBody = JsonSerializer.Serialize(message);

            try
            {
                producer.Produce(KafkaTopic, new Message<Null, string> { Value = messageBody }, (deliveryReport) =>
                {
                    if (deliveryReport.Error.IsError)
                    {
                        _logger.LogError($"Delivery error: {deliveryReport.Error.Reason}");
                    }
                    else
                    {
                        _logger.LogError($"Message delivered to {deliveryReport.TopicPartitionOffset}");
                    }
                });

                producer.Flush(TimeSpan.FromSeconds(10));
            }
            catch (ProduceException<Null, string> ex)
            {
                _logger.LogError($"Kafka produce error: {ex.Error.Reason}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending Kafka message: {ex.Message}");
            }
        }
    }
}
