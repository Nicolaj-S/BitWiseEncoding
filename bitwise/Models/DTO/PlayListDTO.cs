using bitwise.Helpers;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace bitwise.Models.DTO
{
    public class PlayListDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public BitwiseEncoding.Genres EncodedGenres { get; set; }
        public List<int> Songs { get; set; }

        public static Expression<Func<Playlist, PlayListDTO>> PlayListDetails => Playlist => new()
        {
            Id = Playlist.Id,
            Name = Playlist.Name,
            EncodedGenres = Playlist.EncodedGenres,
            Songs = Playlist.Songs.Select(x => x.Id).ToList(),
        };
    }

}
