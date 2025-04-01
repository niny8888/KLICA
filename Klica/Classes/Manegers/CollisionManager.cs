using System;
using System.Collections.Generic;
using Klica.Classes.Objects_sprites;
using Klica.Classes.Organizmi;
using Microsoft.Xna.Framework;

public class CollisionManager
{
    public List<Tuple<Collider, Action<Collider>>> Colliders { get; } = new();

    public void AddCollider(Collider collider, Action<Collider> onCollision)
    {
        Colliders.Add(new Tuple<Collider, Action<Collider>>(collider, onCollision));
    }

    public void Update()
    {
        for (int i = 0; i < Colliders.Count; i++)
        {
            var colliderA = Colliders[i];

            for (int j = i + 1; j < Colliders.Count; j++)
            {
                var colliderB = Colliders[j];

                if (colliderA.Item1.Intersects(colliderB.Item1))
                {
                    colliderA.Item2(colliderB.Item1);
                    colliderB.Item2(colliderA.Item1);
                }
            }
        }
    }

    public void HandleBaseBaseCollision(Player _player, Enemy _enemy)
    {
        Vector2 direction = Vector2.Normalize(_player._position - _enemy._position);
                float bounceStrength = 500f;

                _player.ApplyBounce(direction, bounceStrength);
                _enemy.ApplyBounce(-direction, bounceStrength);
    }

    public void HandlePlayerMouthWithEnemyBase(Player _player, Enemy _enemy){
         Vector2 direction = Vector2.Normalize(_player.GetMouthCollider().Position - _enemy._position);
                float bounceStrength = 500f;

                _player.ApplyBounce(direction, bounceStrength);
                _enemy.ApplyBounce(-direction, bounceStrength);
                _enemy.TakeDamage(10);
    }

    public void HandleEnemyMouthWithPlayerBase(Player _player, Enemy _enemy){
        Vector2 direction = Vector2.Normalize(_enemy.GetMouthCollider().Position - _player._position);
                float bounceStrength = 500f;

                _player.ApplyBounce(-direction, bounceStrength);
                _enemy.ApplyBounce(direction, bounceStrength);
                _player.TakeDamage(10);
    }
    
}
