using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace LasersNMirrors.Core
{
    public static class GridFactory
    {
        public struct LevelData
        {
            public List<UInt32> top;
            public List<UInt32> bot;
            public List<UInt32> lef;
            public List<UInt32> rig;

            public LevelData()
            {
                top = new List<UInt32>();
                bot = new List<UInt32>();
                lef = new List<UInt32>();
                rig = new List<UInt32>();
            }
        }

        public static Grid? Create(string path) 
        {
            string jsonString = File.ReadAllText(path);
            JsonNode node = JsonNode.Parse(jsonString)!;

            LevelData data = new();
            var top = node[0]!.AsArray();
            var bot = node[1]!.AsArray();
            var lef = node[2]!.AsArray();
            var rig = node[3]!.AsArray();

            for (int i = 0; i < top.Count; i++)
            {
                data.top.Add((UInt32)top[i]!.AsValue());
            }
            for (int i = 0; i < bot.Count; i++)
            {
                data.bot.Add((UInt32)bot[i]!.AsValue());
            }
            for (int i = 0; i < lef.Count; i++)
            {
                data.lef.Add((UInt32)lef[i]!.AsValue());
            }
            for (int i = 0; i < rig.Count; i++)
            {
                data.rig.Add((UInt32)rig[i]!.AsValue());
            }

            if (data.top.Count != data.bot.Count) return null;
            if (data.lef.Count != data.rig.Count) return null;

            UInt32 width = (UInt32)data.top.Count;
            UInt32 height = (UInt32)data.lef.Count;

            return new Grid(width, height, data.top, data.bot, data.lef, data.rig);
        }

        public static Grid? CreateRandom()
        {
            var rand = new Random();
            var width = (uint)rand.Next(4, 8);
            var height = (uint)rand.Next(4, 8);

            Grid grid = new Grid((width), height);

            for(uint y = 0; y < height; y++)
            {
                for(uint x = 0; x < width; x++)
                {
                    var cell = grid.GetCell(x, y);
                    var mirror_type = rand.Next(0, 4);
                    if(mirror_type < 2)
                    {
                        grid.AddMirror(x, y, (MirrorType)mirror_type);
                    }
                }
            }

            grid.SolveAndValidate();


            foreach(var number in grid.top_numbers)
            {
                var r = rand.Next(0, 2);
                if(r == 0)
                {
                    number.Constant = true;
                }
            }

            foreach (var number in grid.bottom_numbers)
            {
                var r = rand.Next(0, 2);
                if (r == 0)
                {
                    number.Constant = true;
                }
            }

            foreach (var number in grid.left_numbers)
            {
                var r = rand.Next(0, 2);
                if (r == 0)
                {
                    number.Constant = true;
                }
            }

            foreach (var number in grid.right_numbers)
            {
                var r = rand.Next(0, 2);
                if (r == 0)
                {
                    number.Constant = true;
                }
            }

            grid.ClearMirrors();
            grid.ClearLasers();
            
            return grid;
        }
    }
}
