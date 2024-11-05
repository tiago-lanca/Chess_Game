using System.Data;
using System.IO.Compression;

class Board
{
    readonly static char[] letters = {'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H'};
    readonly static int[] numbers = {1, 2, 3, 4, 5, 6, 7, 8};
    static public Piece[,] board { get;set; } = new Piece[8,8];

    static public void PrintBoard(Piece[,] board){
        DisplayBoard(PieceTeam.White);
        DisplayBoard(PieceTeam.Black);

        Console.Write("  ");
        foreach(char letter in letters){
            Console.Write($" {letter}\t");
        }
        Console.WriteLine("");

        for(int line = 0; line < board.GetLength(0); line++){
            Console.Write($"{numbers[line]} ");
            for(int col = 0; col < board.GetLength(1); col++){
                if (board[line, col] != null)
                    Console.Write(board[line, col].PieceText + "\t");
                else Console.Write("   \t");
            }
            Console.WriteLine("");
        }
        Console.WriteLine("");
    }
    static public void DisplayBoard(PieceTeam team){
        int row, rowPawn;
        bool _isWhite = true;
        if(team == PieceTeam.White){
            row = 7;
            rowPawn = 6;
        }
        else {
            _isWhite = false;
            row = 0;
            rowPawn = 1;
        }

        board[row, 0] = new Piece(PieceType.Rook, row, 0, team, _isWhite ? "WR1" : "BR1");
        board[row, 7] = new Piece(PieceType.Rook, row, 7, team, _isWhite ? "WR2" : "BR2");

        board[row, 1] = new Piece(PieceType.Knight, row, 1, team, _isWhite ? "WH1" : "BH1");
        board[row, 6] = new Piece(PieceType.Knight, row, 6, team, _isWhite ? "WH2" : "BH2");

        board[row, 2] = new Piece(PieceType.Bishop, row, 2, team, _isWhite ? "WB1" : "BB1");
        board[row, 5] = new Piece(PieceType.Bishop, row, 5, team, _isWhite ? "WB2" : "BB2");

        board[row, 3] = new Piece(PieceType.Queen, row, 3, team, _isWhite ? "WQ1" : "BQ1");
        board[row, 4] = new Piece(PieceType.King, row, 4, team, _isWhite ? "WK2" : "BK2");

        for(int col = 0; col < 8; col++){
            board[rowPawn, col] = new Piece(PieceType.Pawn, row, col, team, _isWhite ? $"WP{col+1}" : $"BP{col+1}");
        }
    }
}