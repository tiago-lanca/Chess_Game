using System.Net;
using System.Net.NetworkInformation;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Serialization;
using System.Xml.Linq;

public class Piece
{
    public PieceType? Type { get; set; }
    public Location? Location { get; set; }
    public PieceTeam? Team { get; set; }
    public string PlaceHolder { get; set; }
    public bool isAlive { get; set; } = true;
    public bool SpecialOperation_Enable { get; set; } = true;
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

    public virtual void SpecialOperation(Piece piece, Location fromLocation, Location toLocation, string input_FromPos, string input_ToPos, Piece[,] board)
    {

    }

    /*public virtual List<string> GetAllMoves(Piece piece, List<string> possibleMoves, Piece[,] board)
    {
        return possibleMoves;
    }*/

    public virtual List<string> GetMoves_ForCheckKing(Piece piece, List<string> possibleMoves, Piece[,] board)
    {
        return possibleMoves;
    }
    public void MakePieceMove(Piece piece, List<string> possibleMoves, Location fromLocation, Location toLocation, string input_FromPos, string input_ToPos, Piece[,] board)
    {
        // Mostra informação de movimentação da peça
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
        
    }

    public void MakePieceMove_NoInfo(Piece piece, List<string> possibleMoves, Location fromLocation, Location toLocation, string input_FromPos, string input_ToPos, Piece[,] board)
    {
        piece.Nr_Movements++;

        // Altera a peça de localização, e coloca null onde estava anteriormente
        board[toLocation.Row, toLocation.Col] = piece;
        board[toLocation.Row, toLocation.Col].Location.Col = toLocation.Col;
        board[toLocation.Row, toLocation.Col].Location.Row = toLocation.Row;
        board[fromLocation.Row, fromLocation.Col] = null;

        Game.Nr_Moves++;

        //Mudança de turno, do jogador a jogar
        //Game.Turn = !Game.Turn;

    }

    public bool IsValidMove(List<string> possibleMoves, string input_ToPos)
        => possibleMoves.Any(move => move.Equals(input_ToPos, StringComparison.OrdinalIgnoreCase));

    public void Print_PossibleMovements(List<string> possibleMoves)
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

    public void Testing_PiecePosition(Piece piece, Location fromLocation, Location toLocation, Piece[,] board)
    {
        if (board[toLocation.Row, toLocation.Col] is not null)
        {
            Game.savePiece = board[toLocation.Row, toLocation.Col];
            board[toLocation.Row, toLocation.Col] = piece;            
            board[fromLocation.Row, fromLocation.Col] = null;
        }
        else
        {
            board[toLocation.Row, toLocation.Col] = piece;
            board[fromLocation.Row, fromLocation.Col] = null;
        }

        // Atualiza as coordenadas da classe da peça
        piece.Location.Row = toLocation.Row;
        piece.Location.Col = toLocation.Col;
    }

    public void Undo_PiecePosition(Piece piece, Location fromLocation, Location toLocation, Piece[,] board)
    {
        if (Game.savePiece is null)
        {
            board[fromLocation.Row, fromLocation.Col] = piece;
            board[toLocation.Row, toLocation.Col] = null;
        }
        else
        {
            board[toLocation.Row, toLocation.Col] = Game.savePiece;
            board[fromLocation.Row, fromLocation.Col] = piece;            
            Game.savePiece = null;
        }

        piece.Location.Row = fromLocation.Row;
        piece.Location.Col = fromLocation.Col;
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

    public bool IsEnemyKing_Checkmate(King king, List<string> possibleMoves, Piece[,] board)
        => king.isCheck && possibleMoves.Count == 0;    

    public List<string> EnemyKing_MovesAvoidingCheckmate(King enemyKing, Piece[,] board) {

        List<string> enemyKing_AvoidCheckmate = new List<string>();
        List<string> positions_ToRemove = new List<string>();

        enemyKing.Get_KingValidPossibleMoves(enemyKing, enemyKing_AvoidCheckmate, board);

        foreach (string position in enemyKing_AvoidCheckmate)
        {
            Location fromLocationTest = new Location(enemyKing.Location.Row, enemyKing.Location.Col);
            Location toLocationTest = new Location(position[1] - '1', position[0] - 'A');

            Testing_PiecePosition(enemyKing, fromLocationTest, toLocationTest, board);
            if (enemyKing.IsKing_InCheck(board))
            {
                positions_ToRemove.Add(position);
            }

            Undo_PiecePosition(enemyKing, fromLocationTest, toLocationTest, board);
        }

        foreach (string position in positions_ToRemove)
        {
            enemyKing_AvoidCheckmate.Remove(position);
        }

        enemyKing.IsKing_InCheck(board); // Atualiza o estado de Check do enemyKing

        return enemyKing_AvoidCheckmate;

    }
    public void FinishGame_Complete()
    {
        Player player1 = PlayerList.players.Find(player => player.Name == Game.Player1.Name);
        Player player2 = PlayerList.players.Find(player => player.Name == Game.Player2.Name);

        Game.FinishGame_Checkmate(player1, player2);
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

    public override string ToString()
    {
        return $"{PlaceHolder}\n";
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