using System.Threading.Tasks;
using API.Interfaces;
using AutoMapper;

namespace API.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DContext _context;
        private readonly IMapper _mapper;
        public UnitOfWork(DContext context, IMapper mapper)
        {
            this._mapper = mapper;
            this._context = context;

        }
        public IUserRepository UserRepository => new UserRepository(_context,_mapper);

        public IMessageRepository MessageRepository => new MessageRepository(_context,_mapper);

        public ILikesRepository LikesRepository => new LikesRepository(_context,_mapper);

        public async Task<bool> Complete()
        {
            return await _context.SaveChangesAsync()>0;
        }

        public bool HasChanges()
        {
            return _context.ChangeTracker.HasChanges();
        }
    }
}