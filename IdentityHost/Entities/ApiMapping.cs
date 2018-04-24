namespace IdentityHost.Entities
{
    public class ApiMapping
    {
        public int Id { get; set; }

        public string ClientId { get; set; }
        public string UserId { get; set; }
        public string CoreToken { get; set; }
        public string CoreApiEndPoint { get; set; }
    }
}