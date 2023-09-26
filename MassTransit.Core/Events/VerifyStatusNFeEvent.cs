using MassTransit.Core.Enums;

namespace MassTransit.Core.Events
{
    public class VerifyStatusNFeEvent
    {

        public VerifyStatusNFeEvent(int id, EnumStatusNFe status)
        {
            Id = id;
            Status = status;
        }

        public int Id { get; set; }
        public EnumStatusNFe Status { get; set; }
    }
}
