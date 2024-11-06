string input;

//printBoard(board, letters, numbers);
//findPiece(board, "BP8", letters);
//movePiece(board, "C7", "C5");

//Player.DeserializePlayerData(); // Carrega lista de jogadores no inicio da aplicação

do{    
    Console.Write("Comando: ");
    input = Console.ReadLine();
    Console.WriteLine();
    CheckCommand(input);
}
while(input != " ");


void CheckCommand(string command){
    string[] words = command.Split(' ');

    switch(words[0]){
        case "RJ": // Registar Jogador
            if (_HasRequiredInputs(words.Length, 2))
                Player.RegisterPlayer(words[1], words);
            else Console.WriteLine("Instrução inválida.\n");
            break;
        
        case "LJ": // Listar Jogadores
            if (_HasRequiredInputs(words.Length, 1))
                Player.ListPlayers();
            else Console.WriteLine("Instrução inválida.\n");
            break;

        case "IJ": // Iniciar Jogo
            if (_HasRequiredInputs(words.Length, 4))
                Game.StartGame(words[1], words[2], words[3]);
            else Console.WriteLine("Instrução inválida.\n");
            break;

        case "MP": // Mover Peça
            if (_HasRequiredInputs(words.Length, 3))
                Game.MovePiece(words[1], words[2]);
            else Console.WriteLine("Instrução inválida.\n");
            break;
        
        case "G": // Gravar 
            if (_HasRequiredInputs(words.Length, 2))
             Game.SaveGame(words[1]);
            else Console.WriteLine("Instrução inválida.\n");
            break;

        case "L": // Ler Jogo
            if (_HasRequiredInputs(words.Length, 2))
            Game.LoadFile(words[1]);
            else Console.WriteLine("Instrução inválida.\n");
            break;
        case "clear": // Limpar Consola
            Console.Clear();
            break;
        default:
            Console.WriteLine("Instrução inválida.\n");
            break;
    }
}

bool _HasRequiredInputs(int nr_inputs, int nr_reqInputs)
{
    return nr_inputs == nr_reqInputs;
}

/*void movePiece(string[,] board, string from, string to)
{
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
}*/

/*void findPiece(string[,] board, string piece, char[] letters)
{
    for (int line = 0; line < board.GetLength(0); line++)
    {
        for (int col = 0; col < board.GetLength(1); col++)
        {
            if (board[line, col] == piece)
            {
                Console.WriteLine($"\n Location: {letters[col]} {8 - line}");
            }
        }
    }
}*/

/*void translateToMatrixCoord(string[,] board, string position){
    string[,] newBoardCoord = new string[2];
    char c = position[0];
    int column = c - 'A';
    c = position[1];
    int line = 7 - (c - '1');

    return board;
}*/

