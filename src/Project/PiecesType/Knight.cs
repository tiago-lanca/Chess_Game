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
        List<string> possibleMoves_EnemyKingCheck = new List<string>();
        List<string> possibleMoves_EnemyKing = new List<string>();


        King enemyKing = FindEnemyKing(piece, board);
        King friendKing = FindFriendKing(piece, board);

        List<string> enemy_possibleMoves = new List<string>();

        if (friendKing.IsKing_InCheck(board))
        {
            GetAllMoves(piece, possibleMoves, board);

            if (IsValidMove(possibleMoves, input_ToPos))
            {
                // Movimenta a peça de posição para proceder à verificação se o Rei da equipa fica em check
                Testing_PiecePosition(piece, fromLocation, toLocation, board);

                friendKing.IsKing_InCheck(board);
                // Recebe todas as possiveis movimentações do inimigo e verifica se o rei da equipa fica check   
                if (friendKing.isCheck)
                {
                    Console.WriteLine("Movimento invalido (Rei Check).\n");
                    Undo_PiecePosition(piece, fromLocation, toLocation, board);
                }
                else
                {
                    Undo_PiecePosition(piece, fromLocation, toLocation, board);
                    MakePieceMove(piece, possibleMoves, fromLocation, toLocation, input_FromPos, input_ToPos, board);
                    friendKing.isCheck = false;
                }
            }
            else
                Print_PossibleMovements(possibleMoves);
        }

        else
        {

            GetAllMoves(piece, possibleMoves, board);

            if (IsValidMove(possibleMoves, input_ToPos))
            {
                Testing_PiecePosition(piece, fromLocation, toLocation, board);
                friendKing.IsKing_InCheck(board);

                if (friendKing.isCheck)
                {
                    Console.WriteLine("Movimento invalido (Rei Check).\n");
                    // Peça retoma à posição que estava antes
                    Undo_PiecePosition(piece, fromLocation, toLocation, board);
                }
                else
                {
                    Undo_PiecePosition(piece, fromLocation, toLocation, board);
                    MakePieceMove(piece, possibleMoves, fromLocation, toLocation, input_FromPos, input_ToPos, board);

                    // Verifica se o Rei adversário fica Check
                    GetAllMoves(piece, possibleMoves_EnemyKingCheck, board);
                    if (enemyKing.IsKing_InCheck(board))
                    {
                        enemyKing.isCheck = true;


                        // Verifica se rei inimigo está checkmate, senao está só check.
                        if (IsEnemyKing_Checkmate(enemyKing, EnemyKing_MovesAvoidingCheckmate(enemyKing, board), board))
                        {
                            Player player1 = PlayerList.players.Find(player => player.Name == Game.Player1.Name);
                            Player player2 = PlayerList.players.Find(player => player.Name == Game.Player2.Name);

                            //Game.FinishGame_Checkmate()
                            if (Game.Turn == false)
                            {
                                Console.WriteLine($"Checkmate. {player2.Name} venceu.\n");
                                player2.NumVictory++;
                                player1.NumLoss++;
                            }

                            else
                            {
                                Console.WriteLine($"Checkmate. {player1.Name} venceu.\n");
                                player1.NumVictory++;
                                player2.NumLoss++;
                            }
                            Game._IsGameInProgress = false;
                            Game._IsNewGame = true;
                        }
                        else
                            Console.WriteLine($"{enemyKing.PlaceHolder} em CHECK.\n");
                    }
                    
                }
            }
            else
                Print_PossibleMovements(possibleMoves);
        }
    }

    public override List<string> GetAllMoves(Piece piece, List<string> possibleMoves, Piece[,] board)
    {
        return Get_VerticalMoves(piece, possibleMoves, board)
            .Concat(Get_HorizontalMoves(piece, possibleMoves,board))
            .ToList();
    }

    public override List<string> GetMoves_ForCheckKing(Piece piece, List<string> possibleMoves, Piece[,] board)
    {
        return GetAllMoves(piece, possibleMoves, board);
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

