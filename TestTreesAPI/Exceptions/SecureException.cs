namespace TestTreesAPI.Exceptions
{
    public class SecureException : Exception
    {
        public Guid EventId { get; set; }

        public SecureException(string message) : base(message)
        {
            EventId = Guid.NewGuid();
        }
    }
}