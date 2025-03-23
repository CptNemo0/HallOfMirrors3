using System.Numerics;

namespace LasersNMirrors.Core
{
    public static class DirectionsExtensions
    {
        public static readonly Dictionary<Directions, Vector2> ToVector =
            new Dictionary<Directions, Vector2>
            {
                { Core.Directions.UP,    new Vector2( 0.0f, -1.0f)},
                { Core.Directions.DOWN,  new Vector2( 0.0f,  1.0f)},
                { Core.Directions.LEFT,  new Vector2(-1.0f,  0.0f)},
                { Core.Directions.RIGHT, new Vector2( 1.0f,  0.0f)}
            };

        public static readonly Dictionary<Vector2, Directions> ToDirection =
            new Dictionary<Vector2, Directions>
            {
                { new Vector2( 0.0f, -1.0f), Core.Directions.UP},
                { new Vector2( 0.0f,  1.0f), Core.Directions.DOWN},
                { new Vector2(-1.0f,  0.0f), Core.Directions.LEFT},
                { new Vector2( 1.0f,  0.0f), Core.Directions.RIGHT}
            };

        public static readonly List<Vector2> Directions = new List<Vector2>
            {
                new Vector2( 0.0f, 1.0f),
                new Vector2( 1.0f, 0.0f),
                new Vector2( -1.0f, 0.0f),
                new Vector2( 0.0f, -1.0f),
            };
    }
}
