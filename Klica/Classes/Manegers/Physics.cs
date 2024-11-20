
using Microsoft.Xna.Framework;

namespace Klica.Classes.Managers{
    public class Physics{
        public Vector2 _positon= Vector2.Zero;
        public Vector2 _velocity= Vector2.Zero;
        public Vector2 _acceleration= Vector2.Zero;
        public float _friction= 1.001f;

        private static float _max_velocity = 100;

        public Physics(){
        }

        public void Update(Vector2 _got_acc){
            _acceleration=_got_acc;
            _velocity/=_friction*200f;
            if(_velocity.Length()>_max_velocity){
                _velocity*=_max_velocity/_velocity.Length();
            }

            _velocity+=_acceleration;
            _positon+=_velocity;

        }

        public Vector2 GetPosition(){
            return _positon;
        }
    }
}