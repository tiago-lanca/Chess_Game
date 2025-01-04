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

    public override Rook Clone()
    {
        return new Rook
        {
            FirstMove = FirstMove,
            Type = Type,
            Location = new Location(Location.Row, Location.Col),
            Team = Team,
            PlaceHolder = PlaceHolder,
        };
    }
    public override void SpecialOperation(Piece piece, Location fromLocation, Location toLocation, string input_FromPos, string input_ToPos, Piece[,] board)
    {
        List<string> possibleMoves = new List<string>();

        King friendKing = FindFriendKing(piece, board);
        King enemyKing = FindEnemyKing(piece, board);
        Rook rook = (Rook)piece;

        if (SpecialOperation_Enable)
        {
            GetRook_SpecialMovements(rook, possibleMoves, board);

            if (IsValidMove(possibleMoves, input_ToPos))
            {
                Testing_PiecePosition(piece, fromLocation, toLocation, board);

                if (friendKing.IsKing_InCheck(board))
                {
                    //Console.WriteLine("Movimento invalido (Rei Check).\n");
                    Console.WriteLine("Movimento inválido.\n");

                    // Peça retoma à posição que estava antes
                    Undo_PiecePosition(piece, fromLocation, toLocation, board);
                }
                else
                {
                    Undo_PiecePosition(piece, fromLocation, toLocation, board);
                    MakePieceMove(piece, possibleMoves, fromLocation, toLocation, input_FromPos, input_ToPos, board);
                    Remove_NextPiece_SpecialOperation(fromLocation, toLocation, board);
                    
                    Console.WriteLine($"Torre {piece.PlaceHolder} capturou duas peças com sucesso.\n");
                    SpecialOperation_Enable = false;
                    if (rook.FirstMove) rook.FirstMove = false;

                    // Verifica se o Rei adversário fica Check
                    GetAllMoves(piece, possibleMoves, board);
                    if (enemyKing.IsKing_InCheck(board))
                    {
                        // Verifica se rei inimigo está checkmate, senao está só check.
                        if (IsEnemyKing_Checkmate(enemyKing, EnemyKing_MovesAvoidingCheckmate(enemyKing, board), board))
                            FinishGame_Complete();
                        else
                            //Console.WriteLine($"{enemyKing.PlaceHolder} em CHECK.\n");
                            Console.WriteLine("Check.\n");
                    }
                }
            }
            else
                Print_PossibleMovements(possibleMoves);
        }
        else
            Console.WriteLine("Movimento inválido.\n");
    }

    public override void MovePiece(Piece piece, Location fromLocation, Location toLocation, string input_FromPos, string input_ToPos, Piece[,] board)
    {
        List<string> possibleMoves = new List<string>();

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
                    //Console.WriteLine("Movimento invalido (Rei Check).\n");
                    Console.WriteLine("Movimento inválido.\n");

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
                    //Console.WriteLine("Movimento invalido (Rei Check).\n");
                    Console.WriteLine("Movimento inválido.\n");

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
                            //Console.WriteLine($"{enemyKing.PlaceHolder} em CHECK.\n");
                            Console.WriteLine("Check.\n");
                    }                    
                }
            }
            else
                Print_PossibleMovements(possibleMoves);
        }
    }
    public List<string> GetAllMoves(Piece piece, List<string> possibleMoves, Piece[,] board)
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

    public List<string> GetRook_SpecialMovements(Piece piece, List<string> possibleMoves, Piece[,] board)
    {
        // Cima
        if (piece.Location.Row - 1 >= 0)
        {
            if (board[piece.Location.Row - 1, piece.Location.Col] is not null && board[piece.Location.Row - 1, piece.Location.Col].Team != piece.Team)
            {
                for (int row = piece.Location.Row - 2; row >= 0; row--)
                {
                    if (board[row, piece.Location.Col] != null && board[row, piece.Location.Col].Team == piece.Team)
                        break;
                    else if (board[row, piece.Location.Col] != null && board[row, piece.Location.Col].Team != piece.Team)
                    {
                        possibleMoves.Add($"{(char)(piece.Location.Col + 'A')}{row + 1}");
                        break;
                    }
                }
            }
        }

        // Baixo
        if (piece.Location.Row + 1 < board.GetLength(0))
        {
            if (board[piece.Location.Row + 1, piece.Location.Col] is not null && board[piece.Location.Row + 1, piece.Location.Col].Team != piece.Team)
            {
                for (int row = piece.Location.Row + 2; row < board.GetLength(0); row++)
                {
                    if (board[row, piece.Location.Col] != null && board[row, piece.Location.Col].Team == piece.Team)
                        break;
                    else if (board[row, piece.Location.Col] != null && board[row, piece.Location.Col].Team != piece.Team)
                    {
                        possibleMoves.Add($"{(char)(piece.Location.Col + 'A')}{row + 1}");
                        break;
                    }
                }
            }
        }

        // Direita
        if (piece.Location.Col + 1 < board.GetLength(1))
        {
            if (board[piece.Location.Row, piece.Location.Col + 1] is not null && board[piece.Location.Row, piece.Location.Col + 1].Team != piece.Team)
            {
                for (int col = piece.Location.Col + 2; col < board.GetLength(0); col++)
                {
                    if (board[piece.Location.Row, col] != null && board[piece.Location.Row, col].Team == piece.Team)
                        break;
                    else if (board[piece.Location.Row, col] != null && board[piece.Location.Row, col].Team != piece.Team)
                    {
                        possibleMoves.Add($"{(char)(col + 'A')}{piece.Location.Row + 1}");
                        break;
                    }
                }
            }
        }

        // Esquerda
        if (piece.Location.Col - 1 >= 0)
        {
            if (board[piece.Location.Row, piece.Location.Col - 1] is not null && board[piece.Location.Row, piece.Location.Col - 1].Team != piece.Team)
            {
                for (int col = piece.Location.Col - 2; col >= 0; col--)
                {
                    if (board[piece.Location.Row, col] != null && board[piece.Location.Row, col].Team == piece.Team)
                        break;
                    else if (board[piece.Location.Row, col] != null && board[piece.Location.Row, col].Team != piece.Team)
                    {
                        possibleMoves.Add($"{(char)(col + 'A')}{piece.Location.Row + 1}");
                        break;
                    }
                }
            }
        }

        return possibleMoves;
    }

    public void Remove_NextPiece_SpecialOperation(Location fromLocation, Location toLocation, Piece[,] board)
    {
        if (fromLocation.Col == toLocation.Col)
        {
            if (fromLocation.Row > toLocation.Row) // Andou para cima
                board[fromLocation.Row - 1, fromLocation.Col] = null;
            else // Andou para baixo
                board[fromLocation.Row + 1, fromLocation.Col] = null;
        }
        else
        {
            if (fromLocation.Col > toLocation.Col) // Andou para esquerda
                board[fromLocation.Row, fromLocation.Col - 1] = null;
            else // Andou para direita
                board[fromLocation.Row, fromLocation.Col + 1] = null;
        }
    }

}

