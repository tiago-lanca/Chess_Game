using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;

class Player
{
    #region Variables
    public string Name { get;set; }     
    public int NumGames { get;set; }
    public int NumVictory { get;set; }
    public int NumDraw { get;set; }
    public int NumLoss { get;set; }

    protected const string file = "player.json";
    public static List<Player> players = new List<Player>();
    
    #endregion

    #region Functions
    public static void ListPlayers(){
        if(_IsPlayerListEmpty()){ // Se a lista estiver vazia, desserializa os dados e adiciona à lista "players"
            DeserializePlayerData();
            ShowAllPlayers(); // Mostra cada objeto da classe Player
        }
        else ShowAllPlayers();
    }

    public static void RegisterPlayer(string name, string[] words){
        if(words.Length > 2) Console.WriteLine("Inserir nome sem espaços\n");
        else{
            if(_IsPlayerListEmpty()){
                DeserializePlayerData();
            }       
            if(!players.Exists(player => player.Name == name)){        
                //Instancia novo jogador e adiciona à lista "players"
                Player player = new Player { Name = name };
                players.Add(player);
                SerializePlayerData(); // Serializa os dados para o ficheiro json
                Console.WriteLine("Jogador registado com sucesso.\n");
            }
            else{ Console.WriteLine("Jogador existente.\n"); }
        }
    }
    public static bool _IsPlayerListEmpty(){ return players.Count == 0; }
    public static void SerializePlayerData(){
        var options = new JsonSerializerOptions{ WriteIndented = true }; // Identação no ficheiro json
        string jsonString = JsonSerializer.Serialize(players, options);
        File.WriteAllText(file, jsonString);
    }
    public static void DeserializePlayerData(){
        var playerJson = File.ReadAllText(file);
        if(playerJson != string.Empty){
            var jsonList = JsonSerializer.Deserialize<List<Player>>(playerJson);            
            players.AddRange(jsonList);            
        }
    }
    public static void ShowAllPlayers(){
        if(_IsPlayerListEmpty()) Console.WriteLine("Sem jogadores registados.\n");
        else {
            // Filtra a lista "players" em ordem decrescente pelo NºVitorias, 
            // se o valor for igual filtra alfabeticamente
            players = players.OrderByDescending(player => player.NumVictory).ThenBy(player => player.Name).ToList();
            foreach(Player player in players){
                Console.WriteLine(player);
            }
        }
        //players.ForEach(n => Console.WriteLine($"{n.name} NumJogos: {n.numJogos} NumVitorias: {n.numVitorias} NumEmpates: {n.numEmpates} NumDerrotas: {n.numDerrotas}\n")); 
    }

    // String default quando se chama o objeto da classe Player
    public override string ToString()
    {
        return $"{Name} NumJogos: {NumGames} NumVitorias: {NumVictory} NumEmpates: {NumDraw} NumDerrotas: {NumLoss}\n";
    }
    #endregion
}

 