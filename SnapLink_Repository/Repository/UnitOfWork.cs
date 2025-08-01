using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using SnapLink_Repository.DBContext;
using SnapLink_Repository.Entity;

namespace SnapLink_Repository.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SnaplinkDbContext _context;
        private IDbContextTransaction _transaction;

        // Repository instances
        private IGenericRepository<Administrator> _administratorRepository;
        private IGenericRepository<Advertisement> _advertisementRepository;
        private IGenericRepository<Booking> _bookingRepository;
        private IGenericRepository<Complaint> _complaintRepository;
        private IGenericRepository<Location> _locationRepository;
        private IGenericRepository<LocationOwner> _locationOwnerRepository;
        private IGenericRepository<Messagess> _messagessRepository;
        private IGenericRepository<Moderator> _moderatorRepository;
        private IGenericRepository<Notification> _notificationRepository;
        private IGenericRepository<Payment> _paymentRepository;
        private IGenericRepository<Photographer> _photographerRepository;
        private IGenericRepository<PhotographerStyle> _photographerStyleRepository;
        private IGenericRepository<Wallet> _walletRepository;
        private IGenericRepository<PremiumPackage> _premiumPackageRepository;
        private IGenericRepository<PremiumSubscription> _premiumSubscriptionRepository;
        private IGenericRepository<Review> _reviewRepository;
        private IGenericRepository<Role> _roleRepository;
        private IGenericRepository<Style> _styleRepository;
        private IGenericRepository<Transaction> _transactionRepository;
        private IGenericRepository<User> _userRepository;
        private IGenericRepository<UserRole> _userRoleRepository;
        private IGenericRepository<UserStyle> _userStyleRepository;
        private IGenericRepository<WithdrawalRequest> _withdrawalRequestRepository;
        private IGenericRepository<PhotographerEvent> _photographerEventRepository;
        private IGenericRepository<PhotographerEventLocation> _photographerEventLocationRepository;
        private IGenericRepository<Image> _imageRepository;
        private IGenericRepository<Availability> _availabilityRepository;

        public UnitOfWork(SnaplinkDbContext context)
        {
            _context = context;
        }

        // Repository properties
        public IGenericRepository<Administrator> AdministratorRepository => 
            _administratorRepository ??= new GenericRepository<Administrator>(_context);

        public IGenericRepository<Advertisement> AdvertisementRepository => 
            _advertisementRepository ??= new GenericRepository<Advertisement>(_context);

        public IGenericRepository<Booking> BookingRepository => 
            _bookingRepository ??= new GenericRepository<Booking>(_context);

        public IGenericRepository<Complaint> ComplaintRepository => 
            _complaintRepository ??= new GenericRepository<Complaint>(_context);

        public IGenericRepository<Location> LocationRepository => 
            _locationRepository ??= new GenericRepository<Location>(_context);

        public IGenericRepository<LocationOwner> LocationOwnerRepository => 
            _locationOwnerRepository ??= new GenericRepository<LocationOwner>(_context);

        public IGenericRepository<Messagess> MessagessRepository => 
            _messagessRepository ??= new GenericRepository<Messagess>(_context);

        public IGenericRepository<Moderator> ModeratorRepository => 
            _moderatorRepository ??= new GenericRepository<Moderator>(_context);

        public IGenericRepository<Notification> NotificationRepository => 
            _notificationRepository ??= new GenericRepository<Notification>(_context);

        public IGenericRepository<Payment> PaymentRepository => 
            _paymentRepository ??= new GenericRepository<Payment>(_context);

        public IGenericRepository<Photographer> PhotographerRepository => 
            _photographerRepository ??= new GenericRepository<Photographer>(_context);

        public IGenericRepository<PhotographerStyle> PhotographerStyleRepository => 
            _photographerStyleRepository ??= new GenericRepository<PhotographerStyle>(_context);

        public IGenericRepository<Wallet> WalletRepository => 
            _walletRepository ??= new GenericRepository<Wallet>(_context);

        public IGenericRepository<PremiumPackage> PremiumPackageRepository => 
            _premiumPackageRepository ??= new GenericRepository<PremiumPackage>(_context);

        public IGenericRepository<PremiumSubscription> PremiumSubscriptionRepository => 
            _premiumSubscriptionRepository ??= new GenericRepository<PremiumSubscription>(_context);

        public IGenericRepository<Review> ReviewRepository => 
            _reviewRepository ??= new GenericRepository<Review>(_context);

        public IGenericRepository<Role> RoleRepository => 
            _roleRepository ??= new GenericRepository<Role>(_context);

        public IGenericRepository<Style> StyleRepository => 
            _styleRepository ??= new GenericRepository<Style>(_context);

        public IGenericRepository<Transaction> TransactionRepository => 
            _transactionRepository ??= new GenericRepository<Transaction>(_context);

        public IGenericRepository<User> UserRepository => 
            _userRepository ??= new GenericRepository<User>(_context);

        public IGenericRepository<UserRole> UserRoleRepository => 
            _userRoleRepository ??= new GenericRepository<UserRole>(_context);

        public IGenericRepository<UserStyle> UserStyleRepository => 
            _userStyleRepository ??= new GenericRepository<UserStyle>(_context);

        public IGenericRepository<WithdrawalRequest> WithdrawalRequestRepository => 
            _withdrawalRequestRepository ??= new GenericRepository<WithdrawalRequest>(_context);

        public IGenericRepository<PhotographerEvent> PhotographerEventRepository => 
            _photographerEventRepository ??= new GenericRepository<PhotographerEvent>(_context);

        public IGenericRepository<PhotographerEventLocation> PhotographerEventLocationRepository => 
            _photographerEventLocationRepository ??= new GenericRepository<PhotographerEventLocation>(_context);

        public IGenericRepository<Image> ImageRepository =>
            _imageRepository ??= new GenericRepository<Image>(_context);

        public IGenericRepository<Availability> AvailabilityRepository =>
            _availabilityRepository ??= new GenericRepository<Availability>(_context);

        // Transaction methods
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await _transaction?.CommitAsync();
            }
            catch
            {
                await _transaction?.RollbackAsync();
                throw;
            }
            finally
            {
                _transaction?.Dispose();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            try
            {
                await _transaction?.RollbackAsync();
            }
            finally
            {
                _transaction?.Dispose();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context?.Dispose();
        }
    }
}
