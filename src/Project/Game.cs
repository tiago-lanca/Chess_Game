using Chess_Game.src.Project;
using Chess_Game.src.Project.PiecesType;
using System.IO.Compression;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Text.Json.Serialization;

class Game()
{
    #region Variables
    static public Player? Player1 { get; set; }
    static public Player? Player2 { get; set; }
    public static Piece[,]? board { get;set; }
    public static bool _IsNewGame = true;
    public static bool? Turn = false;
    public static bool _IsGameInProgress = false;
    public static string[] gameInProgress_Players = new string[2];
    public static string jsonFile;
    public static bool player1Exists, player2Exists, endingGame = false;
    public static int Nr_Moves { get; set; } = 0;


    #endregion

    #region Functions
    static public void StartGame(string typegame, string playerone, string playertwo, string testBoard = null){
        player1Exists = PlayerList.players.Exists(player => player.Name == playerone);
        player2Exists = PlayerList.players.Exists(player => player.Name == playertwo);

        if (!_IsGameInProgress) {
            if (player1Exists && player2Exists)
            {
                Player1 = PlayerList.players.Find(player => player.Name == playerone);
                Player2 = PlayerList.players.Find(player => player.Name == playertwo);

                gameInProgress_Players[0] = Player1.Name;
                gameInProgress_Players[1] = Player2.Name;
                _IsGameInProgress = true;

                switch (typegame)
                {
                    case "novo":
                        _IsNewGame = true;
                        Board.PrintBoard(board = new Piece[8, 8]);
                        Console.WriteLine("Jogo iniciado com sucesso.\n");
                        _IsNewGame = false;
                        endingGame = false;
                        break;

                    case "continuacao":
                        _IsNewGame = false;
                        string?[] tokens = new string[64];
                        string[] splitTokens = testBoard.Split(',');
                        for(int i = 0; i < tokens.Length; i++) 
                        {
                            if (i < splitTokens.Length)
                                tokens[i] = splitTokens[i];
                            else tokens[i] = null;
                        }
                        int nr_tokens = 0;
                        board = new Piece[8, 8];

                        for (int row = 0; row < board.GetLength(0); row++)
                        {
                            for (int col = 0; col < board.GetLength(1); col++)
                            {
                                board[row, col] = CreatePiece(tokens[nr_tokens], row, col);
                                nr_tokens++;
                            }
                        }
                        Board.PrintBoard(board);
                        break;

                    default:
                        Console.WriteLine("Instrução inválida");
                        break;
                }
            }
            else { Console.WriteLine("Jogador inexistente.\n"); }
        }
        else { Console.WriteLine("Existe um jogo em curso.\n"); }

        // Criar variavel turno para verificar quem joga a jogada, criar array de jogadores
        // do jogo em curso e verificar se no MP NomeJogador coincide com o array[turno].nome
    }

    static public void SaveFileExiting(){ SaveFile(jsonFile); }

    static public void SaveFile(string namefile){
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
            PlayerList.players, // lista total jogadores
            Player1,            //
            Player2,
            //Turn,            
            board = boardList   //  Dados do jogo em curso
        };

        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            Converters = { new PieceConverter() }
        }; // Identação no ficheiro json
        string jsonString = JsonSerializer.Serialize(serializableFile, options);
        File.WriteAllText($"{namefile}.json", jsonString);
        
        if(!endingGame)
            Console.WriteLine("Jogo gravado com sucesso.\n");

        jsonFile = $"{namefile}";
    }

    static public void LoadFile(string namefile)
    {
        if (!_IsGameInProgress)
        {
            if (File.Exists($"{namefile}.json"))
            {
                var gameJson = File.ReadAllText($"{namefile}.json");

                if (gameJson != string.Empty)
                {
                    var options = new JsonSerializerOptions
                    {
                        IncludeFields = true,
                        PropertyNameCaseInsensitive = true,
                        Converters = { new PieceConverter() }
                    };

                    var activeGame = JsonSerializer.Deserialize<SerializableFile>(gameJson, options);
                    PlayerList.players = activeGame.players;

                    if (activeGame.board.Count != 0) // Haver jogo em curso gravado
                    {
                        Player1 = activeGame.Player1;
                        Player2 = activeGame.Player2;
                        gameInProgress_Players[0] = Player1.Name;
                        gameInProgress_Players[1] = Player2.Name;
                        //Turn = activeGame.Turn;
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

                        // Depois de LoadFile verifica se existe jogo no tabuleiro pendente
                        if (board == null) _IsNewGame = true;
                        
                        else
                        {
                            _IsNewGame = false;
                            _IsGameInProgress = true;
                        }

                        Console.WriteLine("Ficheiro lido com sucesso.\n");
                        jsonFile = $"{namefile}";
                    }
                    else
                    {
                        Console.WriteLine("Jogo lido com sucesso.\n");
                        jsonFile = $"{namefile}";
                    }
                }
                else
                {
                    Console.WriteLine("O ficheiro não existe.\n");
                }
            }
        }
        else Console.WriteLine("Existe um jogo em curso.\n");
    }

    public static void Command_MovePiece(string playerName, string fromPos, string toPos)
    {
        if (_IsGameInProgress)
        {
            if (playerName == gameInProgress_Players[0] || playerName == gameInProgress_Players[1])
            {
                
                //if (gameInProgress_Players[GetTurn(Turn)] == playerName)
                //{
                    Piece piece;

                    Location fromLocation = new Location
                    (
                        GetRowCoord(int.Parse(fromPos.Substring(1))),
                        GetColCoord(char.ToUpper(fromPos[0]))
                    );

                    Location toLocation = new Location
                    (
                        GetRowCoord(int.Parse(toPos.Substring(1))),
                        GetColCoord(char.ToUpper(toPos[0]))
                    );

                    bool fromCoordInBounds = IsInBounds(fromLocation.Row, board.GetLength(1)) && IsInBounds(fromLocation.Col, board.GetLength(0));
                    bool toCoordInBounds = IsInBounds(toLocation.Row, board.GetLength(1)) && IsInBounds(toLocation.Col, board.GetLength(0));

                    // Verifica se as 2 coordenadas estão dentro dos limites do tabuleiro
                    if (fromCoordInBounds && toCoordInBounds)
                    {
                        piece = board[fromLocation.Row, fromLocation.Col];

                        if (piece != null)
                        {
                            // Verifica se o jogador a jogar está a mover a peça da equipa dele
                            //if ((GetTurn(Turn) == 0 && piece.PlaceHolder[0] == 'B') || GetTurn(Turn) == 1 && piece.PlaceHolder[0] == 'W')
                            //{
                                switch (piece.Type)
                                {
                                    case PieceType.Pawn:
                                        piece.MovePiece(piece, fromLocation, toLocation, fromPos, toPos, board);


                                        break;

                                    case PieceType.Rook:
                                        piece.MovePiece(piece, fromLocation, toLocation, fromPos, toPos, board);
                                        break;

                                    case PieceType.Knight:
                                        piece.MovePiece(piece, fromLocation, toLocation, fromPos, toPos, board);
                                        break;

                                    case PieceType.Bishop:
                                        piece.MovePiece(piece, fromLocation, toLocation, fromPos, toPos, board);
                                        break;

                                    case PieceType.Queen:
                                        piece.MovePiece(piece, fromLocation, toLocation, fromPos, toPos, board);
                                        break;

                                    case PieceType.King:
                                        piece.MovePiece(piece, fromLocation, toLocation, fromPos, toPos, board);
                                        break;

                                    default:
                                        Console.WriteLine("Comando inválido.\n");
                                        break;
                                }
                            //}
                            //else Console.WriteLine("Não é a vez do jogador.\n");
                        }

                        else Console.WriteLine("Não existe peça na posição inicial.\n");
                    }
                    else Console.WriteLine("Posição inválida.\n");                
                //}
                //else Console.WriteLine("Não é a vez do jogador.\n");
            }
            else Console.WriteLine("Jogador não participa no jogo em curso.\n");
        }
        else Console.WriteLine("Não existe jogo em curso.\n");
    }

    public static void ForfeitGame(string player1, string player2 = null)
    {
        if (player2 == null)
        {
            player1Exists = PlayerList.players.Exists(player => player.Name == player1);

            if (_IsGameInProgress) // Verifica se existe jogo em curso
            {
                if (player1Exists) // Verifica se o jogador existe na lista de jogadores registados
                {
                    if (gameInProgress_Players.Contains(player1)) // Verifica se o jogador participa no jogo em curso
                    {
                        List<String> playerWon = new List<String>(gameInProgress_Players);
                        playerWon.Remove(player1);
                        Player PlayerLost = PlayerList.players.Find(player => player.Name == player1);
                        Player PlayerWon = PlayerList.players.Find(player => player.Name == playerWon.ElementAt(0));
                        PlayerLost.NumLoss++;
                        PlayerWon.NumVictory++;
                        
                        Console.WriteLine($"{player1} desistiu.\n");
                        Console.WriteLine("Jogo terminado com sucesso.\n");

                        ResetAllData(); // Faz reset dos dados para um novo jogo

                        endingGame = true;
                        SaveFile(jsonFile);
                        endingGame = false;

                    }
                    else { Console.WriteLine("Jogador não participa no jogo em curso.\n"); }
                }
                else { Console.WriteLine("Jogador inexistente.\n"); }
            }
            else { Console.WriteLine("Não existe jogo em curso.\n"); }
            
        }
        else
        {
            player1Exists = PlayerList.players.Exists(player => player.Name == player1);
            player2Exists = PlayerList.players.Exists(player => player.Name == player2);

            if (_IsGameInProgress)
            {
                if (player1Exists && player2Exists)
                {
                    if (gameInProgress_Players.Contains(player1) && gameInProgress_Players.Contains(player2))
                    {
                        Player1 = PlayerList.players.Find(player => player.Name == player1);
                        Player2 = PlayerList.players.Find(player => player.Name == player2);
                        Player1.NumDraw++;
                        Player2.NumDraw++;
                        Console.WriteLine($"{player1} e {player2} empataram.\n");

                        ResetAllData(); // Faz reset dos dados para um novo jogo

                        endingGame = true;
                        SaveFile(jsonFile);
                        endingGame = false;
                    }
                    else { Console.WriteLine("Jogador não participa no jogo.\n"); }
                }
                else { Console.WriteLine("Jogador inexistente.\n"); }
            }
            else { Console.WriteLine("Não existe jogo em curso.\n"); }
        }
    }
    public static int GetColCoord(int x)
    {
        return x - 'A';
    }
    public static int GetRowCoord(int y) =>  y - 1;
    // return y - 1;
    

    public static int GetTurn(bool turn) { return Convert.ToInt16(turn); }
    public static bool IsInBounds(int coord, int limit) => (coord + 1 >= 0) && (coord + 1 <= limit);

    public static void ResetAllData()
    {
        Player1 = null;
        Player2 = null;
        board = null;
        _IsNewGame = true;
        _IsGameInProgress = false;
    }

    private static bool IsWhiteTeam(string boardPiece) { return boardPiece[0] == 'W'; } 
        
    public static Piece CreatePiece(string boardPiece, int row, int col)
    {
        if (boardPiece == "" || boardPiece == null) return null;
        else
        {
            switch (boardPiece[1])
            {
                case 'P':
                    return new Pawn(PieceType.Pawn, new Location(row, col), IsWhiteTeam(boardPiece) ? PieceTeam.White : PieceTeam.Black, boardPiece);

                case 'R':
                    return new Rook(PieceType.Rook, new Location(row, col), IsWhiteTeam(boardPiece) ? PieceTeam.White : PieceTeam.Black, boardPiece);

                case 'H':
                    return new Knight(PieceType.Knight, new Location(row, col), IsWhiteTeam(boardPiece) ? PieceTeam.White : PieceTeam.Black, boardPiece);

                case 'B':
                    return new Bishop(PieceType.Bishop, new Location(row, col), IsWhiteTeam(boardPiece) ? PieceTeam.White : PieceTeam.Black, boardPiece);

                case 'Q':
                    return new Queen(PieceType.Queen, new Location(row, col), IsWhiteTeam(boardPiece) ? PieceTeam.White : PieceTeam.Black, boardPiece);

                case 'K':
                    return new King(PieceType.King, new Location(row, col), IsWhiteTeam(boardPiece) ? PieceTeam.White : PieceTeam.Black, boardPiece);

                default:
                    return null;
            }
        }
    }
    
    #endregion

    // Classe para entrar como objeto no JsonSerializer
    public class SerializableFile
    {
        public List<Player>? players { get; set; }
        public Player? Player1 { get; set; }
        public Player? Player2 { get; set; }
        //public bool? Turn { get; set; }
        public List<List<Piece>>? board { get; set; }   
    }
}

