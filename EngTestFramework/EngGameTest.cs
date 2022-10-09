using Microsoft.VisualStudio.TestTools.UnitTesting;
using EngGame;
using EngGame.Information;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using System;


namespace EngTestFramework
{
    [TestClass]
    public class EngGameTest
    {
        private Eng.PlayerStatus[] playerStatus;
        private Eng Game;
        private int PlayerTurn;
        private Random random => new Random(Guid.NewGuid().GetHashCode());
        public void Init()
        {
            
            Game = new Eng();
            
            Game.EventUpdateStatus = delegate() { Update(); };
            Game.EventUpdateTurn = delegate () { UpdateTurn(); };
        }
        [TestMethod]
        
        public void Turn1()
        {



            //assing

            Init();

            Eng.Confing confing = new Eng.Confing();

            confing.PlayerCount = 6;

            confing.Players = new Player[6];
            confing.Players[0] = new Player().PlayerSetup(1,"Reza",80);
            confing.Players[1] = new Player().PlayerSetup(2, "ali", 120);
            confing.Players[2] = new Player().PlayerSetup(3, "mohamad", 60);
            confing.Players[3] = new Player().PlayerSetup(4, "fateme", 95);
            confing.Players[4] = new Player().PlayerSetup(5, "aref", 95);
            confing.Players[5] = new Player().PlayerSetup(6, "gasem", 100);
            confing.PlayerStartCartCount = 5;

            //Act
            int[] bets = { 0, 0, 0, 1, 3, 7, 11, 15, 17, 20 }; 
            Game.CheckPlayers(confing.Players);
            Game.Setup(confing,bets);
            
           
            Game.Betting(3);
            Game.Betting(12);
            Game.Betting(20);
            Game.Betting(7);
            Game.Betting(0);
            Game.Betting(0);


            Game.ChoiceTile(0);
            Game.ChoiceTile(1);
            Game.ChoiceTile(2);
            Game.ChoiceTile(0);
            Game.ChoiceTile(0);
            Game.ChoiceTile(0);
            Game.ChoiceTile(0);
            Game.ChoiceTile(0);
            Game.ChoiceTile(0);
            Game.ChoiceTile(0);
            Game.StopChoicingTile();

            RaiseTakeover(2);
            RaiseTakeover(0);
            RaiseTakeover(0);
            RaiseTakeover(0);
            RaiseTakeover(0);
            RaiseTakeover(0);
            RaiseTakeover(0);

            var Tabletile = Game.ReturnInTableTiles();

            bool Continue = true;
            while (Continue) {

                int TableIndex = random.Next(0, Tabletile.Length);

                int TileIndex = random.Next(0, Tabletile[TableIndex].Tiles.Length);

                Continue = Game.TakeOverTiles(TableIndex, TileIndex);
                if(Continue)
                    Console.WriteLine("White Cart");
                else
                    Console.WriteLine("red Cart");
            }
            

            TilePack[] playerTiles = Game.ReturnTiles();
            Console.WriteLine("player " + Game._Confing.Players[Game.Status.TakeOverTilePlayerIndex].Name+" win the raise up with :" +Game.Status.HighestRaiseUp+" cart");



            for (int i = 0; i < playerTiles.Length; i++)
            {

                Console.WriteLine("result  : " + Game._Confing.Players[i].Name + " in table has : \r " 
                     
                    );

                for (int c = 0; c < Game.Status.Table[i].Tile.Count; c++)
                {
                    Console.WriteLine(Game.Status.Table[i].Tile[c].color);
                }
                    
                    
                    
                for (int y = 0; y < playerTiles[i].Tiles.Length; y++)
                {
                    Console.WriteLine("    Carts  : " + playerTiles[i].Tiles[y].color);
                    
                }
                
            }

            Game.StartNextTurn();
            //Assert
            Game.Betting(0);
            Game.Betting(1);
            Game.Betting(7);
            Game.Betting(3);
            Game.Betting(12);
            Game.Betting(1);
           
            TilePack[] tiles = Game.ReturnTiles();
            for (int i = 0; i < Game._Confing.Players.Length; i++)
            {
               

                for (int c = 0; c < random.Next(1, 3); c++)
                {
                    int a = random.Next(0, 0);
                    Game.ChoiceTile(a);
                  
                }
            }
            Game.StopChoicingTile();

           Game.RaiseTakeOverTile(2);
            RaiseTakeover(3);
            RaiseTakeover(4);
            RaiseTakeover(6);
            RaiseTakeover(8);
            RaiseTakeover(10);
            RaiseTakeover(12);
        }
 

    
        public void Update()
        {
            playerStatus = Game.playerStatus;


        }
        public void UpdateTurn()
        {
            PlayerTurn = Game.Status.PlayerTurnIndex;
        }
        public void RaiseTakeover(int count)
        {
            if (playerStatus == null)
                Update();

         

            
                Game.RaiseTakeOverTile(count);

        }

    }
}
