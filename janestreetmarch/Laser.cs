using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace LasersNMirrors.Core
{
    public class Laser
    {
        private readonly Vector2 start_position;
        private readonly Vector2 start_direction;
        
        private List<Vector2> path = new();

        private UInt32 length = 1;
        private bool active = false;
        private bool solved = false;

        public Laser(Vector2 start_dir, Vector2 position)
        {
            this.start_direction = start_dir;
            this.start_position = position;
        }

        public Vector2 StartPosition => start_position;
        public Vector2 StartDirection => start_direction;

        public UInt32 Length { get => length; set => length = value; }
        public bool Active { get => active; set => active = value; }
        public bool Solved { get => solved; set => solved = value; }

        public IReadOnlyCollection<Vector2> Path => path.AsReadOnly();

        public void AddPath(Vector2 corner) 
        {
            path.Add(corner);
        }

        public void ClearPath()
        {
            path.Clear();
        }
    }
}
