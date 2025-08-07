using System;
using System.Threading.Tasks;
using SnapLink_Repository.Entity;

namespace SnapLink_Repository.Repository
{
    public interface IUnitOfWork
    {
        // Repository properties for all entities
        IGenericRepository<Administrator> AdministratorRepository { get; }
        IGenericRepository<Advertisement> AdvertisementRepository { get; }
        IGenericRepository<Booking> BookingRepository { get; }
        IGenericRepository<Complaint> ComplaintRepository { get; }
        IGenericRepository<Location> LocationRepository { get; }
        IGenericRepository<LocationOwner> LocationOwnerRepository { get; }
        IGenericRepository<Messagess> MessagessRepository { get; }
        IGenericRepository<Moderator> ModeratorRepository { get; }
        IGenericRepository<Notification> NotificationRepository { get; }
        IGenericRepository<Payment> PaymentRepository { get; }
        IGenericRepository<Photographer> PhotographerRepository { get; }
        IGenericRepository<PhotographerStyle> PhotographerStyleRepository { get; }
        IGenericRepository<Wallet> WalletRepository { get; }
        IGenericRepository<PremiumPackage> PremiumPackageRepository { get; }
        IGenericRepository<PremiumSubscription> PremiumSubscriptionRepository { get; }
        IGenericRepository<Review> ReviewRepository { get; }
        IGenericRepository<Role> RoleRepository { get; }
        IGenericRepository<Style> StyleRepository { get; }
        IGenericRepository<Transaction> TransactionRepository { get; }
        IGenericRepository<User> UserRepository { get; }
        IGenericRepository<UserRole> UserRoleRepository { get; }
        IGenericRepository<UserStyle> UserStyleRepository { get; }
        IGenericRepository<WithdrawalRequest> WithdrawalRequestRepository { get; }
        IGenericRepository<Image> ImageRepository { get; }
        IGenericRepository<Availability> AvailabilityRepository { get; }

        // Transaction methods
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
