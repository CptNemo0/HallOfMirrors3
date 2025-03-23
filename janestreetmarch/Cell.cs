using System.Diagnostics;
using System.Numerics;

namespace LasersNMirrors.Core
{
    public class Cell
    {
        Mirror? mirror;
        public bool Empty => mirror == null;

        public Mirror? Mirror => mirror;

        public Cell()
        {
            mirror = null;
        }
        
        public Cell(Mirror mirror)
        {
            this.mirror = mirror;
        }

        public void SetMirror(MirrorType? mirror_type = null)
        {
            if (mirror_type == null)
            {
                mirror = null;
                return;
            }
            this.mirror = new Mirror((MirrorType)mirror_type);
        }

        public Vector2 PassThrough(Vector2 incoming_direction)
        {
            if (Empty) return incoming_direction;
            return mirror!.Reflect(incoming_direction);
        }
    }
}
