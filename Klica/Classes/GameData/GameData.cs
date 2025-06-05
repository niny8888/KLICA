using System.Collections.Generic;
using Microsoft.Xna.Framework;

public class GameData
{
    public List<EvolutionTrait> Traits { get; set; } = new();
    
    public int LastCompletedLevel { get; set; } = 0; // Default to 0 (no level completed)
    

}

