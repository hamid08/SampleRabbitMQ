using DotNetCore.CAP;
using Quartz;

namespace Publisher.Jobs
{
    public class SendEmailJob : IJob
    {
        private readonly ICapPublisher _capBus;

        public SendEmailJob(ICapPublisher capPublisher)
        {
            _capBus = capPublisher;

        }
        public async Task Execute(IJobExecutionContext context)
        {
           await _capBus.PublishAsync("place.order.qty.deducted",
                  contentObj: new { OrderId = 1234, ProductId = 23255, Qty = 1 },
                  callbackName: "place.order.mark.status");

        }
    }
}
