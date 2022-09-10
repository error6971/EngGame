using EngGame;
using EngGame.Information;
using EngGame.Data;
using System;


namespace EngGame
{
    public class Eng
    {

        /// <summary>
        /// The Base Eng Class
        /// </summary>

        #region Public prop

        public Confing GameInfo { get; set; } 
        public int Gameturn { private set; get; }

        public GameStatus Status { get; private set; } 

        public Random RandomSystem;

        #endregion

        #region Private prop

        public Confing _Confing { get; private set; }
        private const int RedChance = 35;//the chance of getting red in the carts

        #endregion

        #region Public metod

        public void StartGame()
        {
           
                if (_Confing.PlayerCount == _Confing.Players.Length)
                {
                    for (int i = 0; i < _Confing.Players.Length; i++)
                    {

                        Player Player = _Confing.Players[i];
                        Player.Token = RandomSystem.Next(1, 200);//only for test !!!!
                        Player.Location = i;
                        Player.LocationColor = (LocationColor)i;
                        Player.Tiles = new Tile[_Confing.PlayerStartCartCount];
                        for (int c = 0; c < _Confing.PlayerStartCartCount; c++)
                        {
                            //Create The Begining Cart At Start
                            Player.Tiles[i] = CreateTile();
                        }
                        _Confing.Players[i] = Player;

                    }

                }
                else { Console.Write("Someting Wrong Here Player Losted"); return; }
        }

        public void Setup(Confing gameinfo)
        {
            GameData.Players = gameinfo.Players;
            _Confing = gameinfo;
            RandomSystem = new Random(Guid.NewGuid().GetHashCode());
            Status = new GameStatus();
        }

        

        public void CheckPlayers(Player[] Players)
        {
            for (int i = 0; i < Players.Length; i++)
            {
                if (Players[i].ID == 0 || Players[i].Token == 0)
                    Console.Write(Players[i].Name.ToString() + ": Has problem ");

                //check the player can play or have problem
            }
        }

        public Tile CreateTile()
        {
            byte color = 0;

            Tile cart = new Tile();

            

            if (RedChance >= RandomSystem.Next(100))//chance  of getting red
                color = 1;
            else
                color = 0;

            cart.color = (TileColor)color;


            return cart;
        }
        
        public void StartTurn()
        {
            Gameturn = 1;
            SetPlayerTurn(0);

        }
        
        public void SetPlayerTurn(int PlayerIndex)
        {
            _Confing.Players[PlayerIndex].IsTurn = true;
            Status.PlayerTurnIndex = PlayerIndex;
        }

        public void Betting()
        {
            foreach (var player in _Confing.Players)
            {

            }
        }

        #endregion

        #region Private metod

        private void Start()
        {
            //Confing gameInfo = new Confing();
            // gameinfo.everyting = someting
            CheckPlayers(GameInfo.Players);

            Setup(GameInfo);


            StartGame();
            Betting();
            
        }//only for test

        #endregion

        #region Public Class

        public class Confing
        {
            public int PlayerCount { set; get; } = 4;//The Count of Player  
            public Player[] Players { set; get; }// the players who is in the game 
            public int PlayerStartCartCount { set; get; } = 4;// the count of cart at starter 

        }

        public class GameStatus
        {
            public int PlayerTurnIndex { set; get; } = 0;
            
        }
        #endregion

        #region Private Class

        #endregion


    }
    
}
