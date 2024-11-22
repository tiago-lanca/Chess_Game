using System.Net;
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

    public void MakePieceMove(Piece piece, List<string> possibleMoves, Location fromLocation, Location toLocation, string input_FromPos, string input_ToPos, Piece[,] board)
    {
        // Informação se peça foi capturada ou movida
        _PieceMoved_Info(piece, possibleMoves, toLocation, input_FromPos, input_ToPos, board);
        piece.Nr_Movements++;

        // Altera a peça de localização, e coloca null onde estava anteriormente
        board[toLocation.Row, toLocation.Col] = piece;
        board[toLocation.Row, toLocation.Col].Location.Col = toLocation.Col;
        board[toLocation.Row, toLocation.Col].Location.Row = toLocation.Row;
        board[fromLocation.Row, fromLocation.Col] = null;        

        Game.Nr_Moves++;
        //if (Math.Abs(fromLocation.Row - toLocation.Row) == 2) Verificar se andou 2 casas

    }

    public bool IsValidMove(List<string> possibleMoves, string input_ToPos)
        => possibleMoves.Any(move => move.Equals(input_ToPos, StringComparison.OrdinalIgnoreCase));

    public void Print_PossibleMovements(List<string> possibleMoves, string input_ToPos)
    {
        if (!IsValidMove(possibleMoves, input_ToPos))
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
    }

    public virtual void _PieceMoved_Info(Piece piece, List<string> possibleMoves, Location toLocation, string input_FromPos, string input_ToPos, Piece[,] board)
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