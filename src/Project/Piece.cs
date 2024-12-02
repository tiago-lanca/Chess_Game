using System.Net;
using System.Net.NetworkInformation;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Serialization;

public class Piece
{
    public PieceType? Type { get; set; }
    public Location? Location { get; set; }
    public PieceTeam? Team { get; set; }
    public string PlaceHolder { get; set; }
    public bool isAlive { get; set; } = true;
    public bool SpecialOperation_ON { get; set; } = true;
    public int Nr_Movements { get; set; } = 0;
    public Piece() { }

    [JsonConstructor]
    public Piece(PieceType type, Location location, PieceTeam team, string placeholder)
    {
        Type = type;
        Location = location;
        Team = team;
        PlaceHolder = placeholder;
    }

    public virtual void MovePiece(Piece piece, Location fromLocation, Location toLocation, string input_FromPos, string input_ToPos, Piece[,] board)
    {

    }

    public virtual List<string> GetAllMoves(Piece piece, List<string> possibleMoves, Piece[,] board)
    {
        return possibleMoves;
    }

    public virtual List<string> GetMoves_ForKingCheck(Piece piece, List<string> possibleMoves, Piece[,] board)
    {
        return possibleMoves;
    }
    public void MakePieceMove(Piece piece, List<string> possibleMoves, Location fromLocation, Location toLocation, string input_FromPos, string input_ToPos, Piece[,] board)
    {
        // Informação se peça foi capturada ou movida
        PieceMoved_Info(piece, possibleMoves, toLocation, input_FromPos, input_ToPos, board);
        piece.Nr_Movements++;

        // Altera a peça de localização, e coloca null onde estava anteriormente
        board[toLocation.Row, toLocation.Col] = piece;
        board[toLocation.Row, toLocation.Col].Location.Col = toLocation.Col;
        board[toLocation.Row, toLocation.Col].Location.Row = toLocation.Row;
        board[fromLocation.Row, fromLocation.Col] = null;

        Game.Nr_Moves++;

        //Mudança de turno, do jogador a jogar

        //Game.Turn = !Game.Turn;
        //if (Math.Abs(fromLocation.Row - toLocation.Row) == 2) Verificar se andou 2 casas

    }

    public bool IsValidMove(List<string> possibleMoves, string input_ToPos)
        => possibleMoves.Any(move => move.Equals(input_ToPos, StringComparison.OrdinalIgnoreCase));

    public void Print_PossibleMovements(List<string> possibleMoves, string input_ToPos)
    {
        Console.WriteLine("Movimento inválido.\n");
        Console.Write("Possiveis Movimentações: ");
        if (possibleMoves.Count == 0) Console.Write("N/A\n");
        else
        {
            foreach (var move in possibleMoves)
                Console.Write($"{move} ");
            Console.WriteLine("\n");
        }
    }

    public virtual void PieceMoved_Info(Piece piece, List<string> possibleMoves, Location toLocation, string input_FromPos, string input_ToPos, Piece[,] board)
    {
        Piece toPositionPiece = board[toLocation.Row, toLocation.Col];

        if (toPositionPiece != null)
        {
            Console.WriteLine($"Peça {toPositionPiece.PlaceHolder} capturada.\n");
            toPositionPiece.isAlive = false;
        }
        else
            Console.WriteLine($"{piece.PlaceHolder} movimentada com sucesso.\n");
    }

    public King FindEnemyKing(Piece piece, Piece[,] board)
    {
        for(int row = 0; row < board.GetLength(0); row++)
        {
            for(int col = 0; col < board.GetLength(1); col++)
            {
                if (board[row, col] is King && board[row, col].Team != piece.Team)
                {
                    return (King)board[row, col];
                }
            }
        }
        return null;
    }

    public King FindFriendKing(Piece piece, Piece[,] board)
    {
        for (int row = 0; row < board.GetLength(0); row++)
        {
            for (int col = 0; col < board.GetLength(1); col++)
            {
                if (board[row, col] is King && board[row, col].Team == piece.Team)
                {
                    return (King)board[row, col];
                }
            }
        }
        return null;
    }

    public bool IsEnemyKing_Checkmate(King king, List<string> possibleMoves)
    
        => king.isCheck && possibleMoves.Count == 0;
    

    public bool IsEnemyKing_InCheck(Piece piece, List<string> possibleMoves, Piece[,] board)
    {
        return possibleMoves.Any(move => move.Equals(Get_KingCoord(FindEnemyKing(piece, board))));
    }
    public string Get_KingCoord(King king)
    {
        return $"{(char)(king.Location.Col + 'A')}{king.Location.Row + 1}";
    }

    public bool IsEnemyKing(Piece piece, int row, int col, Piece[,] board)
    {
        return board[row, col] != null && board[row, col] is King && board[row, col].Team != piece.Team;
    }

    public bool IsEnemyPiece(Piece piece, int row, int col, Piece[,] board)
    {
        return board[row, col] != null && board[row, col].Team != piece.Team;
    }
}

public class Location { 
    public int Row { get; set; }
    public int Col { get; set; }

    public Location(int row, int col) {
        Row = row;
        Col = col;
    }

    public override string ToString() => $"Row: {Row} / Col: {Col}";
    
}

public enum PieceTeam {
        White,
        Black
    }

public enum PieceType {
    Pawn,
    Bishop,
    Knight,
    Rook,
    Queen,
    King
}