using System.ComponentModel.DataAnnotations;

namespace ChepoAPI
{
    public class SuccessData
    {
        [Key]
        public Guid uuid { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string image { get; set; }
    }
}
