namespace MedAnalyzer.Core.Application.Base
{
    public abstract class BaseDto<Type>
    {
        public required Type Id { get; set; }

        public required bool IsActive { get; set; }

        public  DateTime? CreatedAt { get; set; } = DateTime.Now;


    }
}
