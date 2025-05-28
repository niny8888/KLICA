using System.Collections.Generic;
using Microsoft.Xna.Framework;

public class GameData
{
    public int Score { get; set; }
    public int PlayerHealth { get; set; }
    public Vector2 PlayerPosition { get; set; }
    public List<Vector2> FoodPositions { get; set; } = new();
    public List<Vector2> EnemyPositions { get; set; } = new();
    public List<int> EnemyHealths { get; set; } = new();
    public List<EvolutionTrait> Traits { get; set; } = new();

}

