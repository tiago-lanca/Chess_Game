using System.IO.Compression;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;

class Game()
{
    #region Variables
    static public Player? player1 { get; set; }
    static public Player? player2 { get; set; }
    public bool _GameInProgress = false;
    private static string? json_file;

    readonly static char[] letters = {'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H'};
    readonly static int[] numbers = {1, 2, 3, 4, 5, 6, 7, 8};
    static public Piece[,] board { get;set; }

    #endregion

    #region Functions
    static public void StartGame(string typegame, string playerone, string playertwo){      
        
        player1 = Player.players.Find(player => player.Name == playerone);
        player2 = Player.players.Find(player => player.Name == playertwo);
        board = Board.board;

        switch (typegame)
        {
            case "novo":
                Board.PrintBoard(Board.board);
                Board._IsNewGame = false;
                break;

            case "continuacao":
                //LoadGame ??
                break;

            default:
                Console.WriteLine("Instrução inválida");
                break;
        }
    }

    static public void SaveGame(string namefile){
        List<List<Piece>> boardList = new List<List<Piece>>();
        if (board != null)
        {
            for (int row = 0; row < 8; row++)
            {
                List<Piece> rowList = new List<Piece>();
                for (int col = 0; col < 8; col++)
                {
                    rowList.Add(board[row, col]);
                }
                boardList.Add(rowList);
            }
        }      

        var serializableFile = new
        {
            Player.players, // lista total jogadores
            player1,        //
            player2,        //  Dados do jogo em curso
            board = boardList //
        };

        var options = new JsonSerializerOptions { WriteIndented = true }; // Identação no ficheiro json
        string jsonString = JsonSerializer.Serialize(serializableFile, options);
        File.WriteAllText($"{namefile}.json", jsonString);
        Console.WriteLine("Jogo gravado com sucesso.\n");
    }

    static public void LoadFile(string namefile)
    {
        if (File.Exists($"{namefile}.json"))
        {

            var gameJson = File.ReadAllText($"{namefile}.json");

            if (gameJson != string.Empty)
            {
                var options = new JsonSerializerOptions { IncludeFields = true };
                var activeGame = JsonSerializer.Deserialize<SerializableFile>(gameJson, options);
                Player.players = activeGame.players;

                if (activeGame.player1 != null)
                {
                    player1 = activeGame.player1;
                    player2 = activeGame.player2;
                    board = new Piece[8, 8];

                    for (int row = 0; row < 8; row++)
                    {
                        for (int col = 0; col < 8; col++)
                        {
                            if (activeGame.board[row][col] != null)
                                board[row, col] = activeGame.board[row][col];
                            else board[row, col] = null;
                        }
                    }

                    Console.WriteLine("Ficheiro lido com sucesso.\nExiste um jogo em curso.\n");
                    Board.PrintBoard(board);
                }
                else Console.WriteLine("Ficheiro lido com sucesso.\n");
            }            
        }
        else
        {
            Console.WriteLine("O ficheiro não existe.\n");
        }
    }
    
    static public void MovePiece(string fromPos, string toPos)
    {
        Piece piece;
        
        Location fromLocation = new Location{ 
            Row = GetRowCoord(fromPos[1]),
            Col = GetColCoord(Char.ToUpper(fromPos[0]))
        };        

        Location toLocation = new Location{
            Row = GetRowCoord(toPos[1]),
            Col = GetColCoord(Char.ToUpper(toPos[0]))
        };

        if (!IsInBounds(fromLocation.Row, board.GetLength(1)) || !IsInBounds(fromLocation.Col, board.GetLength(0)))
        {
            Console.WriteLine("Jogada fora dos limites do tabuleiro.\n");
        }
        
        else
        {
            piece = board[fromLocation.Row, fromLocation.Col];
            // Altera a peça de localização, e coloca null onde estava anteriormente
            board[toLocation.Row, toLocation.Col] = piece;
            board[toLocation.Row, toLocation.Col].Location.Col = toLocation.Col;
            board[toLocation.Row, toLocation.Col].Location.Row = toLocation.Row;
            board[fromLocation.Row, fromLocation.Col] = null;

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

    static public bool IsInBounds(int coord, int limit)
    {
        return ((coord + 1 >= 0) && (coord + 1 <= limit));
    }

    #endregion

    // Classe para entrar como objeto no JsonSerializer
    public class SerializableFile
    {
        public List<Player> players { get; set; }
        public Player player1 { get; set; }
        public Player player2 { get; set; }
        public List<List<Piece>> board { get; set; }   
    }
}

