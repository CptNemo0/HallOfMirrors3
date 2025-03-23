namespace LasersNMirrors.Core
{
    using DirMTypePair = Tuple<Directions, MirrorType>;
    public static class Reflections
    {
        public static readonly Dictionary<DirMTypePair, Directions> ReflectionMap = new Dictionary<DirMTypePair, Directions>
        {
            {new DirMTypePair(Directions.UP, MirrorType.SLASH),     Directions.RIGHT},
            {new DirMTypePair(Directions.UP, MirrorType.BACKSLASH), Directions.LEFT },

            {new DirMTypePair(Directions.DOWN, MirrorType.SLASH),     Directions.LEFT },
            {new DirMTypePair(Directions.DOWN, MirrorType.BACKSLASH), Directions.RIGHT},

            {new DirMTypePair(Directions.RIGHT, MirrorType.SLASH),     Directions.UP  },
            {new DirMTypePair(Directions.RIGHT, MirrorType.BACKSLASH), Directions.DOWN},

            {new DirMTypePair(Directions.LEFT, MirrorType.SLASH),     Directions.DOWN},
            {new DirMTypePair(Directions.LEFT, MirrorType.BACKSLASH), Directions.UP  },
        };
    }
}
