
using System;
using System.Collections.Generic;



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
            public int Index { get; set; }//the turn of player
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
            public Betstatus(int[] BetsAmount = null)
            {
               if(BetsAmount != null)
                    BetplaceCount = BetsAmount;
               CreatePlaces();
            }

            private int[] BetplaceCount = { 0, 0, 0, 1, 1, 3, 7, 12, 15, 17, 19, 20 };
            public BetPlacement[] place { get; set; } = null;

            public int PlayerBetCount = 0;//The number of people who bet

            public List<BetPlacement> highestBets = new List<BetPlacement>();

            public void CreatePlaces()
            {
                place = new BetPlacement[BetplaceCount.Length];

                for (int i = 0; i < BetplaceCount.Length; i++)
                {
                    place[i] = new BetPlacement();
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
                            highestBets.Add(place[i]);
                            PlayerBetCount += 1;
                            
                            return 1;
                           
                        }
                    }
                }
                return -1;

            }
            public void Sort()
            {
                highestBets.Sort((a,b) => a.BetAmount.CompareTo(b.BetAmount));
                highestBets.Reverse();
            }

            public void ResetBetting()
            {
                CreatePlaces();
                highestBets.Clear();
                PlayerBetCount = 0;
            }
        }

        public class Tile
        {
            public TileColor color { get; set; }
            public int AssignedTo { get; set; } = 0;
        }

        public enum TileColor
        {
            White, Red
        }

        public enum LocationColor
        {
            Red,Blue,Green,Yellow,Black,White,Pink,Brown
        }


        public class BetPlacement
        {
            public int BetAmount { get; set; } = 0;
            public Player AssignedTo { get; set; } = null;
        }

        public class Table
        {

            public System.Collections.Generic.List<Tile> Tile { get; set; } = new List<Tile>();

        }

        public class freezer
        {
            public float AllFreezedToken;
        }

        public class TokenManager
        {
            public TokenManager(Eng.Confing confing)
            {
                Tokens = new int[confing.PlayerCount];
                Player = new Player[confing.PlayerCount];

                for (int i = 0; i < confing.PlayerCount; i++)
                {
                    Player[i] = confing.Players[i];
                    Tokens[i] = confing.Players[i].Token;
                }
            }
            

            public int[] Tokens;
            public Player[] Player;


            public void AddToken(int Amount,Player player)
            {
                int index = 0;
                if (Extensions.FindIndex<Player>(Player, i => i.Index == player.Index, ref index))
                {
                    Tokens[index] += Amount;
                    Console.WriteLine(Tokens[index]);
                }
                else
                    Console.WriteLine("Problem in chanching token");
            }
            public void ReduceToken(int Amount, Player player)
            {
                int index = 0;
                if (Extensions.FindIndex<Player>(Player, i => i.Index == player.Index, ref index))
                    Tokens[index] -= Amount;
                else
                    Console.WriteLine("Problem in chanching token");
            }
        }

        public static class Extensions 
        {
            public static bool FindIndex<T>(T[] arr, Predicate<T> predicate , ref int index)
            {
                for (int i = 0; i < arr.Length; i++)
                {
                    if (predicate.Invoke(arr[i]))
                    {
                        index = i;
                        return true;
                    }
                }
                return false;
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