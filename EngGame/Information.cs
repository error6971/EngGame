using System;


namespace EngGame
{
    namespace Information{
        /// <summary>
        /// the Definition of objects 
        /// </summary>
        
        public class Player
        {

            public int ID { get; set; }
            public string Name { get; set; }
            public int Token { get; set; } = 0;
            public int Location { get; set; }
            public LocationColor LocationColor { get; set; }
            public Tile[] Tiles { get; set; }

            public bool IsTurn { get; set; }
            public Player PlayerSetup(int Id ,string name,int token = 100)
            {
                ID = Id;
                Name = name;
                Token = token;
                return this;
            }
        }

        public class betstatus
        {
            public betPlacement[] place { get; set; }


        }

        public class Tile
        {
            public TileColor color { get; set; }
        }

        public enum TileColor
        {
            White, Red
        }

        public enum LocationColor
        {
            Red,Blue,Green,Yellow,Black,White,Pink,Brown
        }


        public class betPlacement
        {
            public int BetAmount { get; set; }
        }
    }

}