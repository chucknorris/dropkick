namespace dropkick.Tasks.Iis
{
    public class IisSiteBinding
    {
        public IisSiteBinding()
        {
            Protocol = "http";
            Port = 80;
        }

        public string Protocol { get; set; }
        public int Port { get; set; }
    }
}