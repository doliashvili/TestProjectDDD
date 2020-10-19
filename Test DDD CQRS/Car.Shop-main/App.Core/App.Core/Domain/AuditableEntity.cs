using System;

namespace App.Core.Domain
{
    /// <summary>
    /// Useful for auditable database, when application not uses Event-sourcing pattern
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    public abstract class AuditableEntity<TId> : Entity<TId> 
        where TId: IComparable, IEquatable<TId>
    {
        /// <summary>
        /// Creator userId
        /// </summary>
        public string CreatedBy { get; private set; }
        /// <summary>
        /// when entity created (Utc)
        /// </summary>
        public DateTime CreatedAt { get; private set; }
        /// <summary>
        /// Modifier userId
        /// </summary>
        public string LastModifiedBy {  get; private set; }
        /// <summary>
        /// when entity modified (Utc)
        /// </summary>
        public DateTime? LastModifiedAt { get; private set; }


        public void SetCreationInfo(string createdBy, DateTime createdAt)
        {
            CreatedBy = createdBy;
            CreatedAt = createdAt;
        }
        
        public void UpdateChangeInfo(string lastModifiedBy, DateTime lastModifiedAt)
        {
            LastModifiedBy = lastModifiedBy;
            LastModifiedAt = lastModifiedAt;
        }
    }
}