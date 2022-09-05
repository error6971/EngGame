using System;

namespace Ludo
{
    public class Ludo
    {
        public Slot[] slots { get; private set; }
        public Piece[] pieces { get; private set; }
        public Player[] players { get; private set; }
        public Area[] areas { get; private set; }

        public int turn { get; private set; }
        public Dice[] dices { get; private set; }
        public bool diceRolled { get; private set; }

        public Player player => players[turn];

        public Random random { get; private set; }

        public void Setup(GameConfig config)
        {
            Setup(config, Guid.NewGuid().GetHashCode());
        }
        public void Setup(GameConfig config, int seed)
        {
            slots = new Slot[config.slots.Length];
            pieces = new Piece[config.pieces.Length];
            players = new Player[config.players.Length];
            areas = new Area[config.areas.Length];
            turn = 0;
            dices = new Dice[] { };
            diceRolled = false;
            random = new Random(seed);
            for (int i = 0; i < slots.Length; i++) slots[i] = new Slot();
            for (int i = 0; i < pieces.Length; i++) pieces[i] = new Piece();
            for (int i = 0; i < players.Length; i++) players[i] = new Player();
            for (int i = 0; i < areas.Length; i++) areas[i] = new Area();
            for (int i = 0; i < slots.Length; i++) slots[i]._Setup(this, i, config.slots[i]);
            for (int i = 0; i < pieces.Length; i++) pieces[i]._Setup(this, i, config.pieces[i]);
            for (int i = 0; i < players.Length; i++) players[i]._Setup(this, i, config.players[i]);
            for (int i = 0; i < areas.Length; i++) areas[i]._Setup(this, i, config.areas[i]);
            for (int i = 0; i < slots.Length; i++) slots[i].Setup();
            for (int i = 0; i < pieces.Length; i++) pieces[i].Setup();
            for (int i = 0; i < players.Length; i++) players[i].Setup();
            for (int i = 0; i < areas.Length; i++) areas[i].Setup();
        }

        public bool CanPlay(int playerIndex, out int movePieceIndex, out int diceIndex)
        {
            movePieceIndex = -1;
            diceIndex = -1;
            if (turn != playerIndex) return false;
            if (!diceRolled) return true;
            for (int i = 0; i < pieces.Length; i++)
            {
                for (int j = 0; j < dices.Length; j++)
                {
                    if (pieces[i].player == turn
                        && CanMove(playerIndex, i, j))
                    {
                        movePieceIndex = i;
                        diceIndex = j;
                        return true;
                    }
                }
            }
            return false;
        }

        public bool CanDice(int playerIndex)
        {
            return !diceRolled && turn == playerIndex;
        }

        public bool CanMove(int playerIndex, int pieceIndex, out int diceIndex)
        {
            diceIndex = -1;
            for (int j = 0; j < dices.Length; j++)
            {
                if (pieces[pieceIndex].player == turn
                    && CanMove(playerIndex, pieceIndex, j))
                {
                    diceIndex = j;
                    return true;
                }
            }
            return false;
        }
        public bool CanMove(int playerIndex, int pieceIndex, int diceIndex)
        {
            if (!TryGetDice(diceIndex, out var dice)) return false;
            if (!diceRolled || turn != playerIndex) return false;
            if (!TryGetPlayer(turn, out var player)) return false;
            if (!TryGetPiece(pieceIndex, out var piece)) return false;
            if (piece.player != turn) return false;
            if (piece.position == 0 && dice.value != 6) return false;
            if (!piece.CanMoveDice(dice)) return false;
            var targetSlotIndex = piece.GetSlotIndex(dice);
            if (TryGetPieceAt(targetSlotIndex, out var targetPiece))
            {
                if (targetPiece.player == turn && targetPiece.isFinish)
                {
                    return false;
                }
            }
            return true;
        }

        public void ThrowDice()
        {
            var dice = new Dice(random.Next(1, 7));
            if (pieces.Count(e => e.area == players[turn].area && e.position > 0) == 0
                && dices.Count(e => e.value == 6) == 0)
            {
                dice.value = 6;
            }
            AddDice(dice);
            // AddDice(new Dice(random.Next(1, 7)));
            // AddDice(new Dice(random.Next(1, 7)));
            if (dice.value < 6)
            {
                diceRolled = true;
            }
        }

        public void Move(int pieceIndex, int diceIndex)
        {
            var piece = GetPiece(pieceIndex);
            if (piece.position == 0)
                piece.position = 1;
            else
                piece.position += dices[diceIndex].value;
            var targetPiece = GetPieceAt(piece.slotIndex, piece.area);
            if (targetPiece != null && targetPiece.player != turn)
            {
                targetPiece.position = 0;
            }
            RemoveDice(diceIndex);
            if (dices.Length == 0)
                NextTurn();
        }

        public void NextTurn()
        {
            diceRolled = false;
            dices = new Dice[] { };
            turn++;
            if (turn >= players.Length)
            {
                turn = 0;
            }
        }

        public void UpdateScore()
        {
            players[turn].score = pieces.Count(e => e.player == turn && e.isFinish);
        }

        public bool IsWinner()
        {
            // if (turn == 3) return true;
            var winScore = players[turn].config.winScore;
            if (winScore <= 0) winScore = areas[players[turn].area].piecesCount;
            if (players[turn].score >= players[turn].config.winScore)
            {
                return true;
            }
            return false;
        }

        public void AddDice(Dice dice)
        {
            var newDices = new Dice[dices.Length + 1];
            for (int i = 0; i < dices.Length; i++)
                newDices[i] = dices[i];
            newDices[newDices.Length - 1] = dice;
            dices = newDices;
        }
        public void RemoveDice(int diceIndex)
        {
            var newDices = new Dice[dices.Length - 1];
            for (int i = 0; i < newDices.Length; i++)
            {
                if (i < diceIndex)
                    newDices[i] = dices[i];
                else
                    newDices[i] = dices[i + 1];
            }
            dices = newDices;
        }

        public bool TryGetPlayer(int index, out Player player)
        {
            player = null;
            if (index < 0 || index >= players.Length) return false;
            player = players[index];
            return true;
        }
        public bool TryGetDice(int index, out Dice dice)
        {
            dice = null;
            if (index < 0 || index >= dices.Length) return false;
            dice = dices[index];
            return true;
        }
        public bool TryGetPiece(int index, out Piece piece)
        {
            piece = null;
            if (index < 0 || index >= pieces.Length) return false;
            piece = pieces[index];
            return true;
        }
        public bool TryGetPieceAt(int slotIndex, out Piece piece)
        {
            piece = pieces.Find(e => e.slotIndex == slotIndex);
            return piece != null;
        }

        public Piece GetPiece(int index)
        {
            return pieces[index];
        }
        public Piece GetPieceAt(int slotIndex, int excludeArea)
        {
            return pieces.Find(e => e.slotIndex == slotIndex && e.area != excludeArea);
        }

        public GameData GetData()
        {
            return new GameData()
            {
                turn = turn,
                dices = dices.Select(e => new DiceData() { value = e.value }),
                diceRolled = diceRolled,
                pieces = pieces.Select(e => e.GetData()),
            };
        }

        public void LoadDate(GameData data)
        {
            turn = data.turn;
            dices = data.dices.Select(e => new Dice(e.value));
            diceRolled = data.diceRolled;
            for (int i = 0; i < data.pieces.Length; i++)
            {
                pieces[i].LoadData(data.pieces[i]);
            }
        }

        public GameResult GetResult()
        {
            var winners = new int[players.Length];
            var scores = new int[players.Length];
            for (int i = 0; i < players.Length; i++)
            {
                scores[i] = players[i].score;
                winners[i] = players[i].area;
            }
            for (int i = 0; i < scores.Length; i++)
            {
                for (int j = 0; j < scores.Length; j++)
                {
                    if (scores[i] < scores[j])
                    {
                        var tempScore = scores[i];
                        var tempWinner = winners[i];
                        scores[i] = scores[j];
                        winners[i] = winners[j];
                        scores[j] = tempScore;
                        winners[j] = tempWinner;
                    }
                }
            }
            return new GameResult()
            {
                winners = winners
            };
        }
    }

    public class LudoProp<TConfig>
    {
        public Ludo game { get; private set; }
        public int index { get; private set; }
        public TConfig config { get; private set; }

        public virtual void _Setup(Ludo game, int index, TConfig config)
        {
            this.game = game;
            this.index = index;
            this.config = config;
        }

        public virtual void Setup() { }
    }

    public class Dice
    {
        public int value;

        public Dice(int value)
        {
            this.value = value;
        }
    }

    public class Slot : LudoProp<SlotConfig> { }

    public class Piece : LudoProp<PieceConfig>
    {
        public int area => config.area;
        public int[] path => config.path;

        public int slotIndex => path[position];
        public bool isFinish => path.Length - position <= game.areas[area].piecesCount;

        public int player;
        public int position;

        public override void Setup()
        {
            base.Setup();
            position = 0;
            player = game.players.FindIndex(e => e.area == area);
        }

        public bool CanMoveDice(Dice dice)
        {
            return position + dice.value < path.Length - 1;
        }

        public int GetSlotIndex(Dice dice)
        {
            return path[position + dice.value];
        }

        public PieceData GetData()
        {
            return new PieceData() { position = position };
        }
        public void LoadData(PieceData data)
        {
            position = data.position;
        }
    }

    public class Player : LudoProp<PlayerConfig>
    {
        public int area => config.area;
        public int score { get; set; } = 0;
    }

    public class Area : LudoProp<AreaConfig>
    {
        public int piecesCount { get; private set; }

        public override void Setup()
        {
            base.Setup();
            piecesCount = game.pieces.Count(e => e.area == index);
        }
    }

    [System.Serializable]
    public struct PieceData
    {
        public int position;
    }

    [System.Serializable]
    public struct DiceData
    {
        public int value;
    }

    [System.Serializable]
    public class SlotConfig { public byte _boo = 0; }

    [System.Serializable]
    public class PieceConfig
    {
        public int area;
        public int[] path;
    }

    [System.Serializable]
    public class PlayerConfig
    {
        public int area;
        public int winScore;
    }

    [System.Serializable]
    public class AreaConfig { public byte _boo = 0; }

    [System.Serializable]
    public struct GameData
    {
        public int turn;
        public bool diceRolled;

        public DiceData[] dices;
        public PieceData[] pieces;
    }

    [System.Serializable]
    public class GameResult
    {
        public int[] winners = new int[] { };
    }

    [System.Serializable]
    public class GameConfig
    {
        public SlotConfig[] slots;
        public PieceConfig[] pieces;
        public PlayerConfig[] players;
        public AreaConfig[] areas;
    }

    public static class Extensions
    {
        public delegate bool Predicate<T>(T t);
        public delegate R Convert<T, R>(T t);

        public static int FindIndex<T>(this T[] arr, Predicate<T> predicate)
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

        public static T Find<T>(this T[] arr, Predicate<T> predicate)
        {
            var index = arr.FindIndex(predicate);
            if (index != -1) return arr[index];
            return default(T);
        }

        public static int Count<T>(this T[] arr, Predicate<T> predicate)
        {
            var result = 0;
            for (int i = 0; i < arr.Length; i++)
            {
                if (predicate.Invoke(arr[i]))
                {
                    result++;
                }
            }
            return result;
        }

        public static R[] Select<T, R>(this T[] arr, Convert<T, R> convert)
        {
            var result = new R[arr.Length];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = convert.Invoke(arr[i]);
            }
            return result;
        }
    }
}
