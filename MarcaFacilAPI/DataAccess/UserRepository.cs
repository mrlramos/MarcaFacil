using MarcaFacilAPI.DataAccess.Context;
using MarcaFacilAPI.Models;

namespace MarcaFacilAPI.DataAccess
{
    public class UserRepository
    {
        private readonly PostgreSqlContext _context;

        public UserRepository(PostgreSqlContext context)
        {
            _context = context;
        }

        public List<User> GetUsers()
        {
            return _context.User.ToList();
        }

        public User GetUserById(Guid id)
        {
            return _context.User.FirstOrDefault(t => t.Id == id);
        }

        public IEnumerable<User> GetUsersByPage(int page, int pageSize)
        {
            List<User> users = _context.User
                .OrderBy(o => o.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return users;
        }

        public void PostUser(User user)
        {
            _context.User.Add(user);
            _context.SaveChanges();
        }

        public void DeleteUser(Guid id)
        {
            var entity = _context.User.FirstOrDefault(t => t.Id == id);
            _context.User.Remove(entity);
            _context.SaveChanges();
        }

        public User GetUserByEmail(string email)
        {
            return _context.User.Where(x => x.Email == email).FirstOrDefault();
        }
    }
}
