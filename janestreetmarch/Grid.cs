using System.Numerics;

namespace LasersNMirrors.Core
{
    public class Grid
    {
        private Cell[,] cells;
        private readonly UInt32 width;
        private readonly UInt32 height;

        public List<ClueNumber> top_numbers;
        public List<ClueNumber> bottom_numbers;
        public List<ClueNumber> left_numbers;
        public List<ClueNumber> right_numbers;

        public List<Laser> top_lasers;
        public List<Laser> bottom_lasers;
        public List<Laser> left_lasers;
        public List<Laser> right_lasers;

        Dictionary<Laser, ClueNumber> laser_clue_map = new();
        Dictionary<Vector2, Laser> pos_laser_map = new();

        public UInt32 Width => width;

        public UInt32 Height => height;

        public Cell[,] Cells
        {
            get { return cells; }
            set { cells = value; }
        }

        public Grid(UInt32 width, UInt32 height)
        {
            this.width = width;
            this.height = height;
            cells = new Cell[this.width, this.height];

            for (UInt32 x = 0; x < this.width; x++)
            {
                for (UInt32 y = 0; y < this.height; y++)
                {
                    cells[x, y] = new();
                }
            }

            top_numbers = new((int)this.width);
            bottom_numbers = new((int)this.width);
            top_lasers = new((int)this.width);
            bottom_lasers = new((int)this.width);

            for (int i = 0; i < this.width; i++)
            {
                top_numbers.Add(new());
                bottom_numbers.Add(new());

                top_lasers.Add(new Laser(new Vector2(0.0f, 1.0f), new Vector2(i, -1.0f)));
                bottom_lasers.Add(new Laser(new Vector2(0.0f, -1.0f), new Vector2(i, height)));
                pos_laser_map.Add(new Vector2(i, -1.0f), top_lasers.Last());
                pos_laser_map.Add(new Vector2(i, height), bottom_lasers.Last());

                laser_clue_map.Add(top_lasers.Last(), top_numbers.Last());
                laser_clue_map.Add(bottom_lasers.Last(), bottom_numbers.Last());
            }

            left_numbers = new((int)this.height);
            right_numbers = new((int)this.height);
            left_lasers = new((int)this.height);
            right_lasers = new((int)this.height);   
            for (int i = 0; i < this.height; i++)
            {
                left_numbers.Add(new());
                right_numbers.Add(new());
                left_lasers.Add(new Laser(new Vector2(1.0f, 0.0f), new Vector2(-1.0f, i)));
                right_lasers.Add(new Laser(new Vector2(-1.0f, 0.0f), new Vector2(width, i)));
                pos_laser_map.Add(new Vector2(-1.0f, i), left_lasers.Last());
                pos_laser_map.Add(new Vector2(width, i), right_lasers.Last());
                laser_clue_map.Add(left_lasers.Last(), left_numbers.Last());
                laser_clue_map.Add(right_lasers.Last(), right_numbers.Last());
            }
        }

        public Grid(UInt32 width, 
                    UInt32 height,
                    List<UInt32> top,
                    List<UInt32> bot,
                    List<UInt32> lef,
                    List<UInt32> rig)
        {
            this.width = width;
            this.height = height;
            cells = new Cell[this.width, this.height];

            for (UInt32 x = 0; x < this.width; x++)
            {
                for (UInt32 y = 0; y < this.height; y++)
                {
                    cells[x, y] = new();
                }
            }

            top_numbers    = new((int)this.width);
            bottom_numbers = new((int)this.width);

            top_lasers    = new((int)this.width);
            bottom_lasers = new((int)this.width);

            for (int i = 0; i < this.width; i++)
            {
                top_numbers.   Add(new(top[i]));
                bottom_numbers.Add(new(bot[i]));

                var top_laser_pos = new Vector2(i,  -1.0f);
                var bot_laser_pos = new Vector2(i, height);

                top_lasers.   Add(new Laser(new Vector2(0.0f,  1.0f), top_laser_pos));
                bottom_lasers.Add(new Laser(new Vector2(0.0f, -1.0f), bot_laser_pos));

                pos_laser_map.Add(top_laser_pos,    top_lasers.Last());
                pos_laser_map.Add(bot_laser_pos, bottom_lasers.Last());

                laser_clue_map.Add(   top_lasers.Last(),    top_numbers.Last());
                laser_clue_map.Add(bottom_lasers.Last(), bottom_numbers.Last());
            }

            left_numbers  = new((int)this.height);
            right_numbers = new((int)this.height);
            
            left_lasers   = new((int)this.height);
            right_lasers  = new((int)this.height);

            for (int i = 0; i < this.height; i++)
            {
                left_numbers. Add(new(lef[i]));
                right_numbers.Add(new(rig[i]));

                var lef_laser_pos = new Vector2(-1.0f, i);
                var rig_laser_pos = new Vector2(width, i);

                left_lasers. Add(new Laser(new Vector2(1.0f, 0.0f),  lef_laser_pos));
                right_lasers.Add(new Laser(new Vector2(-1.0f, 0.0f), rig_laser_pos));

                pos_laser_map.Add(lef_laser_pos,  left_lasers.Last());
                pos_laser_map.Add(rig_laser_pos, right_lasers.Last());

                laser_clue_map.Add( left_lasers.Last(),  left_numbers.Last());
                laser_clue_map.Add(right_lasers.Last(), right_numbers.Last());
            }
        }

        public bool IsInBounds(Vector2 pos)
        {
            return pos.X >= 0 && pos.X < Width && pos.Y >= 0 && pos.Y < Height;
        }

        public bool IsInBounds(UInt32 x, UInt32 y)
        {
            return x >= 0 && x < Width && y >= 0 && y < Height;
        }

        public Cell GetCell(Vector2 pos)
        {
            if(!IsInBounds(pos)) throw new IndexOutOfRangeException();
            return cells[(UInt32)pos.X, (UInt32)pos.Y];
        }

        public Cell GetCell(UInt32 x, UInt32 y)
        {
            if (!IsInBounds(x, y)) throw new IndexOutOfRangeException();
            return cells[(UInt32)x, (UInt32)y];
        }

        public void AddMirror(UInt32 x, UInt32 y, MirrorType type)
        {
            Vector2 pos = new Vector2(x, y);

            foreach (var dir in DirectionsExtensions.Directions) 
            {
                var new_pos = pos + dir;
                if (!IsInBounds(new_pos)) continue;
                if (!cells[(UInt32)new_pos.X, (UInt32)new_pos.Y].Empty) return;
            }

            cells[x, y].SetMirror(type);
        }

        public void ClearMirror(UInt32 x, UInt32 y)
        {
            cells[x, y].SetMirror();
        }
    
        public void CleanNumbers()
        {
            foreach (var number in left_numbers)
            {
                if (number.Constant) continue;
                number.Number = 0;
            }

            foreach (var number in right_numbers)
            {
                if (number.Constant) continue;
                number.Number = 0;
            }

            foreach (var number in bottom_numbers)
            {
                if (number.Constant) continue;
                number.Number = 0;
            }

            foreach (var number in top_numbers)
            {
                if (number.Constant) continue;
                number.Number = 0;
            }
        }

        public void CalculateLaser(Laser laser)
        {
            laser.ClearPath();
            var current = laser.StartPosition;
            var direction = laser.StartDirection;
            UInt32 current_length = 0;
            laser.Length = 1;
            laser.AddPath(laser.StartPosition);
            while (true)
            {
                current += direction;

                if(IsInBounds(current))
                {
                    var previous_direction = new Vector2(direction.X, direction.Y);
                    direction = cells[(UInt32)current.X, (UInt32)current.Y].PassThrough(direction);
                    current_length++;

                    if (previous_direction != direction)
                    {
                        laser.Length = laser.Length * current_length;
                        current_length = 0;
                    }

                    laser.AddPath(current);
                }
                else
                {
                    current_length++;
                    laser.Length = laser.Length * current_length;
                    break;
                }
            }

            var output_laser = pos_laser_map[current];
            laser.AddPath(output_laser.StartPosition);
            var starting_clue_number = laser_clue_map[laser];
            var ending_clue_number = laser_clue_map[output_laser];

            if(!starting_clue_number.Constant) starting_clue_number.Number = laser.Length;
            if(!ending_clue_number.Constant) ending_clue_number.Number = laser.Length;

            output_laser.Active = false;

            if(starting_clue_number.Number != ending_clue_number.Number)
            {
                laser.Solved = false;
                output_laser.Solved = false;
            }
            else
            {
                laser.Solved = true;
                output_laser.Solved = true;
            }
        }

        public void CalculateLasers()
        {
            foreach (var laser in left_lasers)
            {
                if (!laser.Active) continue;
                CalculateLaser(laser);
            }
            
            foreach (var laser in right_lasers)
            {
                if (!laser.Active) continue;
                CalculateLaser(laser);
            }
            
            foreach (var laser in top_lasers)
            {
                if (!laser.Active) continue;
                CalculateLaser(laser);
            }
            
            foreach (var laser in bottom_lasers)
            {
                if (!laser.Active) continue;
                CalculateLaser(laser);
            }
        }
    
        public void Solve()
        {
            foreach (var laser in left_lasers)
            {
                laser.Active = true;
            }

            foreach (var laser in right_lasers)
            {
                laser.Active = true;
            }

            foreach (var laser in top_lasers)
            {
                laser.Active = true;
            }

            foreach (var laser in bottom_lasers)
            {
                laser.Active = true;
            }
            CalculateLasers();
        }

        public bool IsValid()
        {
            foreach (var laser in left_lasers) if (!laser.Solved) return false;
            foreach (var laser in right_lasers) if (!laser.Solved) return false;
            foreach (var laser in top_lasers) if (!laser.Solved) return false;
            foreach (var laser in bottom_lasers) if (!laser.Solved) return false;

            foreach (var number in left_numbers) if (number.Number == 0) return false;
            foreach (var number in right_numbers) if (number.Number == 0) return false;
            foreach (var number in top_numbers) if (number.Number == 0) return false;
            foreach (var number in bottom_numbers) if (number.Number == 0) return false;

            return true;
        }

        public bool SolveAndValidate()
        {
            Solve();
            return IsValid();
        }
    }
}
