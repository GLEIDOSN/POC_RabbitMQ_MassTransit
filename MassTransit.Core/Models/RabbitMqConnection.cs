namespace MassTransit.Core.Models
{
    public class RabbitMqConnection
    {
        public string HostName { get; set; } = string.Empty;
        public ushort Port { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool UseSSL { get; set; }
        public ushort PortSSL { get; set; }
        public string CertificatePath { get; set; } = string.Empty;
        public string CertificatePassphrase { get; set; } = string.Empty;
    }
}
