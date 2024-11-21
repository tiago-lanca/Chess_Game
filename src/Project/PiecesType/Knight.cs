using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Transactions;

class Knight : Piece
{

    public Knight() { }
    public Knight(PieceType pieceType, Location location, PieceTeam team, string placeholder)
        : base(pieceType, location, team, placeholder)
    {
    }

    public override void MovePiece(Piece piece, Location fromLocation, Location toLocation, string input_FromPos, string input_ToPos, Piece[,] board)
    {
        List<string> possibleMoves = new List<string>();

        // Calculo de movimentação do Cavalo
        Get_VerticalMoves(piece, possibleMoves, fromLocation, input_FromPos, board);
        Get_HorizontalMoves(piece, possibleMoves, fromLocation, input_FromPos, board);

        if (IsValidMove(possibleMoves, input_ToPos))
            MakePieceMove(piece, possibleMoves, fromLocation, toLocation, input_ToPos, board);
        else
            Print_PossibleMovements(possibleMoves, input_ToPos);
    }

    public static List<string> Get_VerticalMoves(Piece piece, List<string> possibleMoves, Location fromLocation, string input_FromPos, Piece[,] board)
    {
        int columnRight = char.ToUpper(input_FromPos[0]) - 'A' + 1;
        int columnLeft = char.ToUpper(input_FromPos[0]) - 'A' - 1;
        int rowUp = fromLocation.Row - 2;
        int rowDown = fromLocation.Row + 2;
        bool _IsInTopLimits = rowUp >= 0;
        int char_RowUp = fromLocation.Row - 1;
        int char_RowDown = fromLocation.Row + 3; // Cavalo anda 2 casas
        char char_ColumnRight = char.ToUpper(Convert.ToChar(input_FromPos[0] + 1));
        char char_ColumnLeft = char.ToUpper(Convert.ToChar(input_FromPos[0] - 1));

        if (_IsInTopLimits) // Verifica se está dentro do limite topo
        {
            if (piece.Team == PieceTeam.White) // WHITE TEAM
            {
                if (columnRight < board.GetLength(1))
                    if (board[rowUp, columnRight] == null || board[rowUp, columnRight].Team != PieceTeam.White)
                        possibleMoves.Add($"{char_ColumnRight}{char_RowUp}");
                
                if(columnLeft >= 0)
                    if (board[rowUp, columnLeft] == null || board[rowUp, columnLeft].Team != PieceTeam.White)
                        possibleMoves.Add($"{char_ColumnLeft}{char_RowUp}");
            }
            else // BLACK TEAM
            {
                if (columnRight < board.GetLength(1))
                    if (board[rowUp, columnRight] == null || board[rowUp, columnRight].Team != PieceTeam.Black)
                        possibleMoves.Add($"{char_ColumnRight}{char_RowUp}");

                if (columnLeft >= 0)
                    if (board[rowUp, columnLeft] == null || board[rowUp, columnLeft].Team != PieceTeam.Black)
                        possibleMoves.Add($"{char_ColumnLeft}{char_RowUp}");
            }
        }

        if(rowDown < board.GetLength(0))
        {
            if (piece.Team == PieceTeam.White) // WHITE TEAM
            {
                if (columnRight < board.GetLength(1))
                    if (board[rowDown, columnRight] == null || board[rowDown, columnRight].Team != PieceTeam.White)
                        possibleMoves.Add($"{char_ColumnRight}{char_RowDown}");

                if (columnLeft >= 0)
                    if (board[rowDown, columnLeft] == null || board[rowDown, columnLeft].Team != PieceTeam.White)
                        possibleMoves.Add($"{char_ColumnLeft}{char_RowDown}");
            }
            else // BLACK TEAM
            {
                if (columnRight < board.GetLength(1))
                    if (board[rowDown, columnRight] == null || board[rowDown, columnRight].Team != PieceTeam.Black)
                        possibleMoves.Add($"{char_ColumnRight}{char_RowDown}");

                if (columnLeft >= 0)
                    if (board[rowDown, columnLeft] == null || board[rowDown, columnLeft].Team != PieceTeam.Black)
                        possibleMoves.Add($"{char_ColumnLeft}{char_RowDown}");
            }
        }
        return possibleMoves;  
    }

    public static List<string> Get_HorizontalMoves(Piece piece, List<string> possibleMoves, Location fromLocation, string input_FromPos, Piece[,] board)
    {
        int columnRight = char.ToUpper(input_FromPos[0]) - 'A' + 2;
        int columnLeft = char.ToUpper(input_FromPos[0]) - 'A' - 2;
        int rowUp = fromLocation.Row - 1;
        int rowDown = fromLocation.Row + 1;
        int char_RowUp = fromLocation.Row;
        int char_RowDown = fromLocation.Row + 2; // Cavalo anda 2 casas
        char char_ColumnRight = char.ToUpper(Convert.ToChar(input_FromPos[0] + 2));
        char char_ColumnLeft = char.ToUpper(Convert.ToChar(input_FromPos[0] - 2));

        bool _IsInTopLimits = rowUp >= 0;
        bool _IsInRightLimits = columnRight < board.GetLength(1);
        bool _IsInLeftLimits = columnLeft >= 0;
        bool _IsInDownLimits = rowDown < board.GetLength(0);

        // Obter movimentações andando para DIR./ESQ. E CIMA ->->^  ^<-<-
        if (_IsInTopLimits)
        {
            // Verifica se está dentro do limite direito
            if (_IsInRightLimits)
            {
                // Verifica se a peça 2 casas à direita e 1 para cima é nula ou de equipa inimiga.
                if (board[rowUp, columnRight] == null || board[rowUp, columnRight].Team != piece.Team)
                    possibleMoves.Add($"{char_ColumnRight}{char_RowUp}");
            }

            // Verifica se está dentro do limite esquerdo
            if (_IsInLeftLimits)
            {
                // Verifica se a peça 2 casas à esquerda e 1 para cima é nula ou de equipa inimiga.
                if (board[rowUp, columnLeft] == null || board[rowUp, columnLeft].Team != piece.Team)
                    possibleMoves.Add($"{char_ColumnLeft}{char_RowUp}");
            }
        }

        // Obter movimentações andando para DIR./ESQ. E BAIXO ->->v  v<-<-

        // Verifica se está dentro do limite inferior
        if (_IsInDownLimits)
        {
            // Verifica se está dentro do limite direito
            if (_IsInRightLimits)
            {
                // Verifica se a peça 2 casas à direita e 1 para baixo é nula ou de equipa inimiga.
                if (board[rowDown, columnRight] == null || board[rowDown, columnRight].Team != piece.Team)
                    possibleMoves.Add($"{char_ColumnRight}{char_RowDown}");
            }

            // Verifica se está dentro do limite esquerdo
            if (_IsInLeftLimits)
            {
                // Verifica se a peça 2 casas à esquerda e 1 para baixo é nula ou de equipa inimiga.
                if (board[rowDown, columnLeft] == null || board[rowDown, columnLeft].Team != piece.Team)
                    possibleMoves.Add($"{char_ColumnLeft}{char_RowDown}");
            }
        }
        return possibleMoves;
    }        
}

