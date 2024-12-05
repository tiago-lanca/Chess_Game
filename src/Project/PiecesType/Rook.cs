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
        List<string> possibleMoves = new List<string>();
        List<string> enemyKing_possibleMoves = new List<string>(); ;
        List<string> enemy_possibleMoves = new List<string>();

        King enemyKing = FindEnemyKing(piece, board);
        King friendKing = FindFriendKing(piece, board);
        Rook rook = (Rook)piece;

        friendKing.IsKing_InCheck(board);

        if (friendKing.IsKing_InCheck(board))
        {
            GetAllMoves(piece, possibleMoves, board);
            if (IsValidMove(possibleMoves, input_ToPos))
            {                
                // Movimenta a peça de posição para proceder à verificação se o Rei da equipa fica em check
                Testing_PiecePosition(piece, fromLocation, toLocation, board);

                // Recebe todas as possiveis movimentações do inimigo e verifica se o rei da equipa fica check
                

                if (friendKing.IsKing_InCheck(board))
                {
                    Console.WriteLine("Movimento invalido (Rei Check).\n");
                    // Peça retoma à posição que estava antes
                    Undo_PiecePosition(piece, fromLocation, toLocation, board);
                }
                else
                {
                    // Faz o movimento da peça e coloca o rei da equipa Não Check
                    Undo_PiecePosition(piece, fromLocation, toLocation, board);
                    MakePieceMove(piece, possibleMoves, fromLocation, toLocation, input_FromPos, input_ToPos, board);
                    if (rook.FirstMove) rook.FirstMove = false;
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


                if (friendKing.IsKing_InCheck(board))
                {
                    Console.WriteLine("Movimento invalido (Rei Check).\n");
                    // Peça retoma à posição que estava antes
                    Undo_PiecePosition(piece, fromLocation, toLocation, board);
                }
                else
                {
                    Undo_PiecePosition(piece, fromLocation, toLocation, board);
                    MakePieceMove(piece, possibleMoves, fromLocation, toLocation, input_FromPos, input_ToPos, board);

                    if (rook.FirstMove) rook.FirstMove = false;

                    // Verifica se o Rei adversário fica Check
                    GetAllMoves(piece, possibleMoves, board);
                    if (enemyKing.IsKing_InCheck(board))
                    {

                        // Verifica se rei inimigo está checkmate, senao está só check.
                        if (IsEnemyKing_Checkmate(enemyKing, EnemyKing_MovesAvoidingCheckmate(enemyKing, board), board))
                            FinishGame_Complete();
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
        return Get_VerticalMovement((Rook)piece,possibleMoves, board)
            .Concat(Get_HorizontalMovement((Rook)piece, possibleMoves, board))
            .ToList();
    }

    public override List<string> GetMoves_ForCheckKing(Piece piece, List<string> possibleMoves, Piece[,] board)
    {
        for (int row = piece.Location.Row - 1; row >= 0; row--)
        {
            if (IsEnemyKing(piece, row, piece.Location.Col, board) || board[row, piece.Location.Col] is null)
                possibleMoves.Add($"{(char)(piece.Location.Col + 'A')}{row + 1}");
            else if (IsEnemyPiece(piece, row, piece.Location.Col, board))
            {
                possibleMoves.Add($"{(char)(piece.Location.Col + 'A')}{row + 1}");
                break;
            }
            else
                break;
        }

        for (int row = piece.Location.Row + 1; row < board.GetLength(0); row++)
        {
            if (IsEnemyKing(piece, row, piece.Location.Col, board) || board[row, piece.Location.Col] is null)
                possibleMoves.Add($"{(char)(piece.Location.Col + 'A')}{row + 1}");
            else if (IsEnemyPiece(piece, row, piece.Location.Col, board))
            {
                possibleMoves.Add($"{(char)(piece.Location.Col + 'A')}{row + 1}");
                break;
            }
            else
                break;
        }

        for (int col = piece.Location.Col + 1; col < board.GetLength(1); col++)
        {
            // Calculo do char da coluna com o percorrer do ciclo for
            if (IsEnemyKing(piece, piece.Location.Row, col, board) || board[piece.Location.Row, col] is null)
                possibleMoves.Add($"{(char)(col + 'A')}{piece.Location.Row + 1}");
            else if (IsEnemyPiece(piece, piece.Location.Row, col, board))
            {
                possibleMoves.Add($"{(char)(col + 'A')}{piece.Location.Row + 1}");
                break;
            }
            else
                break;

        }

        // Calculo para movimentação para a esquerda
        for (int col = piece.Location.Col - 1; col >= 0; col--)
        {
            // Calculo do char da coluna com o percorrer do ciclo for
            if (IsEnemyKing(piece, piece.Location.Row, col, board) || board[piece.Location.Row, col] is null)
                possibleMoves.Add($"{(char)(col + 'A')}{piece.Location.Row + 1}");
            else if (IsEnemyPiece(piece, piece.Location.Row, col, board))
            {
                possibleMoves.Add($"{(char)(col + 'A')}{piece.Location.Row + 1}");
                break;
            }
            else
                break;
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

