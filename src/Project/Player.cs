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
    #endregion
       
    // String default quando se chama o objeto da classe Player
    public override string ToString()
    {
        return $"{Name} NumJogos: {NumGames} NumVitorias: {NumVictory} NumEmpates: {NumDraw} NumDerrotas: {NumLoss}\n";
    }
    
}

 