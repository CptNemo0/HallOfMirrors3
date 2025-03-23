using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
