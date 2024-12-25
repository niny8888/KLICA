using System;
using System.Collections.Generic;
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
}
