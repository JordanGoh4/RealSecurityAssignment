namespace RealSecurityAssignment.Model
{
    public class Audit
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Action { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
