using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Pawn : Piece
{
    private bool FirstMove { get; set; } = true;

    public static void MovePawn(Location fromLocation, Location toLocation, string input_FromPos, string input_ToPos, PieceTeam team, Piece[,] board)
    {
        Piece piece = new Piece();
        List<string> possibleMoves = new List<string>();
        int row = fromLocation.Row + 1;

        if (team == PieceTeam.White) // WHITE TEAM
        {
            possibleMoves.Add($"{char.ToUpper(input_FromPos[0])}{row - 1}");
        }
        else 
            possibleMoves.Add($"{char.ToUpper(input_FromPos[0])}{row + 1}");

        if (possibleMoves.Any(m => m.Equals(input_ToPos, StringComparison.OrdinalIgnoreCase)))
        {
            piece = board[fromLocation.Row, fromLocation.Col];
            // Altera a peça de localização, e coloca null onde estava anteriormente
            board[toLocation.Row, toLocation.Col] = piece;
            board[toLocation.Row, toLocation.Col].Location.Col = toLocation.Col;
            board[toLocation.Row, toLocation.Col].Location.Row = toLocation.Row;
            board[fromLocation.Row, fromLocation.Col] = null;
            Board.PrintBoard(board);

            Console.WriteLine($"{piece.PlaceHolder} movimentada com sucesso.\n");
        }
        else
        {
            Console.WriteLine("Movimento inválido.\n");
            Console.Write("Possiveis Movimentações: ");
            foreach (var move in possibleMoves)
                Console.Write($"{move} ");
            Console.WriteLine("");
        }        
    }
}

