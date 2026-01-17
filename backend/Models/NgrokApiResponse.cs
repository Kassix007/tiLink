namespace backend.Models
{
    public class NgrokApiResponse
    {
        public List<NgrokTunnel> tunnels { get; set; } = [];
    }

    public class NgrokTunnel
    {
        public string public_url { get; set; } = string.Empty;
        public string proto { get; set; } = string.Empty;
    }

}
