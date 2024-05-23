namespace bitwise.Helpers
{
    public class BitwiseEncoding
    {
        [Flags]
        public enum Genres
        {
            None = 0,
            Rock = 1,
            Pop = 2,
            Jazz = 4,
            Classical = 8,
            HipHop = 16,
            Country = 32,
            Electronic = 64,
            Metal = 128
        }

        public static Genres EncodeGenres(params Genres[] genres)
        {
            Genres encoded = Genres.None;

            foreach (var genre in genres)
            {
                encoded |= genre;
            }

            return encoded;
        }

        public static bool HasGenre(Genres encodedGenres, Genres genre)
        {
            return (encodedGenres & genre) == genre;
        }
    }
}
