using System.IO.Compression;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;

class Game()
{
    #region Variables
    static public Player? player1 { get; set; } = new Player();
    static public Player? player2 { get; set; } = new Player();
    public static string json_file;

    readonly static char[] letters = {'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H'};
    readonly static int[] numbers = {1, 2, 3, 4, 5, 6, 7, 8};
    static public Piece[,] board {get;set;}

    #endregion

    #region Functions
    static public void StartGame(string typegame, string playerone, string playertwo){        
        player1 = Player.players.Find(player => player.Name == playerone);
        player2 = Player.players.Find(player => player.Name == playertwo);
        board = Board.board;        

        Board.PrintBoard(Board.board);
        Board._IsNewGame = false;

        //Console.WriteLine(board[0, 1].Team);
        //Console.WriteLine(board[0, 1].Location);
        //Console.WriteLine(board[0, 1].PieceText);
    }

    static public void SaveGame(string namefile){
        List<List<Piece>> boardList = new List<List<Piece>>();

        for(int row = 0; row < 8; row++){
            List<Piece> rowList = new List<Piece>();
            for(int col = 0; col < 8; col++){                
                rowList.Add(board[row, col]);
            }
            boardList.Add(rowList);
        }

        var active_game = new {
            player1,
            player2,
            board = boardList
        };

        var options = new JsonSerializerOptions { WriteIndented = true }; // Identação no ficheiro json
        string jsonString = JsonSerializer.Serialize(active_game, options);
        File.WriteAllText($"{namefile}.json", jsonString);
        Console.WriteLine("Jogo gravado com sucesso.\n");
    }
    static public void LoadGame(string namefile){
        
        if(File.Exists($"{namefile}.json")){

            var gameJson = File.ReadAllText($"{namefile}.json");
            
            if(gameJson != string.Empty){
                var options = new JsonSerializerOptions { IncludeFields = true }; 
                var activeGame = JsonSerializer.Deserialize<SerializableGame>(gameJson, options);
                player1 = activeGame.player1;
                player2 = activeGame.player2;
                board = new Piece[8, 8];

                for (int row = 0; row < 8; row++){
                    for (int col = 0; col < 8; col++) {
                        if (activeGame.board[row][col] != null)
                            board[row, col] = activeGame.board[row][col];
                        else board[row, col] = null;
                    }
                }
                Console.WriteLine("Jogo lido com sucesso.\nExiste um jogo em curso.\n");
                Board.PrintBoard(board);
            }
        }
        else {
            Console.WriteLine("O ficheiro não existe.\n");
        }
    }

    static public void MovePiece(string fromPos, string toPos)
    {
        Piece piece;
        
        Location fromLocation = new Location{ 
            X = GetRowCoord(fromPos[1]),
            Y = GetColCoord(Char.ToUpper(fromPos[0]))
        };        

        Location toLocation = new Location{
            X = GetRowCoord(toPos[1]),
            Y = GetColCoord(Char.ToUpper(toPos[0]))
        };

        if (!IsInBounds(fromLocation.X, board.GetLength(1)) || !IsInBounds(fromLocation.Y, board.GetLength(0)))
        {
            Console.WriteLine("Jogada fora dos limites do tabuleiro.\n");
        }
        
        else
        {
            piece = board[fromLocation.X, fromLocation.Y];
            // Altera a peça de localização, e coloca null onde estava anteriormente
            board[toLocation.X, toLocation.Y] = piece;
            board[toLocation.X, toLocation.Y].Location.Y = toLocation.Y;
            board[toLocation.X, toLocation.Y].Location.X = toLocation.X;
            board[fromLocation.X, fromLocation.Y] = null;

            Console.WriteLine("Peça movimentada com sucesso.\n");
            Board.PrintBoard(board);
        }
    }

    public static int GetColCoord(char x)
    {
        return x - 'A';
    }

    public static int GetRowCoord(int y)
    {
        return (y - '1');
    }
    #endregion

    // Classe para entrar como objeto no JsonSerializer
    private class SerializableGame{
        public Player? player1 {get; set; }
        public Player? player2 {get; set;}
        public List<List<Piece>>? board {get; set;}
    };

    static public bool IsInBounds(int coord, int limit)
    {
        return ((coord+1 >= 0) && (coord+1 <= limit));
    }
}

