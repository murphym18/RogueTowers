using System;
using System.Linq;
using Helpers;

public class MapBuilder
{
    private LevelBuilder[] Levels;
    private int LevelWidth, LevelHeight;

    public MapBuilder(int NumLevels, int LevelWidth, int LevelHeight)
    {
        Levels = new LevelBuilder[NumLevels];
        for (int i = 0; i < NumLevels; i++)
            Levels[i] = new LevelBuilder(LevelWidth, LevelHeight);

        this.LevelWidth = LevelWidth;
        this.LevelHeight = LevelHeight;
    }

    public bool this[int x, int y]
    {
        get
        {
            if (x < 0 || x >= LevelWidth*Levels.Length || y < 0 || y >= LevelHeight)
                return true;
            int lvl = x / LevelWidth;
            x = x % LevelWidth;
            return Levels[lvl][y, x];
        }
        set
        {
            int lvl = x / LevelWidth;
            x = x % LevelWidth;
            Levels[lvl][y, x] = value;
        }
    }
}
