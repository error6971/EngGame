using EngGame.Information;
using EngGame.Data;
using System;
using System.Linq;
using System.Diagnostics.Contracts;

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
		public Order order { get; private set; }
		public Random RandomSystem;
        public Confing _Confing { get; private set; }
        public Betstatus betstatus { get; private set; }
        public PlayerStatus[] playerStatus { get; private set; }
		public Action EventUpdateStatus {private get;  set; }
		public Action EventUpdateTurn {private get;  set; }
        #endregion

        #region Private prop


        private const int RedChance = 30;//the chance of getting red in the carts
		


		private bool Lockchoice; // for ChoiceTile Function
		
		 
        #endregion

        #region Public metod



        public void Setup(Confing gameinfo)
		{
			GameData.Players = gameinfo.Players;
			GameData.PlayersBetAmount = new int[gameinfo.PlayerCount];
			_Confing     = gameinfo;
			RandomSystem = new Random(Guid.NewGuid().GetHashCode());
			Status       = new GameStatus();
			betstatus    = new Betstatus();
			Status.Table = new Table[gameinfo.PlayerCount];
			playerStatus = new PlayerStatus[gameinfo.PlayerCount];
			order        = new Order();
			for (int i = 0; i < gameinfo.PlayerCount; i++)
			{
				Status.Table[i] = new Table();
				playerStatus[i] = new PlayerStatus();
				
            }
		}

		public void StartTiling()
		{
			if (_Confing.PlayerCount == _Confing.Players.Length)
			{
				for (int i = 0; i < _Confing.Players.Length; i++)
				{

					Player Player = _Confing.Players[i];
					
					Player.Location = i;
					Player.LocationColor = (LocationColor)i;
					TilePack playerTiles = new TilePack();
					playerTiles.Tiles = new Tile[_Confing.PlayerStartCartCount];
					Player.TilePack = playerTiles;
					for (int c = 0; c < _Confing.PlayerStartCartCount; c++)
					{
						//Create The Begining Cart At Start
						Player.TilePack.Tiles[c] = CreateTile();
					}
					_Confing.Players[i] = Player;
					
				}

			}
			else { Console.Write("Someting Wrong Here Player Losted"); return; }
		}
		public TilePack[] ReturnTiles()
		{
			
			if (_Confing.Players[0].TilePack != null)
			{
				
				TilePack[] playerTiles = new TilePack[_Confing.PlayerCount];

				for (int i = 0; i < _Confing.PlayerCount; i++)
				{
					playerTiles[i] = new TilePack();
					
					
					playerTiles[i].Tiles = _Confing.Players[i].TilePack.Tiles;
					
				}
				return playerTiles;
			}
			return null;
		}

		public void ChoiceTile(int TileIndex)
		{
			if (!Lockchoice)
			{
				/// this class get the chosen tile and select it 
				Tile[] OutPutTile = _Confing.Players[Status.PlayerTurnIndex].TilePack.Tiles;

				if (OutPutTile.Length > TileIndex)
				{
					Table table = Status.Table[Status.PlayerTurnIndex];
					Tile tile = OutPutTile[TileIndex];

					//check if it was null creat one
					if(table.Tile == null)
						table.Tile = new System.Collections.Generic.List<Tile>() ;

					// put the selected tile in table 
					table.Tile.Add(tile) ;
					table.AssignedTo = _Confing.Players[Status.PlayerTurnIndex];

					Status.TileInTableCount += 1;

					// lets remove selected tile in base
					Extensions.RemoveAt<Tile>(ref OutPutTile, TileIndex);

					// save the work
					_Confing.Players[Status.PlayerTurnIndex].TilePack.Tiles = OutPutTile;
					Status.Table[Status.PlayerTurnIndex] = table;

					// finally we set next player turn
					SetPlayerTurn(NextPlayer());
				}
				else { Console.WriteLine("Tile Doesn`t exist"); }

			}
			else Console.WriteLine("Locked");
		}
		public void ChoiceTile(int TileIndex, int playerindex)
		{
			if (!Lockchoice)
			{
				/// this class get the chosen tile and select it 

				Table table = Status.Table[playerindex];
				Tile[] OutPutTile = _Confing.Players[playerindex].TilePack.Tiles;

				if (OutPutTile.Length > TileIndex)
				{

					Tile tile = OutPutTile[TileIndex];

					table.Tile.Add(tile);
					table.AssignedTo = _Confing.Players[playerindex];

					Status.TileInTableCount += 1;

					Extensions.RemoveAt<Tile>(ref OutPutTile, TileIndex);


					_Confing.Players[playerindex].TilePack.Tiles = OutPutTile;

					Status.Table[playerindex] = table;
				}
				else { Console.WriteLine("Tile Doesn`t exist"); }
			}
			else Console.WriteLine("Locked");
		}

		public void StopChoicingTile()
		{
			ResetPlayerTurn();
			Lockchoice = true;
		}

		public void RaiseTakeOverTile(int Count)
		{
			if(Count > Status.TileInTableCount)
			{
				Console.WriteLine("Count is invide");
				Count = Status.TileInTableCount;
	   
			}

			if (!playerStatus[Status.PlayerTurnIndex].LockRaise)
			{

				if (Status.TileInTableCount == Count)
				{
					order.HighestRaiseUp = Count;
					Status.TakeOverTilePlayerIndex = Status.PlayerTurnIndex;

					for (int i = 0; i < playerStatus.Length; i++) if (Status.PlayerTurnIndex != i) playerStatus[i].LockRaise = true;
					
					Console.WriteLine("player " + _Confing.Players[Status.PlayerTurnIndex].Name + " set to takeover tiles : " + Count);
                    EventUpdateStatus();
					
				}
				else if (order.HighestRaiseUp < Count)
				{
					order.HighestRaiseUp = Count;
					Status.TakeOverTilePlayerIndex = Status.PlayerTurnIndex;

					Console.WriteLine("player " + _Confing.Players[Status.PlayerTurnIndex].Name + " set to takeover tiles : " + Count);

					SetPlayerTurn(NextPlayer());
                    EventUpdateStatus();
				}
				else if (order.HighestRaiseUp >= Count)
				{
					playerStatus[Status.PlayerTurnIndex].LockRaise = true;
                    
                    SetPlayerTurn(NextPlayer());
                    EventUpdateStatus();
                }

			}
			else
			{
				int count = 0;

				//Check how many player pass and cant raise

                Extensions.EqualsCount<PlayerStatus>(playerStatus, i => i.LockRaise == true, ref count);

				//if there is left only one player or nobody dont go next player and done the raise up

                if (playerStatus.Length - 1 > count)
				{
					SetPlayerTurn(NextPlayer());
                    RaiseTakeOverTile(count);
					
                }
				//Done
			}

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
				EventUpdateTurn();


			}
			else
			{
				SetPlayerTurn(0);
			}
		}
		public int GetPlayerTurnIndex()
		{
			if (Status != null)
			{
				return Status.PlayerTurnIndex;
			}
			return 0;
		}
		public void Betting(int amount)
		{
			if (!playerStatus[Status.PlayerTurnIndex].LockBet)
			{
				Player player = _Confing.Players[Status.PlayerTurnIndex];

				if (betstatus.SetBet(amount, player) == 1)
				{
					player.Token -= amount;
					Status.AllTokenInTable += amount;
					GameData.PlayersBetAmount[Status.PlayerTurnIndex] = amount;
					playerStatus[Status.PlayerTurnIndex].LockBet = true;

                    Console.WriteLine("set bet " + amount + player.Name);

					if (betstatus.PlayerBetCount == _Confing.PlayerCount)
					{
						
						StartTiling();
					}
					SetPlayerTurn(NextPlayer());
					
				}
				else { Console.WriteLine("Problem in Betting"); };


			}
			else Console.WriteLine("Betting is locked");
		}

		public void ResetPlayerTurn()
		{
			_Confing.Players[Status.PlayerTurnIndex].IsTurn = false;
			SetPlayerTurn(0);
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

			public Table[] Table { set; get; }
			public int TileInTableCount { set; get; }

			public int TakeOverTilePlayerIndex;

		}

		public class PlayerStatus
		{
			public int PlayerIndex { set; get; }
			public bool LockBet { set; get; } = false;//for Betting Function
			public bool Lockchoice { set; get; } = false;// for ChoiceTile Function
			public bool LockRaise { set; get; } = false;// for RaiseTakeOverTile Function

			

        }
		public class Order
		{
			public int HighestRaiseUp ;
		}

		
		#endregion

		#region Private Class

		#endregion


	}
	
}
