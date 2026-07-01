namespace MedAnalyzer.Core.Application.Base
{
    public abstract class BaseDto<Type>
    {
        public Type Id { get; set; } = default!;
        public  DateTime? CreatedAt { get; set; } = DateTime.UtcNow;


    }
}
