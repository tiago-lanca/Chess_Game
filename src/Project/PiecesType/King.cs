﻿using System;
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

    // Verificar se alguma peça da mesma equipa consegue o proteger:
    // Verificar as posições à volta dele e ver se existe alguma peça da mesma equipa se consegue
    // movimentar para frente dele.
    public override void MovePiece(Piece piece, Location fromLocation, Location toLocation, string input_FromPos, string input_ToPos, Piece[,] board)
    {
        List<string> possibleMoves = new List<string>();
        List<string> possibleMovesKingCheck = new List<string>();

        // Lista para guardar todas as movimentações adversarias para determinar se o rei fica Check
        List<string> allEnemy_possibleMoves = new List<string>();

        King king = piece as King; // Converte a piece em tipo King
        King enemyKing = FindEnemyKing(piece, board);

      
        Get_KingValidPossibleMoves((King)piece, possibleMoves, board);

        // Verifica se é possivel mover a peça
        if (IsValidMove(possibleMoves, input_ToPos))
        {
            if (RoqueMovement(king, toLocation))
                Make_RoqueMovement(king, fromLocation, toLocation, board);

            else
            {
                MakePieceMove(piece, possibleMoves, fromLocation, toLocation, input_FromPos, input_ToPos, board);                
                if (IsEnemyKing_InCheck(piece, possibleMovesKingCheck, board))
                    Console.WriteLine($"{enemyKing.PlaceHolder} Rei em CHECK.\n");
            }

            king.FirstMove = false;
        }
        else
        {
            // Se nao houver movimentações possiveis do rei
            //   E se nenhuma outra peça possa ficar à frente do rei
            if (king.isCheck && possibleMoves.Count == 0)
            {
                if (Game.Turn == false)
                {
                    Console.WriteLine($"Checkmate. {Game.Player1.Name} venceu.\n");
                    Game.Player1.NumVictory++;
                    Game.Player2.NumLoss++;                    
                }

                else
                {
                    Console.WriteLine($"Checkmate. {Game.Player2.Name} venceu.\n");
                    Game.Player2.NumVictory++;
                    Game.Player1.NumLoss++;
                }
                Game._IsGameInProgress = false;
                Game._IsNewGame = true;

            }
            else
                Print_PossibleMovements(possibleMoves, input_ToPos);
        }
    }

    public override List<string> GetAllMoves(Piece piece, List<string> possibleMoves, Piece[,] board)
    {
        return Get_HorizontalMoves((King)piece, possibleMoves, board)
            .Concat(Get_DiagonalMoves((King)piece, possibleMoves, board))
            .Concat(Get_VerticalMoves((King)piece, possibleMoves, board))
            .ToList();
    }

    public List<string> Get_KingValidPossibleMoves(Piece piece, List<string> possibleMoves, Piece[,] board)
    {
        List<string> allEnemy_possibleMoves = new List<string>();

        GetAllEnemy_possibleMoves(piece, allEnemy_possibleMoves, board);
        GetAllMoves(piece, possibleMoves, board);

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
        Rook rightRook;

        // Movimentação para a direita
        if (columnRight < board.GetLength(1))
        {
            nextPiece = board[row, piece.Location.Col + 1];
            if (nextPiece == null || nextPiece.Team != piece.Team)
                possibleMoves.Add($"{(char)(columnRight + 'A')}{piece.Location.Row + 1}");
        }
        else
        {

            // Verificação de possibilidade de efetuar ROQUE para a direita
            if (piece.Team == PieceTeam.White)
                rightRook = (Rook)board[board.GetLength(0) - 1, board.GetLength(1) - 1];
            else
                rightRook = (Rook)board[0, board.GetLength(1) - 1];

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
        Rook leftRook;

        // Movimentação para a esquerda
        if (columnLeft >= 0)
        {
            nextPiece = board[row, columnLeft];
            if (nextPiece == null || nextPiece.Team != piece.Team)
                possibleMoves.Add($"{(char)(columnLeft + 'A')}{piece.Location.Row + 1}");

            // Verificação de possibilidade de efetuar ROQUE para esquerda
            if (piece.Team == PieceTeam.White)
                leftRook = (Rook)board[board.GetLength(0) - 1, 0]; // WHITE
            else
                leftRook = (Rook)board[0, 0]; // BLACK

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
        // Verifica se o REI vai andar para a direita 2 casas ou 3 para esquerda pela coordenada que o jogador quer jogar
        return (king.Location.Col - toLocation.Col == -2) || (king.Location.Col - toLocation.Col == 2);
    }

    public void Make_RoqueMovement(King king, Location fromLocation, Location toLocation, Piece[,] board)
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

        // Verifica se o REI andou para a direita 2 casas
        if (king.Location.Col - toLocation.Col == -2)
        {
            board[king.Location.Row, rightRook.Location.Col - 2] = rightRook;
            board[king.Location.Row, rightRook.Location.Col - 1] = king;
            board[rightRook.Location.Row, rightRook.Location.Col] = null; // Mete a posiçao da torre da direita nulo
            board[king.Location.Row, king.Location.Col] = null; // Mete a posiçao antiga do rei nulo
        }

        // Verifica se o REI andou para a esquerda 2 casas
        else if (king.Location.Col - toLocation.Col == 2)
        {
            board[king.Location.Row, king.Location.Col - 1] = leftRook;
            board[king.Location.Row, king.Location.Col - 2] = king;
            board[leftRook.Location.Row, leftRook.Location.Col] = null; // Mete a posiçao da torre da esquerda nulo
            board[king.Location.Row, king.Location.Col] = null; // Mete a posiçao antiga do rei nulo
           
        }

        Console.WriteLine("Roque efetuado.\n");
    }

    public bool IsKing_Check(King king, List<string> possibleMoves)
    {
        return possibleMoves.Any(move => move.Equals(Get_KingCoord(king), StringComparison.OrdinalIgnoreCase));          
       
    }

    public List<string> GetAllEnemy_possibleMoves(Piece piece, List<string> allEnemy_possibleMoves, Piece[,] board)
    {
        for(int row = 0; row < board.GetLength(0); row++)
        {
            for(int col = 0; col < board.GetLength(1); col++)
            {
                if (board[row, col] != null && board[row, col].Team != piece.Team)
                {
                    board[row, col].GetMoves_ForKingCheck(board[row, col], allEnemy_possibleMoves, board);
                }
            }
        }        

        return allEnemy_possibleMoves;
    }
}


