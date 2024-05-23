using bitwise.DBContext;
using bitwise.Models;
using bitwise.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace bitwise.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SongController : ControllerBase
    {
        private readonly DataBaseContext _Context;

        public SongController(DataBaseContext context)
        {
            _Context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SongDTO>>> GetSongs()
        {
            return await _Context.Songs.Include(p => p.Playlists).Select(p => new SongDTO
            {
                Id = p.Id,
                Title = p.Title,
                Artist = p.Artist,
                EncodedGenre = p.EncodedGenre,
                Playlists = p.Playlists.Select(x => x.Id).ToList(),
            }).ToListAsync();
        }

        [HttpGet("{Id}")]
        public async Task<ActionResult<SongDTO>> GetASong(int Id)
        {
            var song = await _Context.Songs.Include(s => s.Playlists).Select(s => new SongDTO()
            {
                Id = s.Id,
                Title = s.Title,
                Artist = s.Artist,
                EncodedGenre = s.EncodedGenre,
                Playlists = s.Playlists.Select(p => p.Id).ToList(),
            }).FirstOrDefaultAsync(s => s.Id == Id);

            if (song == null)
            {
                return NotFound();
            }
            return song;
        }

        [HttpPost]
        public async Task<ActionResult<Song>> PostSong(SongDTO songDto)
        {
            var playlists = await _Context.Playlists.Where(p => songDto.Playlists.Contains(p.Id)).ToListAsync();
            if (playlists.Count != songDto.Playlists.Count)
            {
                return BadRequest("One or more Playlist IDs are invalid.");
            }

            var song = new Song
            {
                Title = songDto.Title,
                Artist = songDto.Artist,
                EncodedGenre = songDto.EncodedGenre,
                Playlists = playlists
            };

            _Context.Songs.Add(song);
            await _Context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetASong), new { id = song.Id }, song);
        }

        [HttpPut("{Id}")]
        public async Task<ActionResult<SongDTO>> UpdateSong(int Id, SongDTO songDTO)
        {
            var song = await _Context.Songs.Include(s => s.Playlists).FirstOrDefaultAsync(s => s.Id == Id);
            if (song == null)
            {
                return NotFound();
            }

            var existingPlaylists = new List<Playlist>();
            foreach (var PlayListId in songDTO.Playlists)
            {
                var existingPlayList = await _Context.Playlists.FindAsync(PlayListId);
                if (existingPlayList != null)
                {
                    existingPlaylists.Add(existingPlayList);
                }
                else
                {
                    return BadRequest($"Play list with ID {PlayListId} does not exist.");
                }
            }

            song.Title = songDTO.Title;
            song.Artist = songDTO.Artist;
            song.EncodedGenre = songDTO.EncodedGenre;
            song.Playlists = existingPlaylists;

            _Context.Entry(song).State = EntityState.Modified;
            try
            {
                await _Context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SongExists(Id))
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

        [HttpDelete]
        public async Task<IActionResult> DeleteSong(int id)
        {
            var song = await _Context.Songs.FindAsync(id);

            if (song == null)
            {
                return NotFound();
            }

            _Context.Songs.Remove(song);
            await _Context.SaveChangesAsync();

            return NoContent();
        }

        private bool SongExists(int id)
        {
            return _Context.Songs.Any(e => e.Id == id);
        }
    }
}
