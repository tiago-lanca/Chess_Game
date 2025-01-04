using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

public class King : Piece
{
    public bool FirstMove { get; set; } = true;
    public bool isCheck { get; set;} = false;
    public King() { }
    public King(PieceType pieceType, Location location, PieceTeam team, string placeholder)
        : base(pieceType, location, team, placeholder)
    {
    }

    public override King Clone()
    {
        return new King
        {
            FirstMove = FirstMove,
            isCheck = isCheck,
            Type = Type,
            Location = new Location(Location.Row, Location.Col),
            Team = Team,
            PlaceHolder = PlaceHolder,
        };
    }
    
    public override void MovePiece(Piece piece, Location fromLocation, Location toLocation, string input_FromPos, string input_ToPos, Piece[,] board)
    {
        List<string> possibleMoves = new List<string>();

        King king = piece as King; // Converte a piece em tipo King
        King enemyKing = FindEnemyKing(piece, board);
      
        Get_KingValidPossibleMoves((King)piece, possibleMoves, board);

        // Verifica se é possivel mover a peça
        if (IsValidMove(possibleMoves, input_ToPos))
        {
            if (RoqueMovement(king, toLocation))
            {
                Testing_PiecePosition(piece, fromLocation, toLocation, board);
                // Recebe todas as possiveis movimentações do inimigo e verifica se o rei da equipa fica check   
                if (king.IsKing_InCheck(board))
                {
                    //Console.WriteLine($"Movimento invalido (Rei {king.PlaceHolder} Check).\n");
                    Console.WriteLine("Movimento inválido.\n");
                    // Peça retoma à posição que estava antes
                    Undo_PiecePosition(piece, fromLocation, toLocation, board);
                }
                else
                {
                    // Faz o movimento da peça e coloca o rei da equipa  Check = false
                    Undo_PiecePosition(piece, fromLocation, toLocation, board);
                    Make_RoqueMovement(king, toLocation, board);
                    king.FirstMove = false;
                    Game.Nr_Rounds++;
                    //Mudança de turno, do jogador a jogar
                    Game.Turn = !Game.Turn;
                }                
            }

            else
            {
                // Movimenta a peça de posição para proceder à verificação se o Rei da equipa fica em check
                Testing_PiecePosition(piece, fromLocation, toLocation, board);

                // Recebe todas as possiveis movimentações do inimigo e verifica se este rei fica check   
                if (king.IsKing_InCheck(board))
                {
                    //Console.WriteLine($"Movimento invalido (Rei {king.PlaceHolder} Check).\n");
                    Console.WriteLine("Movimento inválido.\n");
                    // Peça retoma à posição que estava antes
                    Undo_PiecePosition(piece, fromLocation, toLocation, board);
                }

                else
                {
                    // Faz o movimento da peça e coloca o rei da equipa  Check = false
                    Undo_PiecePosition(piece, fromLocation, toLocation, board);
                    MakePieceMove(piece, possibleMoves, fromLocation, toLocation, input_FromPos, input_ToPos, board);
                    king.FirstMove = false;

                    GetAllMoves(piece, possibleMoves, board);
                    if (enemyKing.IsKing_InCheck(board))
                    {
                        // Verifica se rei inimigo está checkmate, senao está só check.
                        if (IsEnemyKing_Checkmate(enemyKing, EnemyKing_MovesAvoidingCheckmate(enemyKing, board), board))
                            FinishGame_Complete();
                        else
                            //Console.WriteLine($"{enemyKing.PlaceHolder} Rei em CHECK.\n");
                            Console.WriteLine("Check.\n");
                    }
                }
            }
        }

        else
        {
            // Se nao houver movimentações possiveis do rei
            //   E se nenhuma outra peça possa ficar à frente do rei
            if (king.isCheck && possibleMoves.Count == 0)
                FinishGame_Complete();            
            else
                Print_PossibleMovements(possibleMoves);
        }
    }

    public List<string> GetAllMoves(Piece piece, List<string> possibleMoves, Piece[,] board)
    {
        possibleMoves.Clear();

        return Get_HorizontalMoves((King)piece, possibleMoves, board)
            .Concat(Get_DiagonalMoves((King)piece, possibleMoves, board))
            .Concat(Get_VerticalMoves((King)piece, possibleMoves, board))
            .ToList();
    }

    public List<string> Get_KingValidPossibleMoves(King king, List<string> possibleMoves, Piece[,] board)
    {
        List<string> allEnemy_possibleMoves = new List<string>();

        GetAllEnemy_possibleMoves(allEnemy_possibleMoves, board);
        GetAllMoves(king, possibleMoves, board);

        foreach (string move in allEnemy_possibleMoves)
        {
            possibleMoves.Remove(move);
        }

        return possibleMoves;
    }

    public List<string> Get_VerticalMoves(King piece, List<string> possibleMoves, Piece[,] board)
    {
        int rowUp = piece.Location.Row - 1;
        int rowDown = piece.Location.Row + 1;
        int column = piece.Location.Col;
        Piece nextPiece;

        // Movimentação para cima
        if (rowUp >= 0)
        {
            nextPiece = board[rowUp, column];
            if (nextPiece == null || nextPiece.Team != piece.Team)
                possibleMoves.Add($"{(char)(column + 'A')}{rowUp + 1}");
        }

        // Movimentação para baixo
        if (rowDown < board.GetLength(1))
        {
            nextPiece = board[rowDown, column];
            if (nextPiece == null || nextPiece.Team != piece.Team)
                possibleMoves.Add($"{(char)(column + 'A')}{rowDown + 1}");
        }

        return possibleMoves;
    }

    public List<string> Get_HorizontalMoves(King piece, List<string> possibleMoves, Piece[,] board)
    {
        Get_HorizontalRight(piece, possibleMoves, board);
        Get_HorizontalLeft(piece, possibleMoves, board);

        return possibleMoves;
    }

    public List<string> Get_DiagonalMoves(King piece, List<string> possibleMoves, Piece[,] board)
    {
        int rowUp = piece.Location.Row - 1;
        int rowDown = piece.Location.Row + 1;
        int columnLeft = piece.Location.Col - 1;
        int columnRight = piece.Location.Col + 1;
        Piece nextPiece;

        // Movimentação para cima direita
        if (rowUp >= 0 && columnRight < board.GetLength(0))
        {
            nextPiece = board[rowUp, columnRight];
            if (nextPiece == null || nextPiece.Team != piece.Team)
                possibleMoves.Add($"{(char)(columnRight + 'A')}{rowUp + 1}");
        }

        // Movimentação para cima esquerda
        if (rowUp >= 0 && columnLeft >= 0)
        {
            nextPiece = board[rowUp, columnLeft];
            if (nextPiece == null || nextPiece.Team != piece.Team)
                possibleMoves.Add($"{(char)(columnLeft + 'A')}{rowUp + 1}");
        }

        // Movimentação para baixo direita
        if (rowDown < board.GetLength(1) && columnRight < board.GetLength(0))
        {
            nextPiece = board[rowDown, columnRight];
            if (nextPiece == null || nextPiece.Team != piece.Team)
                possibleMoves.Add($"{(char)(columnRight + 'A')}{rowDown + 1}");
        }

        // Movimentação para baixo esquerda
        if (rowDown < board.GetLength(1) && columnLeft >= 0)
        {
            nextPiece = board[rowDown, columnLeft];
            if (nextPiece == null || nextPiece.Team != piece.Team)
                possibleMoves.Add($"{(char)(columnLeft + 'A')}{rowDown + 1}");
        }

        return possibleMoves;
    }

    public List<string> Get_HorizontalRight(King piece, List<string> possibleMoves, Piece[,] board)
    {
        int columnRight = piece.Location.Col + 1;
        int row = piece.Location.Row;
        Piece nextPiece;
        Rook rightRook = null;

        // Movimentação para a direita se a proxima coluna da direita estiver dentro dos limites do tabuleiro
        if (columnRight < board.GetLength(1))
        {
            nextPiece = board[row, piece.Location.Col + 1];
            if (nextPiece == null || nextPiece.Team != piece.Team)
                possibleMoves.Add($"{(char)(columnRight + 'A')}{piece.Location.Row + 1}");


            // Verificação de possibilidade de efetuar ROQUE para a direita
            if (piece.Team == PieceTeam.White)
            {
                if (board[board.GetLength(0) - 1, board.GetLength(1) - 1] is Rook || board[board.GetLength(0) - 1, board.GetLength(1) - 1].Team is PieceTeam.White)
                    rightRook = (Rook)board[board.GetLength(0) - 1, board.GetLength(1) - 1];
            }
            else
            {
                if (board[0, board.GetLength(1) - 1] is Rook || board[0, board.GetLength(1) - 1].Team is PieceTeam.Black)
                    rightRook = (Rook)board[0, board.GetLength(1) - 1];
            }

            if (rightRook != null)
            {
                if (rightRook.FirstMove && piece.FirstMove)
                {
                    for (int col = piece.Location.Col + 1; col < board.GetLength(1); col++)
                    {
                        nextPiece = board[row, col];
                        if (nextPiece != null)
                        {
                            if (nextPiece.Type != PieceType.Rook) break;

                            else
                            {
                                possibleMoves.Add($"{(char)(columnRight + 'A' + 1)}{piece.Location.Row + 1}");
                                break;
                            }
                        }
                    }
                }
            }
        }

        return possibleMoves;
    }

    public List<string> Get_HorizontalLeft(King piece, List<string> possibleMoves, Piece[,] board)
    {
        int columnLeft = piece.Location.Col - 1;
        int row = piece.Location.Row;
        Piece nextPiece;
        Rook leftRook = null;

        // Movimentação para a esquerda e verifica se a coluna da esquerda está dentro dos limites do tabuleiro
        if (columnLeft >= 0)
        {
            nextPiece = board[row, columnLeft];
            if (nextPiece == null || nextPiece.Team != piece.Team)
                possibleMoves.Add($"{(char)(columnLeft + 'A')}{piece.Location.Row + 1}");

            // Verificação de possibilidade de efetuar ROQUE para esquerda
            if (piece.Team == PieceTeam.White)
            {
                if (board[board.GetLength(0) - 1, 0] is Rook || board[board.GetLength(0) - 1, 0].Team is PieceTeam.White)
                    leftRook = (Rook)board[board.GetLength(0) - 1, 0];
            }
            else
            {
                if (board[0, 0] is Rook || board[0, 0].Team is PieceTeam.Black)
                    leftRook = (Rook)board[0, 0];
            }

            if (leftRook != null)
            {
                if (leftRook.FirstMove && piece.FirstMove)
                {
                    for (int col = piece.Location.Col - 1; col >= 0; col--)
                    {
                        nextPiece = board[row, col];
                        if (nextPiece != null)
                        {
                            if (nextPiece.Type != PieceType.Rook) break;

                            else
                            {
                                possibleMoves.Add($"{(char)(columnLeft + 'A' - 1)}{piece.Location.Row + 1}");
                                break;
                            }
                        }
                    }
                }
            }
        }

        return possibleMoves;
    }

    public bool RoqueMovement(King king, Location toLocation)
    {
        // Verifica se o REI vai andar para a direita 2 casas ou 2 para esquerda pela coordenada que o jogador quer jogar
        return (king.Location.Col - toLocation.Col == -2) || (king.Location.Col - toLocation.Col == 2);
    }

    public void Make_RoqueMovement(King king, Location toLocation, Piece[,] board)
    {
        Rook leftRook;
        Rook rightRook;

        //WHITE TEAM para definir a torre da direita/esq.
        if (king.Team == PieceTeam.White)
        {
            leftRook = (Rook)board[board.GetLength(0) - 1, 0];
            rightRook = (Rook)board[board.GetLength(0) - 1, board.GetLength(1) - 1];
        }
        //BLACK TEAM para definir a torre da direita/esq.
        else
        {
            leftRook = (Rook)board[0, 0];
            rightRook = (Rook)board[0, board.GetLength(1) - 1];
        }

        if (rightRook is not null)
        {
            // Verifica se o REI andou para a direita 2 casas
            if (king.Location.Col - toLocation.Col == -2)
            {
                board[king.Location.Row, rightRook.Location.Col - 2] = rightRook;
                board[king.Location.Row, rightRook.Location.Col - 1] = king;
                board[rightRook.Location.Row, rightRook.Location.Col] = null; // Mete a posiçao da torre da direita nulo
                board[king.Location.Row, king.Location.Col] = null; // Mete a posiçao antiga do rei nulo

                Console.WriteLine("Roque efetuado.\n");
            }
        }

        if (leftRook is not null)
        {
            // Verifica se o REI andou para a esquerda 2 casas
            if (king.Location.Col - toLocation.Col == 2)
            {
                board[king.Location.Row, king.Location.Col - 1] = leftRook;
                board[king.Location.Row, king.Location.Col - 2] = king;
                board[leftRook.Location.Row, leftRook.Location.Col] = null; // Mete a posiçao da torre da esquerda nulo
                board[king.Location.Row, king.Location.Col] = null; // Mete a posiçao antiga do rei nulo

                Console.WriteLine("Roque efetuado.\n");
            }
        }        
    }

    public bool IsKing_InCheck(Piece[,] board)
    {
        List<string> enemy_possibleMoves = new List<string>();
        this.GetAllEnemy_possibleMoves(enemy_possibleMoves, board);

        if (enemy_possibleMoves.Any(move => move.Equals(Get_KingCoord(this), StringComparison.OrdinalIgnoreCase)))
        {
            this.isCheck = true;
            return true;
        }
        else
        {
            this.isCheck = false;
            return false;
        }
        
    }

    public List<string> GetAllEnemy_possibleMoves(List<string> allEnemy_possibleMoves, Piece[,] board)
    {
        allEnemy_possibleMoves.Clear();

        for(int row = 0; row < board.GetLength(0); row++)
        {
            for(int col = 0; col < board.GetLength(1); col++)
            {
                if (board[row, col] != null && board[row, col].Team != this.Team)
                {
                    board[row, col].GetMoves_ForCheckKing(board[row, col], allEnemy_possibleMoves, board);
                }
            }
        }        

        return allEnemy_possibleMoves;
    }
}


