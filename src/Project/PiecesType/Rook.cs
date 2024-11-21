using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

class Rook : Piece
{    
    public bool FirstMove { get; set; } = true;

    public Rook() { }
    public Rook(PieceType type, Location location, PieceTeam team, string placeholder)
        : base(type, location, team, placeholder)
    {
    }

    public override void MovePiece(Piece piece, Location fromLocation, Location toLocation, string input_FromPos, string input_ToPos, Piece[,] board)
    {
        Piece nextPiece;
        Rook rook = piece as Rook;
        List<string> possibleMoves = new List<string>();

        // Calculo de movimentação vertical e horizontal da Torre , equipa WHITE
        Get_VerticalMovement((Rook)piece, fromLocation, possibleMoves, input_FromPos, board);
        Get_HorizontalMovement((Rook)piece, fromLocation, possibleMoves, input_FromPos, board);

        // Calculo de movimentação vertical e horizontal da Torre , equipa BLACK
        Get_VerticalMovement((Rook)piece, fromLocation, possibleMoves, input_FromPos, board);
        Get_HorizontalMovement((Rook)piece, fromLocation, possibleMoves, input_FromPos, board);


        if (IsValidMove(possibleMoves, input_ToPos))
        {
            MakePieceMove(piece, possibleMoves, fromLocation, toLocation, input_ToPos, board);
            rook.FirstMove = false;
        }
        else
            Print_PossibleMovements(possibleMoves, input_ToPos);
    }

    public List<string> Get_VerticalMovement(Piece piece, Location fromLocation, List<string> possibleMoves, string input_FromPos, Piece[,] board)
    {
        int col = fromLocation.Col;
        Piece nextPiece;

        // Calculo de movimentação vertical da Torre
        
        for (int row = fromLocation.Row - 1; row >= 0; row--)
        {
            nextPiece = board[row, col];

            if(nextPiece == null)
                possibleMoves.Add($"{char.ToUpper(input_FromPos[0])}{row + 1}");

            else if (nextPiece != null && piece.Team != nextPiece.Team)
            {
                possibleMoves.Add($"{char.ToUpper(input_FromPos[0])}{row + 1}");
                break;
            }
            else break;
        }

        for (int row = fromLocation.Row + 1; row < board.GetLength(0); row++)
        {
            nextPiece = board[row, col];

            if (nextPiece == null)
                possibleMoves.Add($"{char.ToUpper(input_FromPos[0])}{row + 1}");

            else if (nextPiece != null && piece.Team != nextPiece.Team)
            {
                possibleMoves.Add($"{char.ToUpper(input_FromPos[0])}{row + 1}");
                break;
            }
            else break;
        }        

        return possibleMoves;
    }

    public List<string> Get_HorizontalMovement(Piece piece, Location fromLocation, List<string> possibleMoves, string input_FromPos, Piece[,] board)
    {
        int row = fromLocation.Row; // Representação da linha na board
        Piece nextPiece;

        // Calculo para movimentação para a direita
        for (int col = fromLocation.Col + 1; col < board.GetLength(1); col++)
        {
            nextPiece = board[row, col];
            // Calculo do char da coluna com o percorrer do ciclo for
            int column = input_FromPos[0] + (col - fromLocation.Col);

            /* Para evitar que o loop páre quando a proxima peça é inimiga e nao continuar a acrescentar
               possiveis movimentações. Se for nulo, adiciona às possiveis movimentações e continua no ciclo a verificar
               as proximas posições. */
            if(nextPiece == null)
                possibleMoves.Add($"{char.ToUpper((char)column)}{row + 1}");

            else if (nextPiece != null && piece.Team != nextPiece.Team)
            {
                possibleMoves.Add($"{char.ToUpper((char)column)}{row + 1}"); // Representação visual UI coord.
                break;
            }
            else break;
        }
        // Calculo para movimentação para a esquerda
        for (int col = fromLocation.Col - 1; col >= 0; col--)
        {
            nextPiece = board[row, col];
            // Calculo do char da coluna com o percorrer do ciclo for
            int column = input_FromPos[0] - (fromLocation.Col - col);

            /* Para evitar que o loop páre quando a proxima peça é inimiga e nao continuar a acrescentar
               possiveis movimentações. Se for nulo, adiciona às possiveis movimentações e continua no ciclo a verificar
               as proximas posições. */
            if (nextPiece == null)
                possibleMoves.Add($"{char.ToUpper((char)column)}{row + 1}");

            else if (nextPiece != null && piece.Team != nextPiece.Team)
            {
                possibleMoves.Add($"{char.ToUpper((char)column)}{row + 1}"); // Representação visual UI coord.
                break;
            }
            else break;
        }        

        return possibleMoves;
    }

}

