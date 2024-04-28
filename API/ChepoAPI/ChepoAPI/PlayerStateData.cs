using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks.Dataflow;

namespace ChepoAPI
{
    public class PlayerStateData
    {
        [Key]
        public Guid user_uuid { get; set; }
        public bool is_in_game { get; set; }
        public string? map_name { get; set; }
        public Guid server_uuid { get; set; }
        public List<Guid>? friends { get; set; }
    }
}
