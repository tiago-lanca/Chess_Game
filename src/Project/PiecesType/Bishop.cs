using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Bishop : Piece
{
    public Bishop() { }
    public Bishop(PieceType pieceType, Location location, PieceTeam team, string placeholder)
        : base(pieceType, location, team, placeholder)
    {
    }

    // Calcular movimentação nas diagonais, rowup + columnRight em ciclo for, rowup + columnLeft em for,
    // rowdown+columnRight em for, rowdown + columnLeft em for

    public override void MovePiece(Piece piece, Location fromLocation, Location toLocation, string input_FromPos, string input_ToPos, Piece[,] board)
    {
        List<string> possibleMoves = new List<string>();

        Get_DiagonalMoves(piece, possibleMoves, fromLocation, input_FromPos, board);

        MakePieceMove(piece, possibleMoves, fromLocation, toLocation, input_ToPos, board);
    }

   public static List<string> Get_DiagonalMoves(Piece piece, List<string> possibleMoves, Location fromLocation, string input_FromPos, Piece[,] board)
    {
        Piece nextPiece;
        int nextColumn = char.ToUpper(input_FromPos[0]);

        // Diagonal cima direita
        for (int nextRow = fromLocation.Row - 1; nextRow >= 0; nextRow--)
        {
            
            nextColumn += 1;            

            if ((nextColumn - 'A') >= board.GetLength(0)) break; // Verifica se a proxima coluna à direita está dentro dos limites do tabuleiro
            nextPiece = board[nextRow, nextColumn - 'A']; // Atribui à nextPiece uma "peça" na posiçao seguinte
            
            if (nextPiece == null || nextPiece.Team != piece.Team) // Verifica se a proxima peça não é nula ou se é da mesma equipa
                possibleMoves.Add($"{char.ToUpper((char)nextColumn)}{nextRow + 1}");

            else break;
        }

        nextColumn = char.ToUpper(input_FromPos[0]); // Reinicia o valor da proxima coluna para o valor inicial da coordenada[0]
        
        // Diagonal cima esquerda
        for (int nextRow = fromLocation.Row - 1; nextRow >= 0; nextRow--)
        {
            nextColumn -= 1;

            if ((nextColumn - 'A') < 0) 
                break;

            nextPiece = board[nextRow, nextColumn - 'A'];

            if (nextPiece == null || nextPiece.Team != piece.Team)
                possibleMoves.Add($"{char.ToUpper((char)nextColumn)}{nextRow + 1}");

            else break;

            
        }

        nextColumn = char.ToUpper(input_FromPos[0]);

        // Diagonal baixo direita
        for (int nextRow = fromLocation.Row + 1; nextRow < board.GetLength(1); nextRow++)
        {
            
            nextColumn += 1;
            if ((nextColumn - 'A') >= board.GetLength(0)) 
                break;

            nextPiece = board[nextRow, nextColumn - 'A'];
            if (nextPiece == null || nextPiece.Team != piece.Team) 
                possibleMoves.Add($"{char.ToUpper((char)nextColumn)}{nextRow + 1}");

            else break;


        }

        nextColumn = char.ToUpper(input_FromPos[0]);

        // Diagonal baixo esquerda
        for (int nextRow = fromLocation.Row + 1; nextRow < board.GetLength(1); nextRow++)
        {
            
            nextColumn -= 1;
            if ((nextColumn - 'A') < 0) 
                break;

            nextPiece = board[nextRow, nextColumn - 'A'];
            if (nextPiece == null || nextPiece.Team != piece.Team)
                possibleMoves.Add($"{char.ToUpper((char)nextColumn)}{nextRow + 1}"); 

            else break;

            
        }

        return possibleMoves;    
    }
}

