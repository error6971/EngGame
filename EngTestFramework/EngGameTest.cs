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

        public void Init()
        {
            
            Game = new Eng();
            
            Game.EventUpdateStatus = delegate() { Update(); };
            Game.EventUpdateTurn = delegate () { UpdateTurn(); };
        }
        [TestMethod]
        
        public void TestMethod1()
        {



            //assing

            Init();

            Eng.Confing confing = new Eng.Confing();

            confing.PlayerCount = 4;

            confing.Players = new EngGame.Information.Player[4];
            confing.Players[0] = new EngGame.Information.Player().PlayerSetup(1,"Reza",80);
            confing.Players[1] = new EngGame.Information.Player().PlayerSetup(2, "ali", 120);
            confing.Players[2] = new EngGame.Information.Player().PlayerSetup(3, "mohamad", 60);
            confing.Players[3] = new EngGame.Information.Player().PlayerSetup(4, "fateme", 95);
            confing.PlayerStartCartCount = 5;

            //Act

            Game.CheckPlayers(confing.Players);
            Game.Setup(confing);

            Game.StartTurn();
            Game.Betting(3);
            Game.Betting(7);
            Game.Betting(1);
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
            RaiseTakeover(4);
            RaiseTakeover(10);
            RaiseTakeover(6);
            RaiseTakeover(8);
            RaiseTakeover(9);


            TilePack[] playerTiles = Game.ReturnTiles();
            Console.WriteLine("player " + Game._Confing.Players[Game.Status.TakeOverTilePlayerIndex].Name+" win the raise up with :" +Game.order.HighestRaiseUp+" cart");
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
            //Assert
            Assert.AreNotEqual(Game._Confing.Players[0].TilePack.Tiles, Game._Confing.Players[1].TilePack.Tiles);
          
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
