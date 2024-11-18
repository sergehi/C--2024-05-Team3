namespace Common.SignalR
{
    public class SignalMessage : ICloneable
    {
        public Guid SenderModule { get; set; } = Guid.Empty;
        public Guid SenderEntity { get; set; } = Guid.Empty;


        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public List<Guid> Recipients { get; set; } = new List<Guid>();

        public object Clone()
        {
            var clone = new SignalMessage()
            {
                SenderModule = SenderModule,
                SenderEntity = SenderEntity,
                Title = Title,
                Body = Body
            };
            if (null != Recipients && Recipients.Count > 0)
                clone.Recipients.AddRange(Recipients);
            return clone;
        }
    }
}
