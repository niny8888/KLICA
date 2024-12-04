using System;
using System.Security.Cryptography.X509Certificates;
using Klica;
using Klica.Classes.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Klica.Classes.Objects_sprites
//namespace Klica;
{
    public class Player 
    {
        private Base _player_base = new Base(0);
        private Eyes _player_eye= new Eyes(0);
        private Physics _phisycs;
        public Player(){
            _phisycs = new Physics();
        }

        public Vector2 _position { get; internal set; }

        public void UpdatePlayer(){
            Vector2 go= Vector2.Zero; //akselerejšn


            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {   
                go.Y -= 1;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                go.X -= 1;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                go.Y += 1;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                go.X += 1;
            }
            if (go != Vector2.Zero)
            {
                go.Normalize();
            }

            ///posision je to kar mi fizka vrne da je
            _phisycs.Update(go);
            _player_base.SetPosition(_phisycs.GetPosition());
            _position=_player_base.GetPosition();
            _player_base.SetRotation((float)Math.Atan2(_phisycs._velocity.Y,_phisycs._velocity.X) +1.6f);
            _player_eye.SetPosition(_player_base._position_eyes);

            
        }

        public void DrawPlayer(SpriteBatch _spriteBatch){
             _player_base.Draw(_spriteBatch);
             _player_eye.Draw(_spriteBatch);
         }


    }
}
