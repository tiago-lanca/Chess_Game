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

    // Calcular movimentação nas diagonais, rowup + columnRight em ciclo for, rowup + columnLeft em for,
    // rowdown+columnRight em for, rowdown + columnLeft em for

    public override void MovePiece(Piece piece, Location fromLocation, Location toLocation, string input_FromPos, string input_ToPos, Piece[,] board)
    {
        List<string> possibleMoves = new List<string>();
        List<string> possibleMoves_EnemyKingCheck = new List<string>();
        List<string> possibleMoves_EnemyKing = new List<string>();

        King enemyKing = FindEnemyKing(piece, board);
        King friendKing = FindFriendKing(piece, board);

        if (friendKing.isCheck)
        {
            friendKing.Get_KingValidPossibleMoves(friendKing, possibleMoves, board);
            Print_PossibleMovements(possibleMoves, input_ToPos);
        }
        else {

            GetAllMoves(piece, possibleMoves, board);

            if (IsValidMove(possibleMoves, input_ToPos))
            {
                MakePieceMove(piece, possibleMoves, fromLocation, toLocation, input_FromPos, input_ToPos, board);

                // Verifica se o Rei adversário fica Check
                Get_DiagonalMoves(piece, possibleMoves_EnemyKingCheck, board);
                if (IsEnemyKing_InCheck(piece, possibleMoves_EnemyKingCheck, board))
                    enemyKing.isCheck = true;                    
                
                // Verifica se rei inimigo está checkmate, senao está só check.
                if (IsEnemyKing_Checkmate(enemyKing, enemyKing.Get_KingValidPossibleMoves(enemyKing, possibleMoves_EnemyKing, board))){
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
                else Console.WriteLine($"{enemyKing.PlaceHolder} em CHECK.\n");

            }
            else
                Print_PossibleMovements(possibleMoves, input_ToPos);
        }
    }

    public override List<string> GetAllMoves(Piece piece, List<string> possibleMoves, Piece[,] board)
    {
        return Get_DiagonalMoves(piece, possibleMoves, board);
    }

    public override List<string> GetMoves_ForKingCheck(Piece piece, List<string> possibleMoves, Piece[,] board)
    {
        int nextColumn = piece.Location.Col + 'A';

        // Diagonal cima direita
        for (int nextRow = piece.Location.Row - 1; nextRow >= 0; nextRow--)
        {
            nextColumn += 1;

            if ((nextColumn - 'A') >= board.GetLength(0)) break; // Verifica se a proxima coluna à direita está dentro dos limites do tabuleiro
           
            else possibleMoves.Add($"{char.ToUpper((char)nextColumn)}{nextRow + 1}");
                
        }

        nextColumn = piece.Location.Col + 'A';  // Reinicia o valor da proxima coluna para o valor inicial da coordenada[0]

        // Diagonal cima esquerda
        for (int nextRow = piece.Location.Row - 1; nextRow >= 0; nextRow--)
        {
            nextColumn -= 1;

            if ((nextColumn - 'A') < 0)
                break;
            
            else possibleMoves.Add($"{char.ToUpper((char)nextColumn)}{nextRow + 1}");
        }

        nextColumn = piece.Location.Col + 'A';

        // Diagonal baixo direita
        for (int nextRow = piece.Location.Row + 1; nextRow < board.GetLength(1); nextRow++)
        {

            nextColumn += 1;
            if ((nextColumn - 'A') >= board.GetLength(0))
                break;

            
            else possibleMoves.Add($"{char.ToUpper((char)nextColumn)}{nextRow + 1}");            
        }

        nextColumn = piece.Location.Col + 'A';

        // Diagonal baixo esquerda
        for (int nextRow = piece.Location.Row + 1; nextRow < board.GetLength(1); nextRow++)
        {

            nextColumn -= 1;
            if ((nextColumn - 'A') < 0)
                break;

            
            else possibleMoves.Add($"{char.ToUpper((char)nextColumn)}{nextRow + 1}");           
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

   

}

