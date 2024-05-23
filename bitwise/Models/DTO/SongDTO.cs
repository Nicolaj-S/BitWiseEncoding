using bitwise.Helpers;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Text.Json.Serialization;

namespace bitwise.Models.DTO
{
    public class SongDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public BitwiseEncoding.Genres EncodedGenre { get; set; }
        public List<int> Playlists { get; set; }

        public static Expression<Func<Song, SongDTO>> SongDetails => song => new()
        {
            Id = song.Id,
            Title = song.Title,
            Artist = song.Artist,
            EncodedGenre = song.EncodedGenre,
            Playlists = song.Playlists.Select(x => x.Id).ToList(),
        };
    }
    

}
