using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Numerics;
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
        King enemyKing = FindEnemyKing(piece, board);
        King friendKing = FindFriendKing(piece, board);

        List<string> possibleMoves = new List<string>();
        List<string> possibleMovesKingCheck = new List<string>();


        if (friendKing.isCheck)
        {
            friendKing.Get_KingValidPossibleMoves(friendKing, possibleMoves, board);
            Print_PossibleMovements(possibleMoves, input_ToPos);
        }
        else
        {
            // Calculo de movimentação vertical e horizontal da Torre
            GetAllMoves((Rook)piece, possibleMoves, board);

            if (IsValidMove(possibleMoves, input_ToPos))
            {
                MakePieceMove(piece, possibleMoves, fromLocation, toLocation, input_FromPos, input_ToPos, board);
                rook.FirstMove = false;

                GetAllMoves(piece, possibleMovesKingCheck, board);
                if (IsEnemyKing_InCheck(piece, possibleMovesKingCheck, board))
                {
                    enemyKing.isCheck = true;
                    Console.WriteLine($"{enemyKing.PlaceHolder} Rei em CHECK.\n");
                }
            }
            else
                Print_PossibleMovements(possibleMoves, input_ToPos);
        }
    }
    public override List<string> GetAllMoves(Piece piece, List<string> possibleMoves, Piece[,] board)
    {
        return Get_VerticalMovement((Rook)piece,possibleMoves, board)
            .Concat(Get_HorizontalMovement((Rook)piece, possibleMoves, board))
            .ToList();
    }

    public override List<string> GetMoves_ForKingCheck(Piece piece, List<string> possibleMoves, Piece[,] board)
    {
        for (int row = piece.Location.Row - 1; row >= 0; row--)
        {
            possibleMoves.Add($"{(char)(piece.Location.Col + 'A')}{row + 1}");
        }

        for (int row = piece.Location.Row + 1; row < board.GetLength(0); row++)
        {
            possibleMoves.Add($"{(char)(piece.Location.Col + 'A')}{row + 1}");
        }

        for (int col = piece.Location.Col + 1; col < board.GetLength(1); col++)
        {
            // Calculo do char da coluna com o percorrer do ciclo for
            int column = piece.Location.Col + 'A' + (col - piece.Location.Col);

            possibleMoves.Add($"{(char)column}{piece.Location.Row + 1}"); // Representação visual UI coord.

        }

        // Calculo para movimentação para a esquerda
        for (int col = piece.Location.Col - 1; col >= 0; col--)
        {
            // Calculo do char da coluna com o percorrer do ciclo for
            int column = piece.Location.Col + 'A' - (piece.Location.Col - col);
            possibleMoves.Add($"{(char)column}{piece.Location.Row + 1}"); // Representação visual UI coord.
        }

        return possibleMoves;
    }

    public List<string> Get_VerticalMovement(Piece piece, List<string> possibleMoves, Piece[,] board)
    {
        Piece nextPiece;

        // Calculo de movimentação vertical da Torre
        
        for (int row = piece.Location.Row - 1; row >= 0; row--)
        {
            nextPiece = board[row, piece.Location.Col];

            if(nextPiece == null)
                possibleMoves.Add($"{(char)(piece.Location.Col + 'A')}{row + 1}");

            else if (nextPiece != null && piece.Team != nextPiece.Team)
            {
                possibleMoves.Add($"{(char)(piece.Location.Col + 'A')}{row + 1}");
                break;
            }
            else break;
        }

        for (int row = piece.Location.Row + 1; row < board.GetLength(0); row++)
        {
            nextPiece = board[row, piece.Location.Col];

            if (nextPiece == null)
                possibleMoves.Add($"{(char)(piece.Location.Col + 'A')}{row + 1}");

            else if (nextPiece != null && piece.Team != nextPiece.Team)
            {
                possibleMoves.Add($"{(char)(piece.Location.Col + 'A')}{row + 1}");
                break;
            }
            else break;
        }        

        return possibleMoves;
    }

    public List<string> Get_HorizontalMovement(Piece piece, List<string> possibleMoves, Piece[,] board)
    {
        int row = piece.Location.Row; // Representação da linha na board
        Piece nextPiece;

        // Calculo para movimentação para a direita
        for (int col = piece.Location.Col + 1; col < board.GetLength(1); col++)
        {
            nextPiece = board[row, col];
            // Calculo do char da coluna com o percorrer do ciclo for
            int column = piece.Location.Col + 'A' + (col - piece.Location.Col);

            /* Para evitar que o loop páre quando a proxima peça é inimiga e nao continuar a acrescentar
               possiveis movimentações. Se for nulo, adiciona às possiveis movimentações e continua no ciclo a verificar
               as proximas posições. */
            if(nextPiece == null)
                possibleMoves.Add($"{(char)column}{row + 1}");

            else if (nextPiece != null && piece.Team != nextPiece.Team)
            {
                possibleMoves.Add($"{(char)column}{row + 1}"); // Representação visual UI coord.
                break;
            }
            else break;
        }
        // Calculo para movimentação para a esquerda
        for (int col = piece.Location.Col - 1; col >= 0; col--)
        {
            nextPiece = board[row, col];
            // Calculo do char da coluna com o percorrer do ciclo for
            int column = piece.Location.Col + 'A' - (piece.Location.Col - col);

            /* Para evitar que o loop páre quando a proxima peça é inimiga e nao continuar a acrescentar
               possiveis movimentações. Se for nulo, adiciona às possiveis movimentações e continua no ciclo a verificar
               as proximas posições. */
            if (nextPiece == null)
                possibleMoves.Add($"{(char)column}{row + 1}");

            else if (nextPiece != null && piece.Team != nextPiece.Team)
            {
                possibleMoves.Add($"{(char)column}{row + 1}"); // Representação visual UI coord.
                break;
            }
            else break;
        }        

        return possibleMoves;
    }

}

