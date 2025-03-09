using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using MohawkGame2D;

namespace haedrich_owen_a3_game
{
    class Creature
    {
        public Vector2 position; //position
        public string viewedTexturePath = "./unknown.png"; // relative path of the texture 
        public string shadowTexturePath = "./unknown.png"; // relative path of the texture 


        public Creature aMon = new Creature()
        {
            position = new Vector2(),
            viewedTexturePath = "./aMon.png",
            shadowTexturePath = "./aMonShadow.png"
        };

        public Creature bMon = new Creature()
        {

            position = new Vector2(),
            viewedTexturePath = "./bMon.png",
            shadowTexturePath = "./bMonShadow.png"
        };

        public Creature cMon = new Creature()
        {
            position = new Vector2(),
            viewedTexturePath = "./cMon.png",
            shadowTexturePath = "./cMonShadow.png"
        };
    }
}
