using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class King : Piece
{
    public King() { }
    public King(PieceType pieceType, Location location, PieceTeam team, string placeholder)
        : base(pieceType, location, team, placeholder)
    {
    }

    // Verificar se alguma peça da mesma equipa consegue o proteger:
    // Verificar as posições à volta dele e ver se existe alguma peça da mesma equipa se consegue
    // movimentar para frente dele.
    public override void MovePiece(Piece piece, Location fromLocation, Location toLocation, string input_FromPos, string input_ToPos, Piece[,] board)
    {
        List<string> possibleMoves = new List<string>();

        Get_VerticalMoves(piece, fromLocation, possibleMoves, input_FromPos, board);
        Get_HorizontalMoves(piece, fromLocation, possibleMoves, input_FromPos, board);
        Get_DiagonalMoves(piece, fromLocation, possibleMoves, input_FromPos, board);

        MakePieceMove(piece, possibleMoves, fromLocation, toLocation, input_ToPos, board);
    }

    public static List<string> Get_VerticalMoves(Piece piece, Location fromLocation, List<string> possibleMoves, string input_FromPos, Piece[,] board)
    {
        int rowUp = fromLocation.Row - 1;
        int rowDown = fromLocation.Row + 1;
        int column = char.ToUpper(input_FromPos[0]) - 'A';        
        Piece nextPiece;

        // Movimentação para cima
        if (rowUp >= 0)
        {
            nextPiece = board[rowUp, column];
            if (nextPiece == null || nextPiece.Team != piece.Team)
                possibleMoves.Add($"{char.ToUpper(input_FromPos[0])}{rowUp + 1}");
        }

        // Movimentação para baixo
        if (rowDown < board.GetLength(1))
        {
            nextPiece = board[rowDown, column];            
            if (nextPiece == null || nextPiece.Team != piece.Team)
                possibleMoves.Add($"{char.ToUpper(input_FromPos[0])}{rowDown + 1}");
        }

        return possibleMoves;
    }

    public static List<string> Get_HorizontalMoves(Piece piece, Location fromLocation, List<string> possibleMoves, string input_FromPos, Piece[,] board)
    {
        int row = fromLocation.Row;
        int columnLeft = char.ToUpper(input_FromPos[0]) - 'A' - 1;
        int columnRight = char.ToUpper(input_FromPos[0]) - 'A' + 1;

        Piece nextPiece;
        // Movimentação para a direita
        if (columnRight < board.GetLength(0))
        {
            nextPiece = board[row, columnRight];
            if (nextPiece == null || nextPiece.Team != piece.Team)
                possibleMoves.Add($"{char.ToUpper((char)(input_FromPos[0] + 1))}{fromLocation.Row + 1}");
        }

        if (columnLeft >= 0)
        {
            nextPiece = board[row, columnLeft];
            // Movimentação para a esquerda
            if (nextPiece == null || nextPiece.Team != piece.Team)
                possibleMoves.Add($"{char.ToUpper((char)(input_FromPos[0] - 1))}{fromLocation.Row + 1}");
        }

        return possibleMoves;
    }

    public static List<string> Get_DiagonalMoves(Piece piece, Location fromLocation, List<string> possibleMoves, string input_FromPos, Piece[,] board)
    {
        int rowUp = fromLocation.Row - 1;
        int rowDown = fromLocation.Row + 1;
        int columnLeft = char.ToUpper(input_FromPos[0]) - 'A' - 1;
        int columnRight = char.ToUpper(input_FromPos[0]) - 'A' + 1;                
        Piece nextPiece;

        // Movimentação para cima direita
        if (rowUp >= 0 && columnRight < board.GetLength(0))
        {
            nextPiece = board[rowUp, columnRight];
            if (nextPiece == null || nextPiece.Team != piece.Team)
                possibleMoves.Add($"{char.ToUpper((char)(input_FromPos[0] + 1))}{rowUp + 1}");
        }

        // Movimentação para cima esquerda
        if (rowUp >= 0 && columnLeft >= 0)
        {
            nextPiece = board[rowUp, columnLeft];
            if (nextPiece == null || nextPiece.Team != piece.Team)
                possibleMoves.Add($"{char.ToUpper((char)(input_FromPos[0] - 1))}{rowUp + 1}");
        }

        // Movimentação para baixo direita
        if (rowDown < board.GetLength(1) && columnRight < board.GetLength(0))
        {
            nextPiece = board[rowDown, columnRight];
            if (nextPiece == null || nextPiece.Team != piece.Team)
                possibleMoves.Add($"{char.ToUpper((char)(input_FromPos[0] + 1))}{rowDown + 1}");
        }

        // Movimentação para baixo esquerda
        if (rowDown < board.GetLength(1) && columnLeft >= 0)
        {
            nextPiece = board[rowDown, columnLeft];
            if (nextPiece == null || nextPiece.Team != piece.Team)
                possibleMoves.Add($"{char.ToUpper((char)(input_FromPos[0] - 1))}{rowDown + 1}");
        }

        return possibleMoves;
    }
}

