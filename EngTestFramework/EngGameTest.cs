using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Configuration;
using EngGame;
using EngGame.Information;

namespace EngTestFramework
{
    [TestClass]
    public class EngGameTest
    {
       

        [TestMethod]
        
        public void TestMethod1()
        {
            //assing

            Eng game = new Eng();
            Eng.Confing confing = new Eng.Confing();
            confing.PlayerCount = 4;

            confing.Players = new Player[4];
            confing.Players[0] = new Player().PlayerSetup(1,"Reza",80);
            confing.Players[1] = new Player().PlayerSetup(2, "ali", 120);
            confing.Players[2] = new Player().PlayerSetup(3, "mohamad", 60);
            confing.Players[3] = new Player().PlayerSetup(4, "fateme", 95);
            confing.PlayerStartCartCount = 4;

            //Act

            game.CheckPlayers(confing.Players);
            game.Setup(confing);
            game.StartGame();
            game.Betting();


            //Assert
            Assert.AreNotEqual(game._Confing.Players[0].Tiles, game._Confing.Players[1].Tiles);
        }
    }
}
