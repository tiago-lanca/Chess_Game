using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Pawn : Piece
{
    public bool FirstMove { get; set; } = true;
    public Pawn() => Type = PieceType.Pawn;
    public Pawn(PieceType type, Location location, PieceTeam team, string placeholder)
        :base(type, location, team, placeholder)
    {

    }
    public override void MovePiece(Location fromLocation, Location toLocation, string input_FromPos, string input_ToPos, PieceTeam team, Piece[,] board)
    {
        Pawn piece = (Pawn)board[fromLocation.Row, fromLocation.Col];
        Piece nextPiece;
        List<string> possibleMoves = new List<string>();
        int row = fromLocation.Row + 1;

        if (team == PieceTeam.White) // WHITE TEAM
        {            
            nextPiece = board[fromLocation.Row - 1, fromLocation.Col];
            if (nextPiece != null)
            {
                if (nextPiece.Team == PieceTeam.White)
                    Console.WriteLine("Movimento inválido. Peça da mesma equipa à frente.\n");
                else
                {
                    Console.WriteLine($"Peça inimiga em frente. {board[fromLocation.Row - 1, fromLocation.Col].Team}\n");
                    possibleMoves.Add($"{char.ToUpper(input_FromPos[0])}{row - 1}");
                }
            }
            else
            {
                possibleMoves.Add($"{char.ToUpper(input_FromPos[0])}{row - 1}");
                if (piece.FirstMove) possibleMoves.Add($"{char.ToUpper(input_FromPos[0])}{row - 2}");
            }
        }
        else // BLACK TEAM
        {
            nextPiece = board[fromLocation.Row +1, fromLocation.Col];
            if (nextPiece != null)
            {
                if (nextPiece.Team == PieceTeam.Black)
                    Console.WriteLine("Movimento inválido. Peça da mesma equipa à frente.\n");
                else
                {
                    Console.WriteLine($"Peça inimiga em frente. {board[fromLocation.Row + 1, fromLocation.Col].Team}\n");
                    possibleMoves.Add($"{char.ToUpper(input_FromPos[0])}{row + 1}");
                }
            }
            else
            {
                possibleMoves.Add($"{char.ToUpper(input_FromPos[0])}{row + 1}");
                if (piece.FirstMove) possibleMoves.Add($"{char.ToUpper(input_FromPos[0])}{row + 2}");
            }
        }

        if (possibleMoves.Any(m => m.Equals(input_ToPos, StringComparison.OrdinalIgnoreCase)))
        {   
            // Altera a peça de localização, e coloca null onde estava anteriormente
            board[toLocation.Row, toLocation.Col] = piece;
            board[toLocation.Row, toLocation.Col].Location.Col = toLocation.Col;
            board[toLocation.Row, toLocation.Col].Location.Row = toLocation.Row;
            board[fromLocation.Row, fromLocation.Col] = null;
            
            Board.PrintBoard(board);

            Console.WriteLine($"{piece.PlaceHolder} movimentada com sucesso.\n");
            piece.FirstMove = false;

            //if (Math.Abs(fromLocation.Row - toLocation.Row) == 2) Verificar se andou 2 casas
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

