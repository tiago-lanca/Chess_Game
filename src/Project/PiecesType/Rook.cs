using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

public class Rook : Piece
{
    public Rook() { }
    public Rook(PieceType type, Location location, PieceTeam team, string placeholder)
        : base(type, location, team, placeholder)
    {
    }

    public override void MovePiece(Piece piece, Location fromLocation, Location toLocation, string input_FromPos, string input_ToPos, Piece[,] board)
    {
        Piece nextPiece;
        List<string> possibleMoves = new List<string>();
        int col = fromLocation.Col;

        
        if (piece.Team == PieceTeam.White)
        {
            // Calculo de movimentação vertical e horizontal da Torre , equipa WHITE
            VerticalMovement(piece, fromLocation, possibleMoves, input_FromPos, board);
            HorizontalMovement(piece, fromLocation, possibleMoves, input_FromPos, board);
        }
        else
        {
            // Calculo de movimentação vertical e horizontal da Torre , equipa BLACK
            VerticalMovement(piece, fromLocation, possibleMoves, input_FromPos, board);
            HorizontalMovement(piece, fromLocation, possibleMoves, input_FromPos, board);
        }

        MakePieceMove(piece, possibleMoves, fromLocation, toLocation, input_ToPos, board);
    }

    public List<string> VerticalMovement(Piece piece, Location fromLocation, List<string> possibleMoves, string input_FromPos, Piece[,] board)
    {
        int col = fromLocation.Col;
        Piece nextPiece;

        // Calculo de movimentação vertical da Torre
        
        for (int row = fromLocation.Row - 1; row >= 0; row--)
        {
            nextPiece = board[row, col];
            if (nextPiece == null || piece.Team != nextPiece.Team)
            {
                possibleMoves.Add($"{char.ToUpper(input_FromPos[0])}{row + 1}");
            }
            else break;
        }

        for (int row = fromLocation.Row + 1; row < board.GetLength(0); row++)
        {
            nextPiece = board[row, col];
            if (nextPiece == null || piece.Team != nextPiece.Team)
            {
                possibleMoves.Add($"{char.ToUpper(input_FromPos[0])}{row + 1}");
            }
            else break;
        }        

        return possibleMoves;
    }

    public List<string> HorizontalMovement(Piece piece, Location fromLocation, List<string> possibleMoves, string input_FromPos, Piece[,] board)
    {
        int row = fromLocation.Row; // Representação da linha na board
        Piece nextPiece;

        // Calculo para movimentação para a direita
        for (int col = fromLocation.Col + 1; col < board.GetLength(1); col++)
        {
            nextPiece = board[row, col];
            // Calculo do char da coluna com o percorrer do ciclo for
            int column = input_FromPos[0] + (col - fromLocation.Col);

            if (nextPiece == null || piece.Team != nextPiece.Team)
            {
                possibleMoves.Add($"{char.ToUpper((char)column)}{row + 1}"); // Representação visual UI coord.
            }
            else break;
        }
        // Calculo para movimentação para a esquerda
        for (int col = fromLocation.Col - 1; col >= 0; col--)
        {
            nextPiece = board[row, col];
            // Calculo do char da coluna com o percorrer do ciclo for
            int column = input_FromPos[0] - (fromLocation.Col - col);

            if (nextPiece == null || piece.Team != nextPiece.Team)
            {
                possibleMoves.Add($"{char.ToUpper((char)column)}{row + 1}"); // Representação visual UI coord.
            }
            else break;
        }        

        return possibleMoves;
    }

}

