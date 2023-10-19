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

        public void PostUser(User user)
        {
            _context.User.Add(user);
            _context.SaveChanges();
        }

        public void PutUser(User user)
        {
            _context.User.Update(user);
            _context.SaveChanges();
        }

        public void DeleteUser(User user)
        {
            _context.User.Remove(user);
            _context.SaveChanges();
        }

        public User GetUserById(Guid id)
        {
            return _context.User.FirstOrDefault(t => t.Id == id);
        }

        //public IEnumerable<User> GetUsersByPage(int page, int pageSize)
        //{
        //    List<User> users = _context.User
        //        .OrderBy(o => o.Name)
        //        .Skip((page - 1) * pageSize)
        //        .Take(pageSize)
        //        .ToList();

        //    return users;
        //}

        public User Login(string email, string password)
        {
            return _context.User.FirstOrDefault(t => t.Email == email && t.Password == password);
        }
    }
}
