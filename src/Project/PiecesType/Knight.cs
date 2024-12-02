using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Numerics;
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
        List<string> possibleMovesKingCheck = new List<string>();

        King enemyKing = FindEnemyKing(piece, board);
        King friendKing = FindFriendKing(piece, board);

        if (friendKing.isCheck)
        {
            friendKing.GetAllMoves(friendKing, possibleMoves, board);
            Print_PossibleMovements(possibleMoves, input_ToPos);
        }
        else
        {
            // Calculo de movimentação do Cavalo
            GetAllMoves(piece, possibleMoves, board);

            if (IsValidMove(possibleMoves, input_ToPos))
            {
                MakePieceMove(piece, possibleMoves, fromLocation, toLocation, input_FromPos, input_ToPos, board);

                GetAllMoves(piece, possibleMovesKingCheck, board);
                if (IsEnemyKing_InCheck(piece, possibleMovesKingCheck, board))
                    Console.WriteLine($"{enemyKing.PlaceHolder} Rei em CHECK.\n");
            }
            else
                Print_PossibleMovements(possibleMoves, input_ToPos);
        }
    }

    public override List<string> GetAllMoves(Piece piece, List<string> possibleMoves, Piece[,] board)
    {
        return Get_VerticalMoves(piece, possibleMoves, board)
            .Concat(Get_HorizontalMoves(piece, possibleMoves,board))
            .ToList();
    }

    public override List<string> GetMoves_ForKingCheck(Piece piece, List<string> possibleMoves, Piece[,] board)
    {
        // HORIZONTAL MOVES

        int columnRight = piece.Location.Col + 2;
        int columnLeft = piece.Location.Col - 2;
        int rowUp = piece.Location.Row - 1;
        int rowDown = piece.Location.Row + 1;
        int char_RowUp = piece.Location.Row;
        int char_RowDown = piece.Location.Row + 2; // Cavalo anda 2 casas
        char char_ColumnRight = (char)(piece.Location.Col + 'A' + 2);
        char char_ColumnLeft = (char)(piece.Location.Col + 'A' - 2);

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
                possibleMoves.Add($"{char_ColumnRight}{char_RowUp}");
            }

            // Verifica se está dentro do limite esquerdo
            if (_IsInLeftLimits)
            {
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
                possibleMoves.Add($"{char_ColumnRight}{char_RowDown}");
            }

            // Verifica se está dentro do limite esquerdo
            if (_IsInLeftLimits)
            {
                possibleMoves.Add($"{char_ColumnLeft}{char_RowDown}");
            }
        }

        // VERTICAL MOVES

        columnRight = piece.Location.Col + 1;
        columnLeft = piece.Location.Col - 1;
        rowUp = piece.Location.Row - 2;
        rowDown = piece.Location.Row + 2;
        _IsInTopLimits = rowUp >= 0;
        char_RowUp = piece.Location.Row - 1;
        char_RowDown = piece.Location.Row + 3; // Cavalo anda 2 casas
        char_ColumnRight = char.ToUpper((char)(piece.Location.Col + 'A' + 1));
        char_ColumnLeft = char.ToUpper((char)(piece.Location.Col + 'A' - 1));

        if (_IsInTopLimits) // Verifica se está dentro do limite topo
        {
            if (columnRight < board.GetLength(1))
                    possibleMoves.Add($"{char_ColumnRight}{char_RowUp}");

            if (columnLeft >= 0)
                    possibleMoves.Add($"{char_ColumnLeft}{char_RowUp}");
        }

        if (rowDown < board.GetLength(0))
        {
            if (columnRight < board.GetLength(1))
                possibleMoves.Add($"{char_ColumnRight}{char_RowDown}");

            if (columnLeft >= 0)
                possibleMoves.Add($"{char_ColumnLeft}{char_RowDown}");
        }

        return possibleMoves;
    }

    public static List<string> Get_VerticalMoves(Piece piece, List<string> possibleMoves, Piece[,] board)
    {
        int columnRight = piece.Location.Col + 1;
        int columnLeft = piece.Location.Col - 1;
        int rowUp = piece.Location.Row - 2;
        int rowDown = piece.Location.Row + 2;
        bool _IsInTopLimits = rowUp >= 0;
        int char_RowUp = piece.Location.Row - 1;
        int char_RowDown = piece.Location.Row + 3; // Cavalo anda 2 casas
        char char_ColumnRight = char.ToUpper((char)(piece.Location.Col + 'A' + 1));
        char char_ColumnLeft = char.ToUpper((char)(piece.Location.Col + 'A' - 1));

        if (_IsInTopLimits) // Verifica se está dentro do limite topo
        {
            if (columnRight < board.GetLength(1))
                if (board[rowUp, columnRight] == null || board[rowUp, columnRight].Team != piece.Team)
                    possibleMoves.Add($"{char_ColumnRight}{char_RowUp}");

            if (columnLeft >= 0)
                if (board[rowUp, columnLeft] == null || board[rowUp, columnLeft].Team != piece.Team)
                    possibleMoves.Add($"{char_ColumnLeft}{char_RowUp}");

        }

        if (rowDown < board.GetLength(0))
        {

            if (columnRight < board.GetLength(1))
                if (board[rowDown, columnRight] == null || board[rowDown, columnRight].Team != piece.Team)
                    possibleMoves.Add($"{char_ColumnRight}{char_RowDown}");

            if (columnLeft >= 0)
                if (board[rowDown, columnLeft] == null || board[rowDown, columnLeft].Team != piece.Team)
                    possibleMoves.Add($"{char_ColumnLeft}{char_RowDown}");

        }

        return possibleMoves;  
    }

    public static List<string> Get_HorizontalMoves(Piece piece, List<string> possibleMoves, Piece[,] board)
    {
        int columnRight = piece.Location.Col + 2;
        int columnLeft = piece.Location.Col - 2;
        int rowUp = piece.Location.Row - 1;
        int rowDown = piece.Location.Row + 1;
        int char_RowUp = piece.Location.Row;
        int char_RowDown = piece.Location.Row + 2; // Cavalo anda 2 casas
        char char_ColumnRight = (char)(piece.Location.Col + 'A' + 2);
        char char_ColumnLeft = (char)(piece.Location.Col + 'A' - 2);

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

