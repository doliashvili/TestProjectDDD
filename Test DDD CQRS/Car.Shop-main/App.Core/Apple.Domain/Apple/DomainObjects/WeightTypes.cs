using System;

namespace Apple.Domain.Apple.DomainObjects
{
    [Flags]
    public enum WeightTypes : byte
    {
        Gr = 1,
        Kg = 2,
    }
}