using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

public class King : Piece
{
    public bool FirstMove { get; set; } = true;
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
        King king = piece as King; // Converte a piece em tipo King

        Get_VerticalMoves((King)piece, fromLocation, possibleMoves, input_FromPos, board);
        Get_HorizontalMoves((King)piece, fromLocation, possibleMoves, input_FromPos, board);
        Get_DiagonalMoves((King)piece, fromLocation, possibleMoves, input_FromPos, board);

        // Verifica se é possivel mover a peça
        if (IsValidMove(possibleMoves, input_ToPos))
        {
            if (!Check_RoqueMovement(king, fromLocation, toLocation, board))
                MakePieceMove(piece, possibleMoves, fromLocation, toLocation, input_FromPos, input_ToPos, board);
        }
        else 
            Print_PossibleMovements(possibleMoves, input_ToPos);
    }

    public List<string> Get_VerticalMoves(King piece, Location fromLocation, List<string> possibleMoves, string input_FromPos, Piece[,] board)
    {
        int rowUp = fromLocation.Row - 1;
        int rowDown = fromLocation.Row + 1;
        int column = char.ToUpper(input_FromPos[0]) - 'A';
        Piece nextPiece;

        // Movimentação para cima
        if (rowUp >= 0)
        {
            nextPiece = board[rowUp, column];
            if (nextPiece == null || nextPiece.Team != piece.Team)
                possibleMoves.Add($"{char.ToUpper(input_FromPos[0])}{rowUp + 1}");
        }

        // Movimentação para baixo
        if (rowDown < board.GetLength(1))
        {
            nextPiece = board[rowDown, column];
            if (nextPiece == null || nextPiece.Team != piece.Team)
                possibleMoves.Add($"{char.ToUpper(input_FromPos[0])}{rowDown + 1}");
        }

        return possibleMoves;
    }

    public List<string> Get_HorizontalMoves(King piece, Location fromLocation, List<string> possibleMoves, string input_FromPos, Piece[,] board)
    {
        Get_HorizontalRight(piece, possibleMoves, fromLocation, input_FromPos, board);
        Get_HorizontalLeft(piece, possibleMoves, fromLocation, input_FromPos, board);

        return possibleMoves;
    }

    public List<string> Get_DiagonalMoves(King piece, Location fromLocation, List<string> possibleMoves, string input_FromPos, Piece[,] board)
    {
        int rowUp = fromLocation.Row - 1;
        int rowDown = fromLocation.Row + 1;
        int columnLeft = char.ToUpper(input_FromPos[0]) - 'A' - 1;
        int columnRight = char.ToUpper(input_FromPos[0]) - 'A' + 1;
        Piece nextPiece;

        // Movimentação para cima direita
        if (rowUp >= 0 && columnRight < board.GetLength(0))
        {
            nextPiece = board[rowUp, columnRight];
            if (nextPiece == null || nextPiece.Team != piece.Team)
                possibleMoves.Add($"{char.ToUpper((char)(input_FromPos[0] + 1))}{rowUp + 1}");
        }

        // Movimentação para cima esquerda
        if (rowUp >= 0 && columnLeft >= 0)
        {
            nextPiece = board[rowUp, columnLeft];
            if (nextPiece == null || nextPiece.Team != piece.Team)
                possibleMoves.Add($"{char.ToUpper((char)(input_FromPos[0] - 1))}{rowUp + 1}");
        }

        // Movimentação para baixo direita
        if (rowDown < board.GetLength(1) && columnRight < board.GetLength(0))
        {
            nextPiece = board[rowDown, columnRight];
            if (nextPiece == null || nextPiece.Team != piece.Team)
                possibleMoves.Add($"{char.ToUpper((char)(input_FromPos[0] + 1))}{rowDown + 1}");
        }

        // Movimentação para baixo esquerda
        if (rowDown < board.GetLength(1) && columnLeft >= 0)
        {
            nextPiece = board[rowDown, columnLeft];
            if (nextPiece == null || nextPiece.Team != piece.Team)
                possibleMoves.Add($"{char.ToUpper((char)(input_FromPos[0] - 1))}{rowDown + 1}");
        }

        return possibleMoves;
    }

    public List<string> Get_HorizontalRight(King piece, List<string> possibleMoves, Location fromLocation, string input_FromPos, Piece[,] board)
    {
        int columnRight = char.ToUpper(input_FromPos[0]) - 'A' + 1;
        int row = fromLocation.Row;
        Piece nextPiece;
        Rook rightRook;

        // Movimentação para a direita
        if (columnRight < board.GetLength(0))
        {
            nextPiece = board[row, columnRight];
            if (nextPiece == null || nextPiece.Team != piece.Team)
                possibleMoves.Add($"{char.ToUpper((char)(input_FromPos[0] + 1))}{fromLocation.Row + 1}");

            // Verificação de possibilidade de efetuar ROQUE
            if(piece.Team == PieceTeam.White)
                rightRook = (Rook)board[board.GetLength(0) - 1, board.GetLength(1) - 1];
            else
                rightRook = (Rook)board[0, board.GetLength(1) - 1];

            if (rightRook != null) 
            {
                if (rightRook.FirstMove && piece.FirstMove)
                {
                    for (int col = fromLocation.Col + 1; col < board.GetLength(1); col++)
                    {
                        nextPiece = board[row, col];
                        if (nextPiece != null)
                        {
                            if (nextPiece.Type != PieceType.Rook) break;

                            else
                            {
                                possibleMoves.Add($"{char.ToUpper((char)(input_FromPos[0] + 2))}{fromLocation.Row + 1}");
                                break;
                            }
                        }
                    }
                }
            }
        }

        return possibleMoves;
    }

    public List<string> Get_HorizontalLeft(King piece, List<string> possibleMoves, Location fromLocation, string input_FromPos, Piece[,] board)
    {
        int columnLeft = char.ToUpper(input_FromPos[0]) - 'A' - 1;
        int row = fromLocation.Row;
        Piece nextPiece;
        Rook leftRook;

        // Movimentação para a esquerda
        if (columnLeft >= 0)
        {
            nextPiece = board[row, columnLeft];
            if (nextPiece == null || nextPiece.Team != piece.Team)
                possibleMoves.Add($"{char.ToUpper((char)(input_FromPos[0] - 1))}{fromLocation.Row + 1}");

            // Verificação de possibilidade de efetuar ROQUE
            if (piece.Team == PieceTeam.White)
                leftRook = (Rook)board[board.GetLength(0) - 1, 0]; // WHITE
            else
                leftRook = (Rook)board[0, 0]; // BLACK

            if (leftRook != null)
            {
                if (leftRook.FirstMove && piece.FirstMove)
                {
                    for (int col = fromLocation.Col - 1; col >= 0; col--)
                    {
                        nextPiece = board[row, col];
                        if (nextPiece != null)
                        {
                            if (nextPiece.Type != PieceType.Rook) break;

                            else
                            {
                                possibleMoves.Add($"{char.ToUpper((char)(input_FromPos[0] - 3))}{fromLocation.Row + 1}");
                                break;
                            }
                        }
                    }
                }
            }
        }

        return possibleMoves;
    }

    public bool Check_RoqueMovement(King king, Location fromLocation, Location toLocation, Piece[,] board)
    {
        Rook leftRook;
        Rook rightRook;

        king.FirstMove = false;

        //WHITE TEAM
        if (king.Team == PieceTeam.White)
        {
            leftRook = (Rook)board[board.GetLength(0) - 1, 0];
            rightRook = (Rook)board[board.GetLength(0) - 1, board.GetLength(1) - 1];
        }
        else
        {
            leftRook = (Rook)board[0, 0];
            rightRook = (Rook)board[0, board.GetLength(1) - 1];
        }

        // Verifica se o REI andou para a direita 2 casas
        if (fromLocation.Col - toLocation.Col == -2)
        {
            board[fromLocation.Row, rightRook.Location.Col - 2] = rightRook;
            board[fromLocation.Row, rightRook.Location.Col - 1] = king;
            board[rightRook.Location.Row, rightRook.Location.Col] = null; // Mete a posiçao da torre da direita nulo
            board[king.Location.Row, king.Location.Col] = null; // Mete a posiçao antiga do rei nulo
            Board.PrintBoard(board);
            Console.WriteLine("Roque efetuado.\n");
            return true;
        }

        // Verifica se o REI andou para a esquerda 3 casas
        else if (fromLocation.Col - toLocation.Col == 3)
        {
            board[fromLocation.Row, fromLocation.Col - 2] = leftRook;
            board[fromLocation.Row, fromLocation.Col - 3] = king;
            board[leftRook.Location.Row, leftRook.Location.Col] = null; // Mete a posiçao da torre da esquerda nulo
            board[king.Location.Row, king.Location.Col] = null; // Mete a posiçao antiga do rei nulo
            Board.PrintBoard(board);
            Console.WriteLine("Roque efetuado.\n");
            return true;
        }

        else return false;
    }
}


