using bitwise.Helpers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace bitwise.Models
{
    public class Playlist
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public BitwiseEncoding.Genres EncodedGenres { get; set; }
        [JsonIgnore]
        public List<Song> Songs { get; set; }
    }
}
