using EngGame;
using System;


namespace EngGame
{
    namespace Information {
        /// <summary>
        /// the Definition of objects 
        /// </summary>

        public class Player
        {

            public int ID { get; set; }
            public string Name { get; set; }
            public int Token { get; set; } = 0;
            public int Location { get; set; }//the turn of player
            public LocationColor LocationColor { get; set; }

            public TilePack TilePack { get; set; }

            public bool IsTurn { get; set; }
            public Player PlayerSetup(int Id, string name, int token = 100)
            {
                ID = Id;
                Name = name;
                Token = token;
                return this;
            }
        }
        public class TilePack
        {

            public Tile[] Tiles { get; set; }
        }
        public class  Betstatus 
        {
            public Betstatus()
            {
                CreatePlaces();
            }

            private int[] BetplaceCount = {0,0,0,1,1,3,7,12,15,17,19,20};
            public betPlacement[] place { get; set; } = null;

            public int PlayerBetCount = 0;//The number of people who bet

            public void CreatePlaces()
            {
                place = new betPlacement[BetplaceCount.Length];

                for (int i = 0; i < BetplaceCount.Length; i++)
                {
                    place[i] = new betPlacement();
                    place[i].BetAmount = BetplaceCount[i];
                }
            }
            public int SetBet(int amount, Player player)
            {
                if (place == null)
                    CreatePlaces();


                for (int i = 0; i < BetplaceCount.Length; i++)
                {
                    if (place[i].BetAmount == amount)
                    {
                        if (place[i].AssignedTo == null)
                        {
                            place[i].AssignedTo = player;
                            PlayerBetCount += 1;
                            
                            return 1;
                           
                        }
                    }
                }
                return -1;

            }
            public void ResetBetting()
            {
                CreatePlaces();
            }
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
            public int BetAmount { get; set; } = 0;
            public Player AssignedTo { get; set; } = null;
        }

        public class Table
        {
            public Player AssignedTo { get; set; } = null;
            public System.Collections.Generic.List<Tile> Tile { get; set; }

        }
        public static class Extensions 
        {
            public static int FindIndex<T>(T[] arr, Predicate<T> predicate)
            {
                for (int i = 0; i < arr.Length; i++)
                {
                    if (predicate.Invoke(arr[i]))
                    {
                        return i;
                    }
                }
                return -1;
            }
            public static void EqualsCount<T>(T[] arr, Predicate<T> predicate , ref int count)
            {
                for (int i = 0; i < arr.Length; i++)
                {
                    if (predicate.Invoke(arr[i]))
                    {
                        count++;
                    }
                }
                
            }
            public static void RemoveAt<T>(ref T[] arr, int index)
            {
                for (int a = index; a < arr.Length - 1; a++)
                {
                    // moving elements downwards, to fill the gap at [index]
                    arr[a] = arr[a + 1];
                }
                // finally, let's decrement Array's size by one
                Array.Resize(ref arr, arr.Length - 1);
            }
        }
    }

}