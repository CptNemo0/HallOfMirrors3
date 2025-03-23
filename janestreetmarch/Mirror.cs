using System.Numerics;

namespace LasersNMirrors.Core
{
    public class Mirror
    {
        private MirrorType type;

        public MirrorType Type => type;

        public Mirror(MirrorType type)
        {
            this.type = type;
        }

        public Vector2 Reflect(Vector2 incoming)
        {
            Directions dir_incoming; 
            Directions dir_reflected;
            Vector2 reflected;

            DirectionsExtensions.ToDirection.TryGetValue(incoming,                    out dir_incoming);
            Reflections.ReflectionMap.       TryGetValue(new (dir_incoming, type),    out dir_reflected);
            DirectionsExtensions.ToVector.   TryGetValue(dir_reflected,               out reflected);

            return reflected;
        }
    }
}
