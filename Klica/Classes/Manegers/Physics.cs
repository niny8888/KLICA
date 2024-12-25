using Microsoft.Xna.Framework;

public class Physics
{
    public Vector2 _positon;
    public Vector2 _velocity = Vector2.Zero;
    public Vector2 _acceleration = Vector2.Zero;
    public float _friction = 1.001f;

    private float _max_velocity = 100;

    // Constructor with initial position
    public Physics(Vector2 initialPosition)
    {
        _positon = initialPosition;
    }

    public void Update(Vector2 _got_acc)
    {
        _acceleration = _got_acc;
        _velocity /= _friction * 200f;
        if (_velocity.Length() > _max_velocity)
        {
            _velocity *= _max_velocity / _velocity.Length();
        }

        _velocity += _acceleration;
        _positon += _velocity;
    }

    public Vector2 GetPosition()
    {
        return _positon;
    }
}
