using System;
using System.Threading.Tasks;
using App.Core.Commands;
using App.Core.Repository;
using Core.Tests.Aggregates;
using Core.Tests.Commands;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Core.Tests
{
    public class AggregateTests
    {
        private readonly IEventStore<AppleAggregate, Guid> _eventStore;

        public AggregateTests()
        {
            var provider = DependencyInjection.GetServiceProvider();
            _eventStore = provider.GetRequiredService<IEventStore<AppleAggregate, Guid>>();
        }

        
        [Fact]
        public async Task CreateAggregate()
        {
            var id = Guid.NewGuid();
            var color = "Green";
            var createApple = new CreateApple(id, color, 
                new CommandMeta(Guid.NewGuid(), Guid.NewGuid()));
            var aggregate = new AppleAggregate(createApple);

            var events = aggregate.GetUncommittedEvents();
            await _eventStore.AppendAsync(events);
            
            Assert.Equal(id, aggregate.Id);
            Assert.Equal(aggregate.Color, color);
            Assert.Single(events);
        }


        [Fact]
        public async Task RestoreFromHistory()
        {
            var eventId = Guid.Parse("8f04bfd2-fca5-499f-94b6-37046c5bbda7");
            var agr = await _eventStore.RestoreAsync(eventId);
            
            Assert.NotNull(agr);
            Assert.NotNull(agr.Color);
            Assert.True(agr.Version > 0);
            Assert.False(agr.Id.Equals(default));
        }
        
        
        [Fact]
        public void ChangeAppleColor_and_restore_from_events()
        {
            var agr = CreateAppleAgr("Red");
            var newColor = "Green";
            
            var changeColorCommand = new ChangeAppleColor(agr.Id, 
                newColor,
                new CommandMeta(Guid.NewGuid(), Guid.NewGuid()));
            agr.ChangeAppleColor(changeColorCommand);

            var events = agr.GetUncommittedEvents();
            
            var aggregateFromHistory = new AppleAggregate();
            aggregateFromHistory.LoadFromHistory(events);
            
            
            Assert.Equal(agr.Color,newColor);
            Assert.Equal(2, events.Length);
            Assert.Equal(aggregateFromHistory.Id, agr.Id);
            Assert.Equal(aggregateFromHistory.Color, agr.Color);
        }
        
        
        private AppleAggregate CreateAppleAgr(string color) => 
            new AppleAggregate(
                new CreateApple(Guid.NewGuid(), 
                    color, 
                    new CommandMeta(Guid.NewGuid(), Guid.NewGuid())
                ));
    }
}