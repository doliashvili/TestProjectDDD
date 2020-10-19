using App.Core.Domain;

namespace Apple.Domain.Apple.DomainObjects
{
    public class Weight : ValueObject<Weight>
    {
        public decimal Value { get; set; }
        public WeightTypes Type { get; set; }
    }
}