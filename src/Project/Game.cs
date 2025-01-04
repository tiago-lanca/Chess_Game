using Chess_Game.src.Project;
using Chess_Game.src.Project.PiecesType;
using System.ComponentModel.Design;
using System.IO.Compression;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Text.Json.Serialization;
using static Game;

class Game()
{
    #region Variables
    static public Player? Player1 { get; set; }
    static public Player? Player2 { get; set; }
    public static Piece[,]? board { get;set; }
    public static bool _IsNewGame = true;
    public static bool Turn = true; 
    public static bool _IsGameInProgress = false;
    public static string[] gameInProgress_Players = new string[2];
    public static string jsonFile;
    public static bool player1Exists, player2Exists, endingGame = false;
    public static int Nr_Rounds { get; set; } = 0;
    public static Piece savePiece { get; set; }
    public static Piece PieceToPos { get; set; } = null;

    public static List<RoundData> savedRounds = new List<RoundData>();

    #endregion

    #region Functions
    static public void StartGame(string typegame, string playerone, string playertwo){
        player1Exists = PlayerList.players.Exists(player => player.Name == playerone);
        player2Exists = PlayerList.players.Exists(player => player.Name == playertwo);

        if (!_IsGameInProgress) {
            if (player1Exists && player2Exists)
            {
                Player1 = PlayerList.players.Find(player => player.Name == playerone);
                Player2 = PlayerList.players.Find(player => player.Name == playertwo);

                gameInProgress_Players[0] = Player1.Name;
                gameInProgress_Players[1] = Player2.Name;
                Turn = true;

                switch (typegame)
                {
                    case "Novo":
                        _IsNewGame = true;
                        Board.SetNewBoard(board = new Piece[8, 8]);
                        //Board.PrintBoard(board = new Piece[8, 8]);
                        Console.WriteLine("Jogo iniciado com sucesso.\n");
                        SaveRound(board, savedRounds);
                        _IsGameInProgress = true;
                        _IsNewGame = false;
                        endingGame = false;
                        break;

                    case "Continuação":
                        _IsNewGame = false;
                        _IsGameInProgress = true;
                        board = new Piece[8, 8];

                        for (int row = 0; row < board.GetLength(0); row++)
                        {
                            string boardLine = Console.ReadLine();
                            string[] splitTokens = boardLine.Split(',');

                                for (int col = 0; col < board.GetLength(1); col++)
                                    board[row, col] = CreatePiece(splitTokens[col], row, col);
                            
                        }

                        Console.WriteLine("\nJogo iniciado com sucesso.\n");
                        SaveRound(board, savedRounds);

                        break;

                    default:
                        Console.WriteLine("Instrução inválida.\n");
                        break;
                }
            }
            else { Console.WriteLine("Jogador inexistente.\n"); }
        }
        else { Console.WriteLine("Existe um jogo em curso.\n"); }

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
            Turn,
            Nr_Rounds,
            board = boardList,   //  Dados do jogo em curso
            SavedRounds = savedRounds
        };

        var options = new JsonSerializerOptions
        {
            WriteIndented = true, // Identação no ficheiro json
            Converters = { new PieceConverter() } // Converter a Peça no seu tipo especifico (Rook, Pawn, ...)
        }; 
        string jsonString = JsonSerializer.Serialize(serializableFile, options);
        File.WriteAllText($"{namefile}.json", jsonString);
        
        // Controla a gravação de jogo (usado no desistir)
        //if(!endingGame)
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
                        // Depois de LoadFile verifica se existe jogo no tabuleiro pendente
                        if (activeGame.board == null)
                            _IsNewGame = true;
                        else
                        {
                            _IsNewGame = false;
                            _IsGameInProgress = true;


                            Player1 = activeGame.Player1;
                            Player2 = activeGame.Player2;
                            gameInProgress_Players[0] = Player1.Name;
                            gameInProgress_Players[1] = Player2.Name;
                            Nr_Rounds = activeGame.Nr_Rounds;
                            savedRounds = activeGame.savedRounds;
                            Turn = activeGame.Turn;
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
                        }
                    }

                    Console.WriteLine("Jogo lido com sucesso.\n");
                    jsonFile = $"{namefile}";

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
                
                if (gameInProgress_Players[GetTurn(Turn)] == playerName)
                {
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
                            if ((GetTurn(Turn) == 0 && piece.PlaceHolder[0] == 'B') || GetTurn(Turn) == 1 && piece.PlaceHolder[0] == 'W')
                            {
                                switch (piece.Type)
                                {
                                    case PieceType.Pawn:
                                        piece.MovePiece(piece, fromLocation, toLocation, fromPos, toPos, board);
                                        SaveRound(board, savedRounds);
                                        break;

                                    case PieceType.Rook:
                                        piece.MovePiece(piece, fromLocation, toLocation, fromPos, toPos, board);
                                        SaveRound(board, savedRounds);
                                        break;

                                    case PieceType.Knight:
                                        piece.MovePiece(piece, fromLocation, toLocation, fromPos, toPos, board);
                                        SaveRound(board, savedRounds);
                                        break;

                                    case PieceType.Bishop:
                                        piece.MovePiece(piece, fromLocation, toLocation, fromPos, toPos, board);
                                        SaveRound(board, savedRounds);
                                        break;

                                    case PieceType.Queen:
                                        piece.MovePiece(piece, fromLocation, toLocation, fromPos, toPos, board);
                                        SaveRound(board, savedRounds);
                                        break;

                                    case PieceType.King:
                                        piece.MovePiece(piece, fromLocation, toLocation, fromPos, toPos, board);
                                        SaveRound(board, savedRounds);
                                        break;

                                    default:
                                        Console.WriteLine("Comando inválido.\n");
                                        break;
                                }
                            }
                            else Console.WriteLine("Não é a vez do jogador.\n");
                        }

                        else Console.WriteLine("Não existe peça na posição inicial.\n");
                    }
                    else Console.WriteLine("Posição inválida.\n");                
                }
                else Console.WriteLine("Não é a vez do jogador.\n");
            }
            else Console.WriteLine("Jogador não participa no jogo em curso.\n");
        }
        else Console.WriteLine("Não existe jogo em curso.\n");
    }

    public static void Command_SpecialOperation(string playerName, string fromPos, string toPos = null)
    {
        if (_IsGameInProgress)
        {
            if (playerName == gameInProgress_Players[0] || playerName == gameInProgress_Players[1])
            {

                if (gameInProgress_Players[GetTurn(Turn)] == playerName)
                {
                Piece piece;
                bool toCoordInBounds = true, fromCoordInBounds;

                Location fromLocation = new Location
                (
                    GetRowCoord(int.Parse(fromPos.Substring(1))),
                    GetColCoord(char.ToUpper(fromPos[0]))
                );

                fromCoordInBounds = IsInBounds(fromLocation.Row, board.GetLength(1)) && IsInBounds(fromLocation.Col, board.GetLength(0));

                Location toLocation = null;
                if (toPos != null)
                {
                    toLocation = new Location
                    (
                        GetRowCoord(int.Parse(toPos.Substring(1))),
                        GetColCoord(char.ToUpper(toPos[0]))
                    );

                    toCoordInBounds = IsInBounds(toLocation.Row, board.GetLength(1)) && IsInBounds(toLocation.Col, board.GetLength(0));

                }                

                // Verifica se as 2 coordenadas estão dentro dos limites do tabuleiro
                if (fromCoordInBounds)
                {
                    if (toCoordInBounds)
                    {
                        piece = board[fromLocation.Row, fromLocation.Col];

                        if (piece != null)
                        {
                            // Verifica se o jogador a jogar está a mover a peça da equipa dele
                            if ((GetTurn(Turn) == 0 && piece.PlaceHolder[0] == 'B') || GetTurn(Turn) == 1 && piece.PlaceHolder[0] == 'W')
                            {
                            switch (piece.Type)
                            {
                                case PieceType.Pawn:
                                    piece.SpecialOperation(piece, fromLocation, toLocation, fromPos, toPos, board);
                                    SaveRound(board, savedRounds);
                                    break;

                                case PieceType.Rook:
                                    piece.SpecialOperation(piece, fromLocation, toLocation, fromPos, toPos, board);
                                    SaveRound(board, savedRounds);
                                    break;

                                case PieceType.Knight:
                                    piece.SpecialOperation(piece, fromLocation, toLocation, fromPos, toPos, board);
                                    SaveRound(board, savedRounds);
                                    break;

                                case PieceType.Bishop:
                                    piece.SpecialOperation(piece, fromLocation, toLocation, fromPos, toPos, board);
                                    SaveRound(board, savedRounds);
                                    break;

                                case PieceType.Queen:
                                    piece.SpecialOperation(piece, fromLocation, toLocation, fromPos, toPos, board);
                                    SaveRound(board, savedRounds);
                                    break;

                                case PieceType.King:
                                    piece.SpecialOperation(piece, fromLocation, toLocation, fromPos, toPos, board);
                                    SaveRound(board, savedRounds);
                                    break;

                                default:
                                    Console.WriteLine("Comando inválido.\n");
                                    break;
                            }
                            }
                            else Console.WriteLine("Não é a vez do jogador.\n");
                        }

                        else Console.WriteLine("Não existe peça na posição inicial.\n");
                    }
                    else Console.WriteLine("Posição final inválida.\n");
                }
                else Console.WriteLine("Posição inválida.\n");
                }
                else Console.WriteLine("Não é a vez do jogador.\n");
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

                        PlayerLost.NumGames++;
                        PlayerWon.NumGames++;

                        //Console.WriteLine($"{player1} desistiu.\n");
                        Console.WriteLine("Jogo terminado com sucesso.\n");

                        ResetAllData(); // Faz reset dos dados para um novo jogo

                        // Para gravar o jogo após o jogo terminar por desistência
                        /*endingGame = true;
                        SaveFile(jsonFile);
                        endingGame = false;*/

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

                        Player1.NumGames++;
                        Player2.NumGames++;

                        //Console.WriteLine($"{player1} e {player2} empataram.\n");
                        Console.WriteLine("Jogo terminado com sucesso.\n");

                        ResetAllData(); // Faz reset dos dados para um novo jogo

                        // Para gravar o jogo após o jogo terminar por desistência
                        /*endingGame = true;
                        SaveFile(jsonFile);
                        endingGame = false;*/
                    }
                    else { Console.WriteLine("Jogador não participa no jogo.\n"); }
                }
                else { Console.WriteLine("Jogador inexistente.\n"); }
            }
            else { Console.WriteLine("Não existe jogo em curso.\n"); }
        }
    }
    
    public static void FinishGame_Checkmate(Player player1, Player player2)
    {
        Board.PrintBoard(board);

        if (Turn == false)
        {
            Console.WriteLine($"Checkmate. {player2.Name} venceu.\n");
            player2.NumVictory++;
            player1.NumLoss++;
        }

        else
        {
            Console.WriteLine($"Checkmate. {player1.Name} venceu.\n");
            player1.NumVictory++;
            player2.NumLoss++;
        }

        player1.NumGames++;
        player2.NumGames++;

        _IsGameInProgress = false;
        _IsNewGame = true;
    }

    public static void SaveRound(Piece[,] board, List<RoundData> savedRounds)
    {
        List<List<Piece>> boardList = new List<List<Piece>>();

        for (int row = 0; row < 8; row++)
        {
            List<Piece> rowList = new List<Piece>();
            for (int col = 0; col < 8; col++)
            {
                rowList.Add(board[row, col]?.Clone()); // Cria novas peças (evita alterações de Locations)
            }
            boardList.Add(rowList);
        }
        if (savedRounds.Count == 0 || savedRounds.Last().NrRounds != Nr_Rounds)
        {
            savedRounds.Add(new RoundData
            {
                NrRounds = Nr_Rounds,
                BoardList = boardList,
                Turn = Turn
            });

            var options = new JsonSerializerOptions
            {
                WriteIndented = true, // Identação no ficheiro json
                Converters = { new PieceConverter() } // Converter a Peça no seu tipo especifico (Rook, Pawn, ...)
            };
            string jsonString = JsonSerializer.Serialize(Game.savedRounds, options);
            File.WriteAllText($"SavedRounds.json", jsonString);
        }
    }

    public static void Undo_Round()
    {
        bool initialRound = Nr_Rounds == 0;

        var gameJson = File.ReadAllText($"SavedRounds.json");

        var options = new JsonSerializerOptions
        {
            IncludeFields = true,
            WriteIndented = true, // Identação no ficheiro json
            PropertyNameCaseInsensitive = true,
            Converters = { new PieceConverter() }
        };

        var undoRound = JsonSerializer.Deserialize<List<RoundData>>(gameJson, options);

        if (initialRound)
            Console.WriteLine("Está na jogada inicial. Não é possivel efetuar Undo.\n");

        else
        {
            board = new Piece[8, 8];

            foreach (var round in undoRound)
            {
                if (round.NrRounds == Nr_Rounds - 1)
                {
                    Nr_Rounds = round.NrRounds;
                    Turn = round.Turn;

                    for (int row = 0; row < 8; row++)
                    {
                        for (int col = 0; col < 8; col++)
                        {
                            if (round.BoardList[row][col] != null)
                                board[row, col] = round.BoardList[row][col];
                            else board[row, col] = null;
                        }
                    }

                    Console.WriteLine("Undo realizado com sucesso.\n");
                    undoRound.RemoveAt(undoRound.Count - 1); // Remove o ultimo item da lista undoRound
                    break;
                }
            }

            savedRounds.Clear();
            savedRounds = undoRound; // Limpa a lista e substitui pela nova lista criada pelo Undo para gravar no ficheiro
            string jsonString = JsonSerializer.Serialize(undoRound, options);
            File.WriteAllText($"SavedRounds.json", jsonString);
        }
        
    }

    public static int GetColCoord(int x)
    {
        return x - 'A';
    }
    public static int GetRowCoord(int y) =>  y - 1;
    // return y - 1;    

    public static int GetTurn(bool turn) { return Convert.ToInt16(turn); }
    public static bool IsInBounds(int coord, int limit) => (coord >= 0) && (coord + 1 <= limit);

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
        public bool Turn { get; set; }
        public List<List<Piece>>? board { get; set; }         
        public int Nr_Rounds { get; set; }
        public List<RoundData>? savedRounds {  get; set; }
    }

    public class RoundData
    {        
        public int NrRounds { get; set; }
        public List<List<Piece>> BoardList { get; set; }        
        public bool Turn { get; set; }
    }
}

