string input;

//printBoard(board, letters, numbers);
//findPiece(board, "BP8", letters);
//movePiece(board, "C7", "C5");

Player.DeserializePlayerData(); // Carrega lista de jogadores no inicio da aplicação
Board.PrintBoard(Board.board);
do{    
    Console.Write("Comando: ");
    input = Console.ReadLine();
    Console.WriteLine();
    CheckCommand(input);
}
while(input != " ");

void movePiece(string[,] board, string from, string to){
    // From = "C7",  To = "C5"
    char c = from[0];
    int column = c - 'A';
    c = from[1];
    int line = 7 - (c - '1');            
            
    //Guarda a peça que estava na posição, na variavel piece
    string piece = board[line, column];
    board[line, column] = "";

    c = to[0];
    column = c - 'A';
    c = to[1];
    line = 7 - (c - '1');
    board[line, column] = piece;
}
    
void findPiece(string[,] board, string piece, char[] letters){
    for(int line = 0; line < board.GetLength(0); line++){
        for(int col = 0; col < board.GetLength(1); col++){
            if(board[line, col] == piece){
                Console.WriteLine($"\n Location: {letters[col]} {8-line}");
            }
        }
    }
}

/*void translateToMatrixCoord(string[,] board, string position){
    string[,] newBoardCoord = new string[2];
    char c = position[0];
    int column = c - 'A';
    c = position[1];
    int line = 7 - (c - '1');

    return board;
}*/

void CheckCommand(string command){
    string[] words = command.Split(' ');

    switch(words[0]){
        case "RJ": // Registar Jogador
            if(words.Length < 2) Console.WriteLine("Instrução inválida.\n");
            else Player.RegisterPlayer(words[1], words);
            break;
        
        case "LJ": // Listar Jogadores
            Player.ListPlayers();
            break;

        case "IJ": // Iniciar Jogo
            Player player1 = new Player();
            Player player2 = new Player();
            
            Game.StartGame(words[1], words[2], words[3]);
            player1.Name = words[2];
            player2.Name = words[3];
            break;
        
        case "G": // Gravar 
            if(words.Length < 2) Console.WriteLine("Instrução inválida.\n");
            else Game.SaveGame(words[1]);
            break;

        case "L": // Ler Jogo           
            Game.LoadGame(words[1]);
            break;

        default:
            Console.WriteLine("Instrução inválida.\n");
            break;
    }
}
