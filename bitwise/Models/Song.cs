using bitwise.Helpers;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace bitwise.Models
{
    public class Song
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public BitwiseEncoding.Genres EncodedGenre { get; set; }
        [JsonIgnore]
        public List<Playlist> Playlists { get; set; }
    }
}
