using MassTransit.Core.Enums;

namespace MassTransit.Worker.Data.Entities
{
    public class Nfe
    {
        public Nfe(int id, string clientName, DateTime date, double total, EnumStatusNFe status)
        {
            Id = id;
            ClientName = clientName;
            Date = date;
            Total = total;
            Status = status;
        }

        public int Id { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public double Total { get; set; }
        public EnumStatusNFe Status { get; set; }
    }
}
