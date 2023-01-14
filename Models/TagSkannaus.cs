namespace Kipa_plus.Models
{
    public class TagSkannaus
    {
        public int? Id { get; set; }
        public int? VartioId { get; set; }
        public int RastiId { get; set; }
        public string TagSerial { get; set; }
        public string? TimeStamp { get; set; }
        public bool? isTulo { get; set; }
    }
}
