using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

class PlayerList
{
    #region Variables

    protected const string file = "player.json";
    public static List<Player> players = new List<Player>();

    #endregion

    #region Functions
    public static void ListPlayers()
    {
        if (_IsPlayerListEmpty())
        { // Se a lista estiver vazia, desserializa os dados e adiciona à lista "players"
            Console.WriteLine("Sem jogadores registados.\n");
        }
        else ShowAllPlayers();
    }

    public static void RegisterPlayer(string name, string[] words)
    {

        if (!players.Exists(player => player.Name == name))
        {
            //Instancia novo jogador e adiciona à lista "players"
            Player player = new Player { Name = name };
            players.Add(player);
            Console.WriteLine("Jogador registado com sucesso.\n");
        }
        else { Console.WriteLine("Jogador existente.\n"); }

    }
    public static bool _IsPlayerListEmpty() { return players.Count == 0; }
    public static void SerializePlayerData()
    {
        var options = new JsonSerializerOptions { WriteIndented = true }; // Identação no ficheiro json
        string jsonString = JsonSerializer.Serialize(players, options);
        File.WriteAllText(file, jsonString);
    }
    public static void DeserializePlayerData()
    {
        var playerJson = File.ReadAllText(file);
        if (playerJson != string.Empty)
        {
            var jsonList = JsonSerializer.Deserialize<List<Player>>(playerJson);
            players.AddRange(jsonList);
        }
    }
    public static void ShowAllPlayers()
    {
        if (_IsPlayerListEmpty()) Console.WriteLine("Sem jogadores registados.\n");
        else
        {
            // Filtra a lista "players" em ordem decrescente pelo NºVitorias, 
            // se o valor for igual filtra alfabeticamente
            players = players.OrderByDescending(player => player.NumVictory).ThenBy(player => player.Name).ToList();
            foreach (Player player in players)
            {
                Console.WriteLine(player);
            }
        }
        //players.ForEach(n => Console.WriteLine($"{n.name} NumJogos: {n.numJogos} NumVitorias: {n.numVitorias} NumEmpates: {n.numEmpates} NumDerrotas: {n.numDerrotas}\n")); 
    }

    #endregion

}
