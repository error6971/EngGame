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
            public int Name { get; set; }
            public int Token { get; set; } = 0;
            public int Location { get; set; }
            public LocationColor LocationColor { get; set; }
            public Cart[] Carts { get; set; }
        }

        public class Cart
        {
            public CartColor color { get; set; }
        }

        public enum CartColor
        {
            White, Red
        }

        public enum LocationColor
        {
            Red,Blue,Green,Yellow,Black,White,Pink,Brown
        }

    }

}