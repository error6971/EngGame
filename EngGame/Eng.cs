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

        public int Gameturn { private set; get; }

        public GameStatus Status { get; private set; } 

        public Random RandomSystem;

        #endregion

        #region Private prop

        public Confing _Confing { get; private set; }
        private const int RedChance = 35;//the chance of getting red in the carts
        public Betstatus betstatus { get; private set; }


        private bool LockBet;//for Betting Function
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
            GameData.PlayersBetAmount = new int[gameinfo.PlayerCount];
            _Confing = gameinfo;
            RandomSystem = new Random(Guid.NewGuid().GetHashCode());
            Status = new GameStatus();
            betstatus = new Betstatus();
        }

        public void StartTiling()
        {
            
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
            Tile cart = new Tile();



            byte color;
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
        public int NextPlayer(int currentPLayer = -1 )
        {
            if (currentPLayer <= 0)
            {

                if (Status.PlayerTurnIndex == _Confing.Players.Length - 1)
                    return 0;

                return Status.PlayerTurnIndex + 1;
            }
            else
            {
                if (currentPLayer == _Confing.Players.Length - 1)
                    return 0;

                return currentPLayer + 1;
            }
            
        }
        public int PreviousPlayer(int currentPLayer = -1)
        {
            if(currentPLayer <= 0)
            {

                if (Status.PlayerTurnIndex == 0)
                    return _Confing.Players.Length - 1;

                return Status.PlayerTurnIndex - 1;
            }
            else
            {
                if (currentPLayer == 0)
                    return _Confing.Players.Length - 1;

                return currentPLayer - 1;
            }


           
        }
        public void SetPlayerTurn(int PlayerIndex)
        {
            if (_Confing.Players.Length > PlayerIndex)
            {
                _Confing.Players[PlayerIndex].IsTurn = true;
              
                _Confing.Players[PreviousPlayer()].IsTurn = false;
                Status.PlayerTurnIndex = PlayerIndex;


            }
            else
            {
                SetPlayerTurn(0);
            }
        }

        public void Betting(int amount)
        {
            if (!LockBet)
            {
                Player player = _Confing.Players[Status.PlayerTurnIndex];

                if (betstatus.SetBet(amount, player) == 1)
                {
                    player.Token -= amount;
                    Status.AllTokenInTable += amount;
                    GameData.PlayersBetAmount[Status.PlayerTurnIndex] = amount;
                    if (betstatus.PlayerBetCount == _Confing.PlayerCount)
                    {
                        LockBet = true;
                        StartTiling();
                    }
                    SetPlayerTurn(NextPlayer());
                    Console.WriteLine("set bet " + amount + player.Name);
                }
                else { Console.WriteLine("Problem in Betting"); };


            }
            else Console.WriteLine("Betting is locked");
        }

        #endregion

        #region Private metod



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
            public int AllTokenInTable { get; set; }
            
        }
        #endregion

        #region Private Class

        #endregion


    }
    
}
