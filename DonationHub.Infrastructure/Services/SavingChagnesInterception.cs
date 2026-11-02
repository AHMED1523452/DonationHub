using DonationHub.Application.Interfaces;
using DonationHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace DonationHub.Infrastructure.Services
{
    public sealed class SavingChagnesInterception : SaveChangesInterceptor
    {
        private readonly ICurrentUserService _currentUser;
        private readonly TimeProvider _timeProvider;

        public SavingChagnesInterception(ICurrentUserService currentUser,
                                         TimeProvider timeProvider)
        {
            _currentUser = currentUser;
            _timeProvider = timeProvider;
        }

        public override InterceptionResult<int> SavingChanges(
            DbContextEventData eventData,
            InterceptionResult<int> result)
        {
            Apply(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            Apply(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private void Apply(DbContext? context)
        {
            if (context is null)
                return;

            var now = _timeProvider.GetUtcNow().UtcDateTime;
            var userId = _currentUser.GetCurrentUserService();
            var ip = _currentUser.UserIP() ?? string.Empty;

            //. will be used to intercept the SaveChanges event and apply soft delete logic for entities that implement ISoftDeletable
            foreach (var entry in context.ChangeTracker.Entries<ISoftDeletable>())
            {
                if (entry.State is not EntityState.Deleted)
                    continue;

                //. make it like that this entity is not deleted from the database but just marked as deleted (Updated)
                entry.State = EntityState.Modified;
                // call the Delete method to set IsDeleted and DeletedAt properties
                entry.Entity.Delete();

                entry.Property(nameof(ISoftDeletable.IsDeleted)).IsModified = true;
                entry.Property(nameof(ISoftDeletable.DeletedAt)).IsModified = true;
            }

            //. bring the entries that are being added or modified and (The properties of the base entity) set the CreatedAt,
            //              UpdatedAt, CreatedByUserId, and CreatedByIP properties accordingly
            foreach (var entry in context.ChangeTracker.Entries<BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = now;
                        entry.Entity.UpdatedAt = now;

                        if (entry.Entity.CreatedByUserId == Guid.Empty
                            || entry.Entity.CreatedByUserId == default(Guid))
                        {
                            entry.Entity.CreatedByUserId = userId;
                        }

                        if (string.IsNullOrWhiteSpace(entry.Entity.CreatedByIP))
                        {
                            entry.Entity.CreatedByIP = ip;
                        }

                        break;

                    case EntityState.Modified:
                        entry.Entity.UpdatedAt = now;

                        entry.Property(entity => entity.CreatedAt).IsModified = false;
                        entry.Property(entity => entity.CreatedByUserId).IsModified = false;
                        entry.Property(entity => entity.CreatedByIP).IsModified = false;
                        break;
                }
            }
        }
    }
}
