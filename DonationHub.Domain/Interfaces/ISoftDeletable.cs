
namespace DonationHub.Application.Interfaces
{
    public interface ISoftDeletable
    {
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        public void Delete()
        {
            IsDeleted = true;
            DeletedAt = DateTime.UtcNow;
        }

        public void UndoDelete()
        {
            IsDeleted = false;
            DeletedAt = null;
        }
    }
}
