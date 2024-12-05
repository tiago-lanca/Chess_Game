using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

public class Queen : Piece
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
        //List<string> enemyKing_possibleMoves = new List<string>();
        //List<string> enemy_possibleMoves = new List<string>();

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
                    Console.WriteLine($"Movimento invalido (Rei {friendKing.PlaceHolder} Check).\n");
                    // Peça retoma à posição que estava antes
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

        // Rei da equipa não está Check
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

                    // Verifica se o Rei adversário fica Check
                    GetAllMoves(piece, possibleMoves, board);                    

                    if (enemyKing.IsKing_InCheck(board))
                    {
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

    public override List<string> GetAllMoves(Piece piece, List<string> possibleMoves, Piece[,] board)
    {
        possibleMoves.Clear();
        // Fazer return de 3 métodos para receber todas as movimentações da peça
        return GetVerticalMoves(piece, possibleMoves, board)
            .Concat(GetHorizontalMoves(piece, possibleMoves, board))
            .Concat(GetDiagonalMoves(piece, possibleMoves, board))
            .ToList();
    }
    public override List<string> GetMoves_ForCheckKing(Piece piece, List<string> possibleMoves, Piece[,] board)
    {
        // VERTICAL MOVES As Empty Board
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

        // HORIZONTAL MOVES As Empty Board
        for (int col = piece.Location.Col + 1; col < board.GetLength(1); col++)
        {
            int column = piece.Location.Col + 'A' + (col - piece.Location.Col);
            if (IsEnemyKing(piece, piece.Location.Row, col, board) || board[piece.Location.Row, col] is null)
                possibleMoves.Add($"{(char)column}{piece.Location.Row + 1}");

            else if (IsEnemyPiece(piece, piece.Location.Row, col, board))
            {
                possibleMoves.Add($"{(char)column}{piece.Location.Row + 1}");
                break;
            }

            else
                break;
        }

        for (int col = piece.Location.Col - 1; col >= 0; col--)
        {
            int column = piece.Location.Col + 'A' - (piece.Location.Col - col);
            if (IsEnemyKing(piece, piece.Location.Row, col, board) || board[piece.Location.Row, col] is null)
                possibleMoves.Add($"{(char)column}{piece.Location.Row + 1}");

            else if (IsEnemyPiece(piece, piece.Location.Row, col, board))
            {
                possibleMoves.Add($"{(char)column}{piece.Location.Row + 1}");
                break;
            }

            else
                break;
        }

        // DIAGONAL MOVES Ignoring Enemy King Board
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

        // Diagonal cima esquerda
        nextColumn = piece.Location.Col;
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

        // Diagonal baixo direita
        nextColumn = piece.Location.Col;
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

        // Diagonal baixo esquerda
        nextColumn = piece.Location.Col;
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
    public List<string> GetVerticalMoves(Piece piece, List<string> possibleMoves, Piece[,] board)
    {
        Piece nextPiece;

        // Calculo de movimentação vertical para cima da Rainha
        for (int row = piece.Location.Row - 1; row >= 0; row--)
        {
            nextPiece = board[row, piece.Location.Col];

            /* Para evitar que o loop páre quando a proxima peça é inimiga e nao continuar a acrescentar
               possiveis movimentações. Se for nulo, adiciona às possiveis movimentações e continua no ciclo a verificar
               as proximas posições. */
            if (nextPiece == null)
                possibleMoves.Add($"{(char)(piece.Location.Col + 'A')}{row + 1}");

            else if (nextPiece != null && piece.Team != nextPiece.Team)
            {
                possibleMoves.Add($"{(char)(piece.Location.Col + 'A')}{row + 1}");
                break;
            }            

            else break;
        }

        // Calculo de movimentação vertical para baixo da Rainha
        for (int row = piece.Location.Row + 1; row < board.GetLength(0); row++)
        {
            nextPiece = board[row, piece.Location.Col];

            /* Para evitar que o loop páre quando a proxima peça é inimiga e nao continuar a acrescentar
               possiveis movimentações. Se for nulo, adiciona às possiveis movimentações e continua no ciclo a verificar
               as proximas posições. */

            // Verifica se a proxima peça é nulo
            if (nextPiece == null)
                possibleMoves.Add($"{(char)(piece.Location.Col + 'A')}{row + 1}");

            // Verifica se a proxima peça não é nula e de equipa inimiga, se for dá break para sair do loop for
            else if (nextPiece != null && piece.Team != nextPiece.Team)
            {
                possibleMoves.Add($"{(char)(piece.Location.Col + 'A')}{row + 1}");
                break;
            }         
            
            else break;
        }        

        return possibleMoves;
    }

    public static List<string> GetHorizontalMoves(Piece piece, List<string> possibleMoves, Piece[,] board)
    {
        Piece nextPiece;

        // Calculo para movimentação para a direita
        for (int col = piece.Location.Col + 1; col < board.GetLength(1); col++)
        {
            nextPiece = board[piece.Location.Row, col];
            // Calculo do char da coluna com o percorrer do ciclo for
            int column = piece.Location.Col + 'A' + (col - piece.Location.Col);

            /* Para evitar que o loop páre quando a proxima peça é inimiga e nao continuar a acrescentar
               possiveis movimentações. Se for nulo, adiciona às possiveis movimentações e continua no ciclo a verificar
               as proximas posições. */
            if (nextPiece == null)
                possibleMoves.Add($"{(char)column}{piece.Location.Row + 1}");

            else if (nextPiece != null && piece.Team != nextPiece.Team)
            {
                possibleMoves.Add($"{(char)column}{piece.Location.Row + 1}"); // Representação visual UI coord.
                break;
            }            

            else break;
        }

        // Calculo para movimentação para a esquerda
        for (int col = piece.Location.Col - 1; col >= 0; col--)
        {
            nextPiece = board[piece.Location.Row, col];
            // Calculo do char da coluna com o percorrer do ciclo for
            int column = piece.Location.Col + 'A' - (piece.Location.Col - col);

            /* Para evitar que o loop páre quando a proxima peça é inimiga e nao continuar a acrescentar
               possiveis movimentações. Se for nulo, adiciona às possiveis movimentações e continua no ciclo a verificar
               as proximas posições. */
            if (nextPiece == null)
                possibleMoves.Add($"{(char)column}{piece.Location.Row + 1}");

            else if (nextPiece != null && piece.Team != nextPiece.Team)
            {
                possibleMoves.Add($"{(char)column}{piece.Location.Row + 1}"); // Representação visual UI coord.
                break;
            }            

            else break;
        }

        return possibleMoves;
    }

    public static List<string> GetDiagonalMoves(Piece piece, List<string> possibleMoves, Piece[,] board)
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
                possibleMoves.Add($"{(char)nextColumn}{nextRow + 1}");
            
            else if (nextPiece != null && nextPiece.Team != piece.Team) // Verifica se a proxima peça não é nula ou se é da mesma equipa
            {  
                possibleMoves.Add($"{(char)nextColumn}{nextRow + 1}");
                break;
            }           

            else break;
        }

        nextColumn = piece.Location.Col + 'A'; // Reinicia o valor da proxima coluna para o valor inicial da coordenada[0]

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
                possibleMoves.Add($"{(char)nextColumn}{nextRow + 1}");

            else if (nextPiece != null && nextPiece.Team != piece.Team)
            {
                possibleMoves.Add($"{(char)nextColumn}{nextRow + 1}");
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
                possibleMoves.Add($"{(char)nextColumn}{nextRow + 1}");

            else if (nextPiece != null && nextPiece.Team != piece.Team)
            {
                possibleMoves.Add($"{(char)nextColumn}{nextRow + 1}");
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
                possibleMoves.Add($"{(char)nextColumn}{nextRow + 1}");

            else if (nextPiece != null && nextPiece.Team != piece.Team)
            {
                possibleMoves.Add($"{(char)nextColumn}{nextRow + 1}");
                break;
            }
            
            else break;
        }

        return possibleMoves;
    }
}

