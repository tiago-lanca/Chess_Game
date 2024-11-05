using System.IO.Compression;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;

class Game()
{
    static public Player? player1 { get; set; } = new Player();
    static public Player? player2 { get; set; } = new Player();
    public static string json_file;

    readonly static char[] letters = {'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H'};
    readonly static int[] numbers = {1, 2, 3, 4, 5, 6, 7, 8};
    static public Piece[,] board {get;set;}
    
    static public void StartGame(string typegame, string playerone, string playertwo){        
        player1 = Player.players.Find(player => player.Name == playerone);
        player2 = Player.players.Find(player => player.Name == playertwo);
        //InitializePieces(PieceTeam.White);
        //InitializePieces(PieceTeam.Black);
        board = Board.board;
        Board.PrintBoard(Board.board);

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

        var active_gameOBJ = new {
            player1,
            player2,
            board = boardList
        };

        var options = new JsonSerializerOptions { WriteIndented = true }; // Identação no ficheiro json
        string jsonString = JsonSerializer.Serialize(active_gameOBJ, options);
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
            }
        }
        else {
            Console.WriteLine("O ficheiro não existe.\n");
        }
        Board.PrintBoard(board);
    }
    

    private class SerializableGame{
        public Player? player1 {get; set; }
        public Player? player2 {get; set;}
        public List<List<Piece>>? board {get; set;}
    };      
}

