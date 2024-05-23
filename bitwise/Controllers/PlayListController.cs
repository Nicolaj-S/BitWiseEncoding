using bitwise.DBContext;
using bitwise.Helpers;
using bitwise.Models;
using bitwise.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace bitwise.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayListController : ControllerBase
    {
        private readonly DataBaseContext _context;

        public PlayListController(DataBaseContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlayListDTO>>> GetPlaylists()
        {
            return await _context.Playlists
            .Include(p => p.Songs)
            .Select(p => new PlayListDTO
            {
                Id = p.Id,
                Name = p.Name,
                EncodedGenres = p.EncodedGenres,
                Songs = p.Songs.Select(s => s.Id).ToList()
            })
            .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PlayListDTO>> GetPlaylist(int id)
        {
            var playlist = await _context.Playlists.Include(p => p.Songs).Select(p => new PlayListDTO()
            {
                Id = p.Id,
                Name = p.Name,
                EncodedGenres = p.EncodedGenres,
                Songs = p.Songs.Select(s => s.Id).ToList(),
            }).FirstOrDefaultAsync(p => p.Id == id);

            if (playlist == null)
            {
                return NotFound();
            }

            return playlist;
        }

        [HttpPost]
        public async Task<ActionResult<PlayListDTO>> PostPlaylist(PlayListDTO playlistDto)
        {
            var existingSongs = new List<Song>();
            foreach (var songId in playlistDto.Songs)
            {
                var existingSong = await _context.Songs.FindAsync(songId);
                if (existingSong != null)
                {
                    existingSongs.Add(existingSong);
                }
                else
                {
                    return BadRequest($"Song with ID {songId} does not exist.");
                }
            }

            var playlist = new Playlist
            {
                Name = playlistDto.Name,
                Songs = existingSongs,
                EncodedGenres = EncodePlaylistGenres(existingSongs)
            };

            _context.Playlists.Add(playlist);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPlaylist), new { id = playlist.Id }, playlist);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutPlaylist(int id, PlayListDTO playlistDto)
        {
            var playlist = await _context.Playlists.Include(p => p.Songs).FirstOrDefaultAsync(p => p.Id == id);
            if (playlist == null)
            {
                return NotFound();
            }

            var existingSongs = new List<Song>();
            foreach (var songId in playlistDto.Songs)
            {
                var existingSong = await _context.Songs.FindAsync(songId);
                if (existingSong != null)
                {
                    existingSongs.Add(existingSong);
                }
                else
                {
                    return BadRequest($"Song with ID {songId} does not exist.");
                }
            }

            playlist.Name = playlistDto.Name;
            playlist.Songs = existingSongs;
            playlist.EncodedGenres = EncodePlaylistGenres(existingSongs);

            _context.Entry(playlist).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlaylistExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlaylist(int id)
        {
            var playlist = await _context.Playlists.FindAsync(id);

            if (playlist == null)
            {
                return NotFound();
            }

            _context.Playlists.Remove(playlist);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PlaylistExists(int id)
        {
            return _context.Playlists.Any(e => e.Id == id);
        }

        private BitwiseEncoding.Genres EncodePlaylistGenres(List<Song> songs)
        {
            var genreFlags = songs.Select(song => song.EncodedGenre).Aggregate((current, next) => current | next);
            return genreFlags;
        }
    }
}
