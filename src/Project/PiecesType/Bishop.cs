using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

class Bishop : Piece
{
    public Bishop() { }
    public Bishop(PieceType pieceType, Location location, PieceTeam team, string placeholder)
        : base(pieceType, location, team, placeholder)
    {
    }

    public override void SpecialOperation(Piece piece, Location fromLocation, Location toLocation, string input_FromPos, string input_ToPos, Piece[,] board)
    {
        if (SpecialOperation_Enable)
        {
            int nr_Pawns = ExecuteBishop_Bomb(board);

            Console.WriteLine($"Bispo {piece.PlaceHolder} capturou {nr_Pawns} peões.\n");
            SpecialOperation_Enable = false;
        }
        else
            Console.WriteLine("Movimento inválido.\n");
    }

    public override void MovePiece(Piece piece, Location fromLocation, Location toLocation, string input_FromPos, string input_ToPos, Piece[,] board)
    {
        List<string> possibleMoves = new List<string>();

        King enemyKing = FindEnemyKing(piece, board);
        King friendKing = FindFriendKing(piece, board);

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
                    Undo_PiecePosition(piece, fromLocation, toLocation, board);
                }
                else
                {
                    // Faz o movimento da peça e coloca o rei da equipa  Check = false
                    Undo_PiecePosition(piece, fromLocation, toLocation, board);
                    MakePieceMove(piece, possibleMoves, fromLocation, toLocation, input_FromPos, input_ToPos, board);
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
                    GetAllMoves(piece, possibleMoves, board);
                    if (enemyKing.IsKing_InCheck(board))
                    {
                        enemyKing.isCheck = true;

                        // Verifica se rei inimigo está checkmate, senao está só check.
                        if (IsEnemyKing_Checkmate(enemyKing, EnemyKing_MovesAvoidingCheckmate(enemyKing, board), board))
                            FinishGame_Complete();
                        else
                            Console.WriteLine($"{enemyKing.PlaceHolder} Rei em CHECK.\n");
                    }
                }
            }
            else
                Print_PossibleMovements(possibleMoves);
        }
    }

    public List<string> GetAllMoves(Piece piece, List<string> possibleMoves, Piece[,] board)
    {
        possibleMoves.Clear();

        return Get_DiagonalMoves(piece, possibleMoves, board);
    }

    public override List<string> GetMoves_ForCheckKing(Piece piece, List<string> possibleMoves, Piece[,] board)
    {
        int nextColumn = piece.Location.Col;

        // Diagonal cima direita
        for (int nextRow = piece.Location.Row - 1; nextRow >= 0; nextRow--)
        {
            nextColumn += 1;
            if (nextColumn < board.GetLength(1))
            {
                if (IsEnemyKing(piece, nextRow, nextColumn, board) || board[nextRow, nextColumn] is null)
                    possibleMoves.Add($"{(char)(nextColumn + 'A')}{nextRow + 1}");

                else if (IsEnemyPiece(piece, nextRow, nextColumn, board))
                {
                    possibleMoves.Add($"{(char)(nextColumn + 'A')}{nextRow + 1}");
                    break;
                }

                else
                    break;
            }
            else
                break;
        }

        nextColumn = piece.Location.Col;  // Reinicia o valor da proxima coluna para o valor inicial da coordenada[0]

        // Diagonal cima esquerda
        for (int nextRow = piece.Location.Row - 1; nextRow >= 0; nextRow--)
        {
            nextColumn -= 1;
            if (nextColumn >= 0)
            {
                if (IsEnemyKing(piece, nextRow, nextColumn, board) || board[nextRow, nextColumn] is null)
                    possibleMoves.Add($"{(char)(nextColumn + 'A')}{nextRow + 1}");

                else if (IsEnemyPiece(piece, nextRow, nextColumn, board))
                {
                    possibleMoves.Add($"{(char)(nextColumn + 'A')}{nextRow + 1}");
                    break;
                }

                else
                    break;
            }
            else break;
        }

        nextColumn = piece.Location.Col;

        // Diagonal baixo direita
        for (int nextRow = piece.Location.Row + 1; nextRow < board.GetLength(1); nextRow++)
        {
            nextColumn += 1;
            if (nextColumn < board.GetLength(1))
            {
                if (IsEnemyKing(piece, nextRow, nextColumn, board) || board[nextRow, nextColumn] is null)
                    possibleMoves.Add($"{(char)(nextColumn + 'A')}{nextRow + 1}");

                else if (IsEnemyPiece(piece, nextRow, nextColumn, board))
                {
                    possibleMoves.Add($"{(char)(nextColumn + 'A')}{nextRow + 1}");
                    break;
                }

                else
                    break;
            }
            else
                break;
        }

        nextColumn = piece.Location.Col;

        // Diagonal baixo esquerda
        for (int nextRow = piece.Location.Row + 1; nextRow < board.GetLength(1); nextRow++)
        {
            nextColumn -= 1;
            if (nextColumn >= 0)
            {
                if (IsEnemyKing(piece, nextRow, nextColumn, board) || board[nextRow, nextColumn] is null)
                    possibleMoves.Add($"{(char)(nextColumn + 'A')}{nextRow + 1}");

                else if (IsEnemyPiece(piece, nextRow, nextColumn, board))
                {
                    possibleMoves.Add($"{(char)(nextColumn + 'A')}{nextRow + 1}");
                    break;
                }

                else
                    break;
            }
            else
                break;
        }

        return possibleMoves;
    }

    public static List<string> Get_DiagonalMoves(Piece piece, List<string> possibleMoves, Piece[,] board)
    {
        Piece nextPiece;
        int nextColumn = piece.Location.Col + 'A';

        // Diagonal cima direita
        for (int nextRow = piece.Location.Row - 1; nextRow >= 0; nextRow--)
        {            
            nextColumn += 1;            

            if ((nextColumn - 'A') >= board.GetLength(0)) break; // Verifica se a proxima coluna à direita está dentro dos limites do tabuleiro
            nextPiece = board[nextRow, nextColumn - 'A']; // Atribui à nextPiece uma "peça" na posiçao seguinte

            /* Para evitar que o loop páre quando a proxima peça é inimiga e nao continuar a acrescentar
               possiveis movimentações. Se for nulo, adiciona às possiveis movimentações e continua no ciclo a verificar
               as proximas posições. */
            if (nextPiece == null)
                possibleMoves.Add($"{char.ToUpper((char)nextColumn)}{nextRow + 1}");

            // Verifica se a proxima peça não é nula ou se é da mesma equipa
            else if (nextPiece != null && nextPiece.Team != piece.Team)
            {
                possibleMoves.Add($"{char.ToUpper((char)nextColumn)}{nextRow + 1}");
                break;
            }

            else break;
        }

        nextColumn = piece.Location.Col + 'A';  // Reinicia o valor da proxima coluna para o valor inicial da coordenada[0]
        
        // Diagonal cima esquerda
        for (int nextRow = piece.Location.Row - 1; nextRow >= 0; nextRow--)
        {
            nextColumn -= 1;

            if ((nextColumn - 'A') < 0) 
                break;

            nextPiece = board[nextRow, nextColumn - 'A'];

            /* Para evitar que o loop páre quando a proxima peça é inimiga e nao continuar a acrescentar
               possiveis movimentações. Se for nulo, adiciona às possiveis movimentações e continua no ciclo a verificar
               as proximas posições. */
            if (nextPiece == null)
                possibleMoves.Add($"{char.ToUpper((char)nextColumn)}{nextRow + 1}");

            else if (nextPiece != null && nextPiece.Team != piece.Team)
            {
                possibleMoves.Add($"{char.ToUpper((char)nextColumn)}{nextRow + 1}");
                break;
            }

            else break;            
        }

        nextColumn = piece.Location.Col + 'A';

        // Diagonal baixo direita
        for (int nextRow = piece.Location.Row + 1; nextRow < board.GetLength(1); nextRow++)
        {
            
            nextColumn += 1;
            if ((nextColumn - 'A') >= board.GetLength(0)) 
                break;

            nextPiece = board[nextRow, nextColumn - 'A'];

            /* Para evitar que o loop páre quando a proxima peça é inimiga e nao continuar a acrescentar
               possiveis movimentações. Se for nulo, adiciona às possiveis movimentações e continua no ciclo a verificar
               as proximas posições. */
            if (nextPiece == null)
                possibleMoves.Add($"{char.ToUpper((char)nextColumn)}{nextRow + 1}");

            else if (nextPiece != null && nextPiece.Team != piece.Team)
            {
                possibleMoves.Add($"{char.ToUpper((char)nextColumn)}{nextRow + 1}");
                break;
            }

            else break;
        }

        nextColumn = piece.Location.Col + 'A';

        // Diagonal baixo esquerda
        for (int nextRow = piece.Location.Row + 1; nextRow < board.GetLength(1); nextRow++)
        {
            
            nextColumn -= 1;
            if ((nextColumn - 'A') < 0) 
                break;

            nextPiece = board[nextRow, nextColumn - 'A'];

            /* Para evitar que o loop páre quando a proxima peça é inimiga e nao continuar a acrescentar
               possiveis movimentações. Se for nulo, adiciona às possiveis movimentações e continua no ciclo a verificar
               as proximas posições. */
            if (nextPiece == null)
                possibleMoves.Add($"{char.ToUpper((char)nextColumn)}{nextRow + 1}");

            else if (nextPiece != null && nextPiece.Team != piece.Team)
            {
                possibleMoves.Add($"{char.ToUpper((char)nextColumn)}{nextRow + 1}");
                break;
            }

            else break;            
        }

        return possibleMoves;    
    }

    public int ExecuteBishop_Bomb(Piece[,] board)
    {
        int row = this.Location.Row, col = this.Location.Col;
        int nr_Pawns = 0;

        // Cima
        if (row - 1 >= 0)
        {
            if (board[row - 1, col] != null && board[row - 1, col] is Pawn && board[row - 1, col].Team != this.Team)
            {
                board[row - 1, col] = null;
                nr_Pawns++;
            }
        }

        // Baixo
        if (row + 1 < board.GetLength(0))
        {
            if (board[row + 1, col] != null && board[row + 1, col] is Pawn && board[row + 1, col].Team != this.Team)
            {
                board[row + 1, col] = null;
                nr_Pawns++;
            }
        }

        // Direita
        if (col + 1 < board.GetLength(1))
        {
            if (board[row, col + 1] != null && board[row, col + 1] is Pawn && board[row, col + 1].Team != this.Team)
            {
                board[row, col + 1] = null;
                nr_Pawns++;
            }
        }

        // Esquerda
        if (col - 1 >= 0)
        {
            if (board[row, col - 1] != null && board[row, col - 1] is Pawn && board[row, col - 1].Team != this.Team)
            {
                board[row, col - 1] = null;
                nr_Pawns++;
            }
        }

        // Cima Direita
        if (row - 1 >= 0 && col + 1 < board.GetLength(1))
        {
            if (board[row - 1, col + 1] != null && board[row - 1, col + 1] is Pawn && board[row - 1, col + 1].Team != this.Team)
            {
                board[row - 1, col + 1] = null;
                nr_Pawns++;
            }
        }

        // Cima Esquerda
        if (row - 1 >= 0 && col - 1 >= 0)
        {
            if (board[row - 1, col - 1] != null && board[row - 1, col - 1] is Pawn && board[row - 1, col - 1].Team != this.Team)
            {
                board[row - 1, col - 1] = null;
                nr_Pawns++;
            }
        }

        // Baixo Esquerda
        if (row + 1 < board.GetLength(0) && col - 1 >= 0)
        {
            if (board[row + 1, col - 1] != null && board[row + 1, col - 1] is Pawn && board[row + 1, col - 1].Team != this.Team)
            {
                board[row + 1, col - 1] = null;
                nr_Pawns++;
            }
        }

        // Baixo Esquerda
        if (row + 1 < board.GetLength(0) && col + 1 < board.GetLength(1))
        {
            if (board[row + 1, col + 1] != null && board[row + 1, col + 1] is Pawn && board[row + 1, col + 1].Team != this.Team)
            {
                board[row + 1, col + 1] = null;
                nr_Pawns++;
            }
        }

        return nr_Pawns;
    }
}

