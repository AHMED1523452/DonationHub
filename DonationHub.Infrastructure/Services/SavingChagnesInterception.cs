using DonationHub.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace DonationHub.Infrastructure.Services
{
    public class SavingChagnesInterception : SaveChangesInterceptor
    {

        //. that's will be like a security for making a hard deleting for any record in any model 
        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            if (eventData?.Context is null)
                return result;

            foreach(var item in eventData.Context.ChangeTracker.Entries())
            {
                if(item is not { State: EntityState.Deleted , Entity: ISoftDeletable entity})
                {
                    continue;
                }
                item.State = EntityState.Modified;

                entity.Delete();
            }

            return result;
        }
    }
}
