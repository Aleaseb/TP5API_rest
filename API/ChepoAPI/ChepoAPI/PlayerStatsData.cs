using System.ComponentModel.DataAnnotations;

namespace ChepoAPI
{
    public class PlayerStatsData
    {
        [Key]
        public Guid user_uuid { get; set; }
        public Guid rank_uuid { get; set; }
        public int kill { get; set; }
        public int death { get; set; }
    }
}
