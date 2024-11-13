using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

public class Rook : Piece
{
    public Rook() { }
    public Rook(PieceType type, Location location, PieceTeam team, string placeholder)
        : base(type, location, team, placeholder)
    {
    }

    public override void MovePiece(Location fromLocation, Location toLocation, string input_FromPos, string input_ToPos, PieceTeam team, Piece[,] board)
    {
        Piece piece = board[fromLocation.Row, fromLocation.Col]; // Iguala a piece à peça que o jogador quer jogar
        Piece nextPiece;
        List<string> possibleMoves = new List<string>();
        int col = fromLocation.Col;

        
        if (team == PieceTeam.White)
        {
            // Calculo de movimentação vertical e horizontal da Torre , equipa WHITE
            VerticalMovement(PieceTeam.White, fromLocation, possibleMoves, input_FromPos, board);
            HorizontalMovement(PieceTeam.White, fromLocation, possibleMoves, input_FromPos, board);
        }
        else
        {
            // Calculo de movimentação vertical e horizontal da Torre , equipa BLACK
            VerticalMovement(PieceTeam.Black, fromLocation, possibleMoves, input_FromPos, board);
            HorizontalMovement(PieceTeam.Black, fromLocation, possibleMoves, input_FromPos, board);
        }

        if (possibleMoves.Any(move => move.Equals(input_ToPos, StringComparison.OrdinalIgnoreCase)))
        {
            // Altera a peça de localização, e coloca null onde estava anteriormente
            board[toLocation.Row, toLocation.Col] = piece;
            board[toLocation.Row, toLocation.Col].Location.Col = toLocation.Col;
            board[toLocation.Row, toLocation.Col].Location.Row = toLocation.Row;
            board[fromLocation.Row, fromLocation.Col] = null;

            Board.PrintBoard(board);

            Console.WriteLine($"{piece.PlaceHolder} movimentada com sucesso.\n");

            //if (Math.Abs(fromLocation.Row - toLocation.Row) == 2) Verificar se andou 2 casas
        }
        else
        {
            Console.WriteLine("Movimento inválido.\n");
            Console.Write("Possiveis Movimentações: ");
            if (possibleMoves.Count == 0) Console.Write("N/A\n");
            else
            {
                foreach (var move in possibleMoves)
                    Console.Write($"{move} ");
                Console.WriteLine("\n");
            }
        }
    }

    public List<string> VerticalMovement(PieceTeam team, Location fromLocation, List<string> possibleMoves, string input_FromPos, Piece[,] board)
    {
        int col = fromLocation.Col;
        Piece nextPiece;

        // Calculo de movimentação vertical da Torre , equipa WHITE
        if (team == PieceTeam.White)
        {
            for (int row = fromLocation.Row - 1; row >= 0; row--)
            {
                nextPiece = board[row, col];
                if (nextPiece == null)
                {
                    possibleMoves.Add($"{char.ToUpper(input_FromPos[0])}{row + 1}");
                }
                else
                {
                    if (nextPiece.Team == PieceTeam.White) break;
                    else
                    {
                        Console.WriteLine($"Peça inimiga em frente. {nextPiece.PlaceHolder}\n");
                        possibleMoves.Add($"{char.ToUpper(input_FromPos[0])}{row + 1}");
                        break;
                    }
                }
            }

            for (int row = fromLocation.Row + 1; row < board.GetLength(0); row++)
            {
                nextPiece = board[row, col];
                if (nextPiece == null)
                {
                    possibleMoves.Add($"{char.ToUpper(input_FromPos[0])}{row + 1}");
                }
                else
                {
                    if (nextPiece.Team == PieceTeam.White) break;
                    else
                    {
                        Console.WriteLine($"Peça inimiga em frente. {nextPiece.PlaceHolder}\n");
                        possibleMoves.Add($"{char.ToUpper(input_FromPos[0])}{row + 1}");
                        break;
                    }
                }
            }
        }
        // Calculo de movimentação vertical da Torre , equipa BLACK
        else
        {
            // Calculo da movimentação para baixo
            for (int row = fromLocation.Row + 1; row < board.GetLength(0); row++)
            {
                nextPiece = board[row, col];
                if (nextPiece == null)
                {
                    possibleMoves.Add($"{char.ToUpper(input_FromPos[0])}{row + 1}");
                }
                else
                {
                    if (nextPiece.Team == PieceTeam.Black) break;
                    else
                    {
                        Console.WriteLine($"Peça inimiga em frente. {nextPiece.PlaceHolder}\n");
                        possibleMoves.Add($"{char.ToUpper(input_FromPos[0])}{row + 1}");
                        break;
                    }
                }
            }
            // Calculo da movimentação para cima
            for (int row = fromLocation.Row - 1; row >= 0; row--)
            {
                nextPiece = board[row, col];
                if (nextPiece == null)
                {
                    possibleMoves.Add($"{char.ToUpper(input_FromPos[0])}{row + 1}");
                }
                else
                {
                    if (nextPiece.Team == PieceTeam.Black) break;
                    else
                    {
                        Console.WriteLine($"Peça inimiga em frente. {nextPiece.PlaceHolder}\n");
                        possibleMoves.Add($"{char.ToUpper(input_FromPos[0])}{row + 1}");
                        break;
                    }
                }
            }
        }

        return possibleMoves;
    }

    public List<string> HorizontalMovement(PieceTeam team, Location fromLocation, List<string> possibleMoves, string input_FromPos, Piece[,] board)
    {
        int row = fromLocation.Row; // Representação da linha na board
        Piece nextPiece;

        if (team == PieceTeam.White)
        {
            // Calculo para movimentação para a direita
            for (int col = fromLocation.Col + 1; col < board.GetLength(1); col++)
            {
                nextPiece = board[row, col];
                // Calculo do char da coluna com o percorrer do ciclo for
                int column = input_FromPos[0] + (col - fromLocation.Col);

                if (nextPiece == null)
                {
                    possibleMoves.Add($"{char.ToUpper((char)column)}{row + 1}"); // Representação visual UI coord.
                }
                else
                {
                    if (nextPiece.Team == PieceTeam.White) break;
                    else
                    {
                        Console.WriteLine($"Peça inimiga em frente. {nextPiece.PlaceHolder}\n");
                        possibleMoves.Add($"{char.ToUpper((char)column)}{row + 1}");
                        break;
                    }
                }
            }
            // Calculo para movimentação para a esquerda
            for (int col = fromLocation.Col - 1; col >= 0; col--)
            {
                nextPiece = board[row, col];
                // Calculo do char da coluna com o percorrer do ciclo for
                int column = input_FromPos[0] - (fromLocation.Col - col);

                if (nextPiece == null)
                {
                    possibleMoves.Add($"{char.ToUpper((char)column)}{row + 1}"); // Representação visual UI coord.
                }
                else
                {
                    if (nextPiece.Team == PieceTeam.White) break;
                    else
                    {
                        Console.WriteLine($"Peça inimiga em frente. {nextPiece.PlaceHolder}\n");
                        possibleMoves.Add($"{char.ToUpper((char)column)}{row + 1}");
                        break;
                    }
                }
            }
        }
        else
        {
            // Calculo para movimentação para a direita
            for (int col = fromLocation.Col + 1; col < board.GetLength(1); col++)
            {
                nextPiece = board[row, col];
                // Calculo do char da coluna com o percorrer do ciclo for
                int column = input_FromPos[0] + (col - fromLocation.Col);

                if (nextPiece == null)
                {
                    possibleMoves.Add($"{char.ToUpper((char)column)}{row + 1}"); // Representação visual UI coord.
                }
                else
                {
                    if (nextPiece.Team == PieceTeam.Black) break;
                    else
                    {
                        Console.WriteLine($"Peça inimiga em frente. {nextPiece.PlaceHolder}\n");
                        possibleMoves.Add($"{char.ToUpper((char)column)}{row + 1}");
                        break;
                    }
                }
            }
            // Calculo para movimentação para a esquerda
            for (int col = fromLocation.Col - 1; col >= 0; col--)
            {
                nextPiece = board[row, col];
                // Calculo do char da coluna com o percorrer do ciclo for
                int column = input_FromPos[0] - (fromLocation.Col - col);

                if (nextPiece == null)
                {
                    possibleMoves.Add($"{char.ToUpper((char)column)}{row + 1}"); // Representação visual UI coord.
                }
                else
                {
                    if (nextPiece.Team == PieceTeam.Black) break;
                    else
                    {
                        Console.WriteLine($"Peça inimiga em frente. {nextPiece.PlaceHolder}\n");
                        possibleMoves.Add($"{char.ToUpper((char)column)}{row + 1}");
                        break;
                    }
                }
            }
        }

        return possibleMoves;
    }
}

