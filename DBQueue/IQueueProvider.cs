using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBQueue
{
    public interface IQueueProvider
    {
        IConsumer GetQueueConsumer();
        IPublisher GetQueuePublisher();
    }
}
