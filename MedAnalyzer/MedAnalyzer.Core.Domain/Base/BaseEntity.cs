namespace MedAnalyzer.Core.Domain.Base
{
    public class BaseEntity<Type>
    {
        public required Type Id { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
