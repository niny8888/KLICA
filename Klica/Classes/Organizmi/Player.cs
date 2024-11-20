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
        private Physics _phisycs;
        public Player(){
            _phisycs = new Physics();
        }

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

            ///posision je to kar mi fizka vrne da je
            _phisycs.Update(go);
            _player_base.SetPosition(_phisycs.GetPosition());


            
        }

        // public void DrawPlayer(SpriteBatch _spriteBatch){
        //     _player_base.Draw(_spriteBatch);
        // }


    }
}
