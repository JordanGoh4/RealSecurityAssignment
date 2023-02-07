using RealSecurityAssignment.Model;

namespace RealSecurityAssignment.Services
{
    public class UserServices
    {
        private readonly AuthDbContext _context;
        public UserServices(AuthDbContext context)
        {
            _context = context;
        }
        public void Add(Class user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }
        //public Class GetUser(string id)
        //{
        //    _context.Users.
        //}
        public void Update(Class user)
        {
            _context.Users.Update(user);
            _context.SaveChanges();
        }
    }
}
