namespace MassTransit.Core.Models
{
    public class ErrorNameFormater : IErrorQueueNameFormatter
    {
        public Dictionary<string, string> QueueErrorBinds;

        public ErrorNameFormater(Dictionary<string, string> queueErrorBinds)
        {
            QueueErrorBinds = queueErrorBinds;
        }

        public string FormatErrorQueueName(string queueName)
        {
            var success = QueueErrorBinds.TryGetValue(queueName, out var queueErrorNameValue);
            if (success)
                return queueErrorNameValue;

            return queueName + "_error";
        }
    }
}
