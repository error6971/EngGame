
using EngGame.Information;
using System;
using System.Collections.Generic;

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

		public freezer freezer { get; private set; }

		public PlayerStatus[] playerStatus { get; private set; }

		public TokenManager tokenManager { get; private set; }

		public Action EventUpdateStatus { private get; set; }

		public Action EventUpdateTurn { private get; set; }
		#endregion

		#region Private prop


		private const int RedChance = 20;//the chance of getting red in the carts


		#endregion

		#region Public metod
		//این متود برای رفتن به ترن بعدیه
		public void StartNextTurn()
		{
			
			Gameturn += 1;
			CheckPlayerOut();
			ChangePlayerSort();
			Status = new GameStatus();
			Status.Table = new Table[_Confing.PlayerCount];
			playerStatus = new PlayerStatus[_Confing.Players.Length];
			betstatus.ResetBetting();
			for (int i = 0; i < _Confing.PlayerCount; i++)
			{
				Status.Table[i] = new Table();
				playerStatus[i] = new PlayerStatus();

			}
		}

		//برای شروع کلاس و کار کردن بازی (این متود فقط در شروغ بازی اجرا میشود 
		public void Setup(Confing gameinfo, int[] betplace)
		{
			
			_Confing	 = gameinfo;
			RandomSystem = new Random(Guid.NewGuid().GetHashCode());
			Status		 = new GameStatus();
			betstatus	 = new Betstatus(betplace);
			Status.Table = new Table[gameinfo.PlayerCount];
			playerStatus = new PlayerStatus[gameinfo.PlayerCount];
			order		 = new Order(gameinfo.Players);
			freezer		 = new freezer();
			tokenManager = new TokenManager(gameinfo);

			for (int i = 0; i < gameinfo.PlayerCount; i++)
			{
				Status.Table[i] = new Table();
				playerStatus[i] = new PlayerStatus();

			}

			StartTurn();
		}

		// برای پخش کردن و ساخت کار برای هر بازی کنه
		public void StartTiling()
		{
			if (_Confing.PlayerCount == _Confing.Players.Length)
			{
				for (int i = 0; i < _Confing.Players.Length; i++)
				{

					Player Player = _Confing.Players[i];

					Player.Index = i;
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

		//برای گرفتن تمام کارت هایی که در دست بازیکن ها است
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

		//بزای گرفتن تمام کارت هایی که در میز هستند
		public TilePack[] ReturnInTableTiles()
		{
			TilePack[] AllTiles = new TilePack[Status.Table.Length];

			for (int i = 0; i < AllTiles.Length; i++)
			{
				AllTiles[i] = new TilePack();
			}
			for (int i = 0; i < Status.Table.Length; i++)
			{
				AllTiles[i].Tiles = new Tile[Status.Table[i].Tile.Count];

				for (int c = 0; c < Status.Table[i].Tile.Count; c++)
				{
					AllTiles[i].Tiles[c] = Status.Table[i].Tile[c];
				}

			}
			return AllTiles;
		}

		//برای انتخاب کارت و گذاشتن ان روی میز 
		public void ChoiceTile(int TileIndex)
		{
			if (!playerStatus[Status.PlayerTurnIndex].Lockchoice)
			{
				/// this metod get the chosen tile and select it 
				Tile[] OutPutTile = _Confing.Players[Status.PlayerTurnIndex].TilePack.Tiles;

				if (OutPutTile.Length > TileIndex)
				{
					Table table = Status.Table[Status.PlayerTurnIndex];
					Tile tile = OutPutTile[TileIndex];

					//check if it was null creat one
					if (table.Tile == null)
						table.Tile = new System.Collections.Generic.List<Tile>();

					// put the selected tile in table 
					tile.AssignedTo = Status.PlayerTurnIndex;
					table.Tile.Add(tile);


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
		
		//override
		public void ChoiceTile(int TileIndex, int playerindex)
		{
			if (!playerStatus[Status.PlayerTurnIndex].Lockchoice)
			{
				/// this class get the chosen tile and select it 

				Table table = Status.Table[playerindex];
				Tile[] OutPutTile = _Confing.Players[playerindex].TilePack.Tiles;

				if (OutPutTile.Length > TileIndex)
				{

					Tile tile = OutPutTile[TileIndex];

					tile.AssignedTo = playerindex;
					table.Tile.Add(tile);


					Status.TileInTableCount += 1;

					Extensions.RemoveAt<Tile>(ref OutPutTile, TileIndex);


					_Confing.Players[playerindex].TilePack.Tiles = OutPutTile;

					Status.Table[playerindex] = table;
				}
				else { Console.WriteLine("Tile Doesn`t exist"); }
			}
			else Console.WriteLine("Locked");
		}
		
		//برای متوقف کردن انخاب تایل (این متود برای رفتن به مرحله بهدی نیاز است)
		public void StopChoicingTile()
		{
			ResetPlayerTurn();

			for (int i = 0; i < playerStatus.Length; i++) playerStatus[i].Lockchoice = true;

		}
		
		//برای افزایش تعداد کارتی که بازیکن رو خواهد کرد 
		public void RaiseTakeOverTile(int Count)
		{
			if (Count > Status.TileInTableCount)
			{
				Console.WriteLine("Count is invide");
				Count = Status.TileInTableCount;

			}

			if (!playerStatus[Status.PlayerTurnIndex].LockRaise)
			{

				if (Status.TileInTableCount == Count)
				{
					Status.HighestRaiseUp = Count;
					Status.TakeOverTilePlayerIndex = Status.PlayerTurnIndex;

					for (int i = 0; i < playerStatus.Length; i++) if (Status.PlayerTurnIndex != i) playerStatus[i].LockRaise = true;

					Console.WriteLine("player " + _Confing.Players[Status.PlayerTurnIndex].Name + " set to takeover tiles : " + Count);
					if (EventUpdateStatus != null)
						EventUpdateStatus();

				}
				else if (Status.HighestRaiseUp < Count)
				{
					Status.HighestRaiseUp = Count;
					Status.TakeOverTilePlayerIndex = Status.PlayerTurnIndex;

					Console.WriteLine("player " + _Confing.Players[Status.PlayerTurnIndex].Name + " set to takeover tiles : " + Count);

					SetPlayerTurn(NextPlayer());
					if (EventUpdateStatus != null)
						EventUpdateStatus();
				}
				else if (Status.HighestRaiseUp >= Count)
				{
					playerStatus[Status.PlayerTurnIndex].LockRaise = true;

					SetPlayerTurn(NextPlayer());
					if (EventUpdateStatus != null)
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
		
		//گذینه مخصوص برای اینکه طرف تمام کارت های زوی میز را بردارد
		public void AllInEvent(int playerindex)
		{
			if (Gameturn >= 3)
			{
				PutAllTilesInTable();
			}
			SetPlayerTurn(playerindex);
			RaiseTakeOverTile(GetWhiteTileCount());
		}

		//برای گرفتن تهداد کارت های سفید روی میز
		public int GetWhiteTileCount()
		{
			var tiles = ReturnInTableTiles();
			int count = 0;

			for (int i = 0; i < tiles.Length; i++)
			{
				for (int c = 0; c < tiles[i].Tiles.Length; c++)
				{
					if (tiles[i].Tiles[c].color == TileColor.White)
						count++;
				}
			}


			return count;
		}
		
		// برای متود قبل که تمام کارت های بازی رو روی میز میگذارد
		public void PutAllTilesInTable()
		{
			var players = _Confing.Players;
			for (int i = 0; i < players.Length; i++)
			{
				playerStatus[i].Lockchoice = false;
				for (int c = 0; c < players[i].TilePack.Tiles.Length; c++)
				{
					ChoiceTile(c);
					
				}
			}
			StopChoicingTile();
		}
		
		//برای برداشتن کارتی که بازیکن انخاب کزده است
        public bool TakeOverTiles(int TableIndex = 0, int TileIndex = 0)
		{

			if (Status.HighestRaiseUp != 0)
			{
				Tile tile = Status.Table[TableIndex].Tile[TileIndex];

				if (tile.color == TileColor.White)
				{
					Status.TurnedUpTiles++;
					if (Status.TurnedUpTiles == Status.HighestRaiseUp)
					{

						SetWinner();
						return false;
					}
					return true;
				}
				else
				{
					SetLoser(tile);
					return false;
				}



			}
			else
			{
				Console.WriteLine("______----");
				return false;
			}
		}
		
		// برای عمیلیاتی که وقنی برنده انخاب شد 
		public void SetWinner()
		{
			freezer.AllFreezedToken += (Status.AllTokenInTable * 0.2f);


			float reward = (Status.AllTokenInTable * 0.8f);

			_Confing.Players[Status.PlayerTurnIndex].Token += (int)reward;

			tokenManager.AddToken((int)reward, _Confing.Players[Status.PlayerTurnIndex]);


		}
        
		// برای عمیلیاتی که وقنی بازنده انخاب شد
        public void SetLoser(Tile ChosenTile)
		{

			float penalty = (Status.AllTokenInTable * 0.9f);

			int reward = (int)(Status.AllTokenInTable - penalty);

			// give token to who have red cart
			_Confing.Players[ChosenTile.AssignedTo].Token += reward;
			tokenManager.AddToken(reward, _Confing.Players[ChosenTile.AssignedTo]);

			//freez tokens 
			freezer.AllFreezedToken += penalty;




		}
        
		//not Important
        public void CheckPlayers(Player[] Players)
		{
			for (int i = 0; i < Players.Length; i++)
			{
				if (Players[i].ID <= 0 || Players[i].Token <= 0)
					Console.Write(Players[i].Name.ToString() + ": Has problem ");

				//check the player can play or have problem
			}
		}
		
		//برای ساخت کارت 
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
        
		//not Important
        public void StartTurn()
		{
			Gameturn = 1;
			StartTiling();
			SetPlayerTurn(0);

		}
		
		//برای کرفتن ایندکس بازیکن بعدی
		public int NextPlayer(int currentPLayer = -1)
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
        
		//برای کرفتن ایندکس بازیکن قبلی
        public int PreviousPlayer(int currentPLayer = -1)
		{
			if (currentPLayer <= 0)
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
		
		//برای تعین نوبت بازیکن
		public void SetPlayerTurn(int PlayerIndex)
		{
			if (_Confing.Players.Length > PlayerIndex)
			{
				_Confing.Players[PlayerIndex].IsTurn = true;

				_Confing.Players[PreviousPlayer()].IsTurn = false;
				Status.PlayerTurnIndex = PlayerIndex;
				if (EventUpdateTurn != null)
					EventUpdateTurn();


			}
			else
			{
				SetPlayerTurn(0);
			}
		}
		
		//برای بگزدوندن نوبت بازکن
		public int GetPlayerTurnIndex()
		{
			if (Status != null)
			{
				return Status.PlayerTurnIndex;
			}
			return 0;
		}
		
		//برای عملیلات شرط بندی
		public bool Betting(int amount)
		{
			if (!playerStatus[Status.PlayerTurnIndex].LockBet)
			{
				Player player = _Confing.Players[Status.PlayerTurnIndex];

				if (betstatus.SetBet(amount, player) == 1)
				{
					//reduce the player token
					player.Token -= amount;
					tokenManager.ReduceToken(amount, player);
					//add the amount to table
					Status.AllTokenInTable += amount;


					//lock player to cant chooce again
					playerStatus[Status.PlayerTurnIndex].LockBet = true;

					Console.WriteLine("set bet " + amount + " " + player.Name);

					if (betstatus.PlayerBetCount == _Confing.PlayerCount)
					{
						// sorting player location
						betstatus.Sort();
						order.CreateNewOrder(betstatus.highestBets);





						//lock all player 
						foreach (var item in playerStatus)
						{
							item.LockBet = true;
						}
					}
					SetPlayerTurn(NextPlayer());
					return true;
				}
				else return false;


			}
			else return false;
		}
		
		// برای برگردوندن ترن ها به حالت عادی
		public void ResetPlayerTurn()
		{
			_Confing.Players[Status.PlayerTurnIndex].IsTurn = false;
			SetPlayerTurn(0);
		}
		
		// برای تغییر نوبت بازیکنها و تغییر ترتیب
		public void ChangePlayerSort()
		{
			Player[] newplayer = order.GetPlayerSort();
			_Confing.Players = newplayer;
			for (int i = 0; i < newplayer.Length; i++)
			{
				_Confing.Players[i].Index = i;

			}
		}
		
		// برای حدف بازیکن
		public void DeletePlayer(int index)
		{
			var player = _Confing.Players;
			var Table = Status.Table;
			Extensions.RemoveAt(ref player, index);
			Extensions.RemoveAt(ref Table, index);

			_Confing.PlayerCount--;

			_Confing.Players = player;
			Status.Table = Table;

		}
		
		// برای چک کردن اینکه بازی کنی نیاز به حذف دارد یا نه
		public void CheckPlayerOut()
		{
			var player = _Confing.Players;
			for (int i = 0; i < player.Length; i++)
			{
				if (player[i].Token <= 0)
				{
					DeletePlayer(i);
					Console.WriteLine("player deleted");
				}

				if (player[i].TilePack.Tiles.Length <= 0)
				{
                    DeletePlayer(i);
                    Console.WriteLine("player deleted");
                }


			} 
		}

		#endregion

		#region Private metod



		#endregion

		#region Public Class
		// کلاس تنظیمات
		public class Confing
		{
			public int PlayerCount { set; get; } = 4;//The Count of Player  
			public Player[] Players { set; get; }// the players who is in the game 
			public int PlayerStartCartCount { set; get; } = 4;// the count of cart at starter 

		}
		// کلاس وضعیت
		public class GameStatus
		{
			// the player turn 
			public int PlayerTurnIndex { set; get; } = 0;
			// sum of all token in table
			public int AllTokenInTable { get; set; }
			// every player has one table you can put tile and token in table
			public Table[] Table { set; get; }
			// the count of tiles in table
			public int TileInTableCount { set; get; }
			// the player who win the raise up and can takeover tiles
			public int TakeOverTilePlayerIndex { set; get; }
			// the count of tiles the player take overed
			public int TurnedUpTiles { set; get; }
			// the player must take over this count
			public int HighestRaiseUp { set; get; }
		}

		public class PlayerStatus
		{

			public bool LockBet { set; get; } = false;//for Betting Function
			public bool Lockchoice { set; get; } = false;// for ChoiceTile Function
			public bool LockRaise { set; get; } = false;// for RaiseTakeOverTile Function



		}
		//کلاس ترتیب
		public class Order
		{
			public Order(Player[] players)
			{
				for (int i = 0; i < players.Length; i++)
				{
					OldPlayers.Add(players[i]);

				}

			}

			private System.Collections.Generic.List<Player> OldPlayers { set; get; } = new System.Collections.Generic.List<Player>();

			private System.Collections.Generic.List<Player> NewPlayers { set; get; } = new System.Collections.Generic.List<Player>();

			public void CreateNewOrder(List<BetPlacement> NewSort)
			{
				for (int i = 0; i < NewSort.Count; i++)
				{
					for (int c = 0; c < OldPlayers.Count; c++)
					{

						if (NewSort[i].AssignedTo.Equals(OldPlayers[c]))
						{
							NewPlayers.Add(OldPlayers[c]);

							break;
						}


					}

				}
				OldPlayers = NewPlayers;
				NewPlayers = new List<Player>();
			}

			public Player[] GetPlayerSort()
			{
				Player[] players = new Player[OldPlayers.Count];

				if (players.Length != 0)
				{
					OldPlayers.CopyTo(players);
					return players;

				}

				return null;
			}

		}


		#endregion

		#region Private Class

		#endregion


	}

}
