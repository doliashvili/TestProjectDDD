using System;
using System.Collections.Generic;

namespace App.Core.Domain
{
    public abstract class Entity<TId> : IEntity<TId> where TId: IComparable, IEquatable<TId>
    {
        protected Entity() { }
        public abstract TId Id { get; protected set; }

        public override bool Equals(object obj)
        {
            return obj is Entity<TId> entity &&
                   GetType() == entity.GetType() &&
                   EqualityComparer<TId>.Default.Equals(Id, entity.Id);
        }

        public override int GetHashCode() 
            => HashCode.Combine(GetType(), Id);

        public static bool operator ==(Entity<TId> entity1, Entity<TId> entity2) 
            => EqualityComparer<Entity<TId>>.Default.Equals(entity1, entity2);

        public static bool operator !=(Entity<TId> entity1, Entity<TId> entity2) 
            => !(entity1 == entity2);
    }
}