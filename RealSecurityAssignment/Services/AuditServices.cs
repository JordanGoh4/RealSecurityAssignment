using RealSecurityAssignment.Model;
namespace RealSecurityAssignment.Services
{
    public class AuditServices
    {
        private readonly AuthDbContext _context;
        public AuditServices(AuthDbContext context)
        {
            _context = context;
        }
        public void Add(Audit audit)
        {
            _context.Audit.Add(audit);
            _context.SaveChanges();
        }
    }
}
