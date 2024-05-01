using System.ComponentModel.DataAnnotations;

namespace ChepoAPI
{
    public class UsersData
    {
        [Key]
        public Guid uuid { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string salt { get; set; }
    }
    public class UsersDataAuth
    {
        [Key]
        public string username { get; set; }
        public string password { get; set; }
    }
}
