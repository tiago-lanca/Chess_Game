using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Queen : Piece
{
    public bool FirstMove { get; set; } = true;
    public Queen() { }
    public Queen(PieceType pieceType, Location location, PieceTeam team, string placeholder)
        : base(pieceType, location, team, placeholder)
    {
    }

    public override void MovePiece(Piece piece, Location fromLocation, Location toLocation, string input_FromPos, string input_ToPos, Piece[,] board)
    {
        List<string> possibleMoves = new List<string>();

        GetVerticalMoves(piece, fromLocation, possibleMoves, input_FromPos, board);
        GetHorizontalMoves(piece, fromLocation, possibleMoves, input_FromPos, board);
        GetDiagonalMoves(piece, fromLocation, possibleMoves, input_FromPos, board);

        MakePieceMove(piece, possibleMoves, fromLocation, toLocation, input_ToPos, board);
    }

    public static List<string> GetVerticalMoves(Piece piece, Location fromLocation, List<string> possibleMoves, string input_FromPos, Piece[,] board)
    {
        int col = fromLocation.Col;
        Piece nextPiece;

        // Calculo de movimentação vertical da Rainha

        for (int row = fromLocation.Row - 1; row >= 0; row--)
        {
            nextPiece = board[row, col];
            if (nextPiece == null || piece.Team != nextPiece.Team)
                possibleMoves.Add($"{char.ToUpper(input_FromPos[0])}{row + 1}");

            else break;
        }

        for (int row = fromLocation.Row + 1; row < board.GetLength(0); row++)
        {
            nextPiece = board[row, col];
            if (nextPiece == null || piece.Team != nextPiece.Team)
                possibleMoves.Add($"{char.ToUpper(input_FromPos[0])}{row + 1}");

            else break;
        }        

        return possibleMoves;
    }

    public static List<string> GetHorizontalMoves(Piece piece, Location fromLocation, List<string> possibleMoves, string input_FromPos, Piece[,] board)
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

    public static List<string> GetDiagonalMoves(Piece piece, Location fromLocation, List<string> possibleMoves, string input_FromPos, Piece[,] board)
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

