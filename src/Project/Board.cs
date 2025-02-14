using System.Data;
using System.IO.Compression;

class Board
{
    #region Variables

    readonly static char[] letters = {'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H'};
    readonly static int[] numbers = {1, 2, 3, 4, 5, 6, 7, 8};
    //public static bool _IsNewGame = true;
    #endregion

    #region Functions

    static public void SetNewBoard(Piece[,] board)
    {
        InitializePieces(PieceTeam.White, board);
        InitializePieces(PieceTeam.Black, board);
    }
    static public void PrintBoard(Piece[,] board)
    {
        if (Game._IsNewGame)
            SetNewBoard(board);

        Console.Write("   ");
        foreach (char letter in letters)
        {
            Console.Write($" {letter}   ");
        }
        Console.WriteLine("");

        for (int line = 0; line < board.GetLength(0); line++)
        {
            Console.Write($"{numbers[line]}  ");
            for (int col = 0; col < board.GetLength(1); col++)
            {
                if (board[line, col] != null)
                    Console.Write(board[line, col].PlaceHolder);
                else Console.Write("   ");

                Console.Write("  ");
            }
            Console.WriteLine("");
        }
        Console.WriteLine("");

    }
    
    static public void InitializePieces(PieceTeam team, Piece[,] board){
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

        board[row, 0] = new Rook(PieceType.Rook, new Location(row, 0), team, _isWhite ? "WR1" : "BR1");
        board[row, 7] = new Rook(PieceType.Rook, new Location(row, 7), team, _isWhite ? "WR2" : "BR2");

        board[row, 1] = new Knight(PieceType.Knight, new Location(row, 1), team, _isWhite ? "WH1" : "BH1");
        board[row, 6] = new Knight(PieceType.Knight, new Location(row, 6), team, _isWhite ? "WH2" : "BH2");
        board[row, 2] = new Bishop(PieceType.Bishop, new Location(row, 2), team, _isWhite ? "WB1" : "BB1");
        board[row, 5] = new Bishop(PieceType.Bishop, new Location(row, 5), team, _isWhite ? "WB2" : "BB2");

        board[row, 3] = new Queen(PieceType.Queen, new Location(row, 3), team, _isWhite ? "WQ1" : "BQ1");
        board[row, 4] = new King(PieceType.King, new Location(row, 4), team, _isWhite ? "WK1" : "BK1");

        for(int col = 0; col < 8; col++){
            board[rowPawn, col] = new Pawn(PieceType.Pawn, new Location(rowPawn,col), team, _isWhite ? $"WP{col + 1}" : $"BP{col + 1}");
        }
    }
    #endregion
}