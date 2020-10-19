using System;
using App.Core.Queries;
using Apple.Domain.Apple.DomainObjects;

namespace Apple.ReadModels.Models
{
    public class AppleReadModel : IReadModel
    {
        public string Id { get; set; }
        public long Version { get; set; }
        public string Color { get; set; }
        public Weight Weight { get; set; }
    }
}