using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

public class Pawn : Piece
{
    public bool FirstMove { get; set; } = true;
    public Pawn() => Type = PieceType.Pawn;
    public Pawn(PieceType type, Location location, PieceTeam team, string placeholder)
        :base(type, location, team, placeholder)
    {

    }
    public override void MovePiece(Piece pawn, Location fromLocation, Location toLocation, string input_FromPos, string input_ToPos, Piece[,] board)
    {
        List<string> possibleMoves = new List<string>();

        if (pawn.Team == PieceTeam.White) // WHITE TEAM
        {
            // Verifica posições na diagonal
            GetPawnDiagonalMoves((Pawn)pawn, possibleMoves, fromLocation, input_FromPos, board);
            // Verifica posições na vertical
            GetPawnVerticalMoves((Pawn)pawn, possibleMoves, fromLocation, input_FromPos, board);
        }
        else // BLACK TEAM
        {
            // Verifica posições na diagonal
            GetPawnDiagonalMoves((Pawn)pawn, possibleMoves, fromLocation, input_FromPos, board);
            // Verifica posições na vertical
            GetPawnVerticalMoves((Pawn)pawn, possibleMoves, fromLocation, input_FromPos, board);
        }

        // Faz a movimentação da peça, se possivel
        MakePawnPieceMove((Pawn)pawn, possibleMoves, fromLocation, toLocation, input_ToPos, board);
        PromotePawn((Pawn)pawn, board);
    }

    public static void MakePawnPieceMove(Pawn pawn, List<string> possibleMoves, Location fromLocation, Location toLocation, string input_ToPos, Piece[,] board)
    {
        if (possibleMoves.Any(m => m.Equals(input_ToPos, StringComparison.OrdinalIgnoreCase)))
        {
            // Altera a peça de localização, e coloca null onde estava anteriormente
            board[toLocation.Row, toLocation.Col] = pawn;
            board[toLocation.Row, toLocation.Col].Location.Col = toLocation.Col;
            board[toLocation.Row, toLocation.Col].Location.Row = toLocation.Row;
            board[fromLocation.Row, fromLocation.Col] = null;

            Board.PrintBoard(board);

            Console.WriteLine($"{pawn.PlaceHolder} movimentada com sucesso.\n");
            pawn.FirstMove = false;

            //if (Math.Abs(fromLocation.Row - toLocation.Row) == 2) Verificar se andou 2 casas
        }
        else
        {
            Console.WriteLine("Movimento inválido.\n");
            Console.Write("Possiveis Movimentações: ");
            if (possibleMoves.Count == 0) Console.Write("N/A\n");
            foreach (var move in possibleMoves)
                Console.Write($"{move} ");
            Console.WriteLine("");
        }
    }

    public static List<string> GetPawnDiagonalMoves(Pawn piece, List<string> possibleMoves, Location fromLocation, string input_FromPos, Piece[,] board)
    {
        int column, row = fromLocation.Row + 1;
        Piece nextLeftColumnPiece, nextRightColumnPiece;
        
        //White Team
        if (piece.Team == PieceTeam.White)
        {
            // Verifica se a coluna esq. está dentro dos limites
            if (fromLocation.Col - 1 >= 0)
            {
                nextLeftColumnPiece = board[fromLocation.Row - 1, fromLocation.Col - 1];

                // Verifica se a coluna dir. está dentro dos limites
                if (fromLocation.Col + 1 < board.GetLength(1))
                {
                    nextRightColumnPiece = board[fromLocation.Row - 1, fromLocation.Col + 1];
                    // Coluna à esquerda
                    if (nextLeftColumnPiece != null && nextLeftColumnPiece.Team != piece.Team)
                    {
                        column = input_FromPos[0] - 1;
                        possibleMoves.Add($"{char.ToUpper((char)column)}{row - 1}");
                    }

                    if (nextRightColumnPiece != null && nextRightColumnPiece.Team != piece.Team)
                    {
                        // Coluna à direita
                        column = input_FromPos[0] + 1;
                        possibleMoves.Add($"{char.ToUpper((char)column)}{row - 1}");
                    }
                }
                else // Coluna à direita está fora dos limites. So calcula a esq.
                {
                    if (nextLeftColumnPiece != null && nextLeftColumnPiece.Team != piece.Team)
                    {
                        column = input_FromPos[0] - 1;
                        possibleMoves.Add($"{char.ToUpper((char)column)}{row - 1}");
                    }
                }
            }
            // Coluna em cima à esquerda está fora do tabuleiro. Só dá para cima e direita
            else
            {
                nextRightColumnPiece = board[fromLocation.Row - 1, fromLocation.Col + 1];
                if (nextRightColumnPiece.Team != piece.Team)
                {
                    column = input_FromPos[0] + 1;
                    possibleMoves.Add($"{char.ToUpper((char)column)}{row - 1}");
                }
            }
        }
        //Black Team
        else
        {
            // Verifica se a coluna esq. está dentro dos limites
            if (fromLocation.Col - 1 >= 0)
            {
                nextLeftColumnPiece = board[fromLocation.Row + 1, fromLocation.Col - 1];

                // Verifica se a coluna dir. está dentro dos limites
                if (fromLocation.Col + 1 < board.GetLength(1))
                {
                    nextRightColumnPiece = board[fromLocation.Row + 1, fromLocation.Col + 1];
                    // Coluna à esquerda
                    if (nextLeftColumnPiece != null && nextLeftColumnPiece.Team != piece.Team)
                    {
                        column = input_FromPos[0] - 1;
                        possibleMoves.Add($"{char.ToUpper((char)column)}{row + 1}");
                    }

                    // Coluna à direita
                    if (nextRightColumnPiece != null && nextRightColumnPiece.Team != piece.Team)
                    {
                        column = input_FromPos[0] + 1;
                        possibleMoves.Add($"{char.ToUpper((char)column)}{row + 1}");

                    }
                }

                else // Coluna à direita está fora dos limites. So calcula a esq.
                {
                    if (nextLeftColumnPiece != null && nextLeftColumnPiece.Team != piece.Team)
                    {
                        column = input_FromPos[0] - 1;
                        possibleMoves.Add($"{char.ToUpper((char)column)}{row + 1}");
                    }
                }                
            }

            // Coluna em baixo à esquerda está fora do tabuleiro. Só dá para baixo e direita
            else
            {                
                nextRightColumnPiece = board[fromLocation.Row + 1, fromLocation.Col + 1];

                if (nextRightColumnPiece != null && nextRightColumnPiece.Team != piece.Team)
                {
                    column = input_FromPos[0] + 1;
                    possibleMoves.Add($"{char.ToUpper((char)column)}{row + 1}");
                }
            }
        }

        return possibleMoves;
    }

    public List<string> GetPawnVerticalMoves(Pawn piece, List<string> possibleMoves, Location fromLocation, string input_FromPos, Piece[,] board)
    {
        Piece nextPiece;
        int row = fromLocation.Row + 1;
        
        // WHITE TEAM
        if (piece.Team == PieceTeam.White)
        {
            nextPiece = board[fromLocation.Row - 1, fromLocation.Col];
            //se a posiçao em cima for null, adiciona posiçao à possibleMove
            if (nextPiece == null)
            {
                possibleMoves.Add($"{char.ToUpper(input_FromPos[0])}{row - 1}");
                if (piece.FirstMove && board[fromLocation.Row - 2, fromLocation.Col] == null) 
                    possibleMoves.Add($"{char.ToUpper(input_FromPos[0])}{row - 2}");
            }
        }
        
        //BLACK TEAM
        else
        {
            nextPiece = board[fromLocation.Row + 1, fromLocation.Col];
            if (nextPiece == null)
            {
                possibleMoves.Add($"{char.ToUpper(input_FromPos[0])}{row + 1}");
                if (piece.FirstMove && board[fromLocation.Row + 2, fromLocation.Col] == null) 
                    possibleMoves.Add($"{char.ToUpper(input_FromPos[0])}{row + 2}");
            }
        }

        return possibleMoves;
    }

    public void PromotePawn(Pawn piece, Piece[,] board)
    {
        if(piece.Team == PieceTeam.White && piece.Location.Row == 0)
        {
            Console.WriteLine($"Peão {piece.PlaceHolder} promovido.\n");

        }
        if(piece.Team == PieceTeam.Black && piece.Location.Row == board.GetLength(1) - 1)
            Console.WriteLine($"Peão {piece.PlaceHolder} promovido.\n");
    }
}

