namespace QuipuSolution.Domain.Common
{
    /// <summary>
    /// Thrown when a business rule of a domain entity is violated.
    /// </summary>
    public class DomainException : Exception
    {
        public DomainException(string message) : base(message) { }
    }
}
