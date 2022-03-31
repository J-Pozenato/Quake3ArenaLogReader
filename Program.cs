using System;
using System.IO;
using System.Text.Json;

namespace SerializeExtra
{
    public class Game
    {
        
        public int game { get; set; }

        public Status? status { get; set; }
    }

    public class Status
    {
        public int total_kills { get; set; }
        public IList<Player>? players { get; set; }
    }

    public class Player
    {
        public int id { get; set; }
        public string nome { get; set; }
        public int kills { get; set; }
        public List<string>? old_names { get; set; }

        public Player(int Id, string Nome, int Kills)
        {
            id = Id;
            nome = Nome;
            kills = Kills;
            old_names = new List<string>();
        }
    }

    public class Program
    {
        // PASTA
        static readonly string rootFolder = @"F:\Csharp\";
        // ARQUIVO DE TEXTO
        static readonly string textFile = @"F:\Csharp\Quake.txt";
        
        public static void Main()
        {

            // Le o arquivo linha a linha  
            string[] lines = File.ReadAllLines(textFile);  
            
            // testa se o arquivo foi lido
            //foreach (string line in lines)  
            //Console.WriteLine(line);
            int gamecount = 1;
            bool addNext = false;
            IList<Game>? games;
            games = new List<Game>();
            
            foreach (string line in lines)
            {
                string linhaDados = line.Substring(7);
                // testa se as linhas tiveram a hora removida
                //Console.WriteLine(linhaDados);
                if (linhaDados.Contains(":"))
                {
                    // Condicional que inicia o jogo
                    if (linhaDados.Substring(0, linhaDados.IndexOf(":")).Equals("InitGame"))
                    {
                        
                        games.Add(new Game
                        {
                            game = gamecount,
                            status = new Status
                            {
                                total_kills = 0,
                                players = new List<Player>()
                            },
                        }
                        );
                        
                    } 
                    // Condicional que adiciona players, quando um jogador se conecta defini que a proxima linha vai ser o jogador que precisa ser adicionado
                    else if (linhaDados.Substring(0, linhaDados.IndexOf(":")).Equals("ClientConnect"))
                    {
                        addNext = true;
                    }
                    // Adiciona o jogador
                    else if (addNext)
                    {
                        // testa se conseguiu pegar o nome inicial do jogador
                        //Console.WriteLine(linhaDados.Substring(linhaDados.IndexOf("n\\") + 2, linhaDados.IndexOf("\\t") - linhaDados.IndexOf("n\\") - 2));
                        
                        string nomeInicial = linhaDados.Substring(linhaDados.IndexOf("n\\") + 2, linhaDados.IndexOf("\\t") - linhaDados.IndexOf("n\\") - 2);
                        // testa se cosneguiu pegar o player ID
                        //Console.WriteLine(linhaDados.Substring(linhaDados.IndexOf("n\\") - 2, 1));
                        int playerid = int.Parse(linhaDados.Substring(linhaDados.IndexOf("n\\") - 2, 1)) - 1;
                        // Por algum motivo o playerId 1 estava repetindo, essa condição eliminiou a repetição do playerId
                        if (games[gamecount - 1].status.players.Count < playerid)
                        {
                            games[gamecount - 1].status.players.Add(new Player( playerid, nomeInicial,   0));
                        }
                        //Console.WriteLine(linhaDados);
                        //Console.WriteLine(linhaDados.IndexOf("n\\"));
                        
                        addNext = false;
                        
                    }
                    // Condicional que conta os kills
                    else if (linhaDados.Substring(0, linhaDados.IndexOf(":")).Equals("Kill"))
                    {
                        games[gamecount - 1].status.total_kills++;
                        // testa se o if está funcionando corretamente
                        // Console.WriteLine(linhaDados.Substring(0, linhaDados.IndexOf(":")));
                        // testa o nome do killer
                        // Console.WriteLine(linhaDados.Substring(linhaDados.IndexOf(":", 5) + 2, linhaDados.IndexOf("killed") - linhaDados.IndexOf(":", 5) - 3));
                        string killer = linhaDados.Substring(linhaDados.IndexOf(":", 5) + 2, linhaDados.IndexOf("killed") - linhaDados.IndexOf(":", 5) - 3);

                        // testa o nome do morto
                        //Console.WriteLine(linhaDados.Substring(linhaDados.IndexOf("killed") + 7, linhaDados.IndexOf("by") - linhaDados.IndexOf("killed") - 7));
                        string morto = linhaDados.Substring(linhaDados.IndexOf("killed") + 7, linhaDados.IndexOf("by") - linhaDados.IndexOf("killed") - 7);

                        if (killer.Equals("<world>") || killer.Equals(morto))
                        {
                            // testa se o if ta funcionando
                            // Console.WriteLine(killer);
                            // -1 kills para o morto
                            //Console.WriteLine(linhaDados.Substring(linhaDados.IndexOf(' ', 7) + 1, 1));
                            int mortoid = int.Parse(linhaDados.Substring(linhaDados.IndexOf(' ', 7) + 1, 1)) - 1;
                            //Console.WriteLine(mortoid);
                            if(mortoid -1 >= 0 && mortoid - 1 < games[gamecount - 1].status.players.Count)
                            {
                            games[gamecount - 1].status.players[mortoid - 1].kills -= 1;
                            } 
                            else
                            {
                                games[gamecount - 1].status.players[mortoid - 2].kills -= 1;
                            }
                        } 
                        else
                        {
                            // +1 kills para o killer
                            int killerid = int.Parse(linhaDados.Substring(linhaDados.IndexOf(' ', 4) + 1, 1)) - 1;
                            //Console.WriteLine(killerid);
                            games[gamecount - 1].status.players[killerid - 1].kills += 1;
                        }

                    }
                    // condição para alterar o nome do jogador e guardar o nome antigo na lista old_names
                    else if (linhaDados.Substring(0, linhaDados.IndexOf(":")).Equals("ClientUserinfoChanged"))
                    {
                        string playername = "";
                        int playerid = int.Parse(linhaDados.Substring(linhaDados.IndexOf("n\\") - 2, 1)) - 1;
                        if (playerid -1 >= 0 && playerid - 1 < games[gamecount - 1].status.players.Count)
                        {
                            playername = games[gamecount - 1].status.players[playerid - 1].nome;
                            string newName = linhaDados.Substring(linhaDados.IndexOf("n\\") + 2, linhaDados.IndexOf("\\t") - linhaDados.IndexOf("n\\") - 2);
                            if (!newName.Equals(playername))
                            {
                                games[gamecount - 1].status.players[playerid - 1].old_names.Add(playername);
                                games[gamecount -1 ].status.players[playerid - 1].nome = newName;
                            }
                        }
                        
                        
                    }
                    // quando o jogo acaba aumentar a contagem de jogos
                    else if (linhaDados.Substring(0, linhaDados.IndexOf(":")).Equals("ShutdownGame"))
                    {
                        gamecount++;
                    }

                }
            }
            
            
            /*
            Teste do formato do Json, algumas coisas foram alterados tinho o old names que no começo era um array mas virou uma Lista, games também virou uma lista

            var game = new Game
            {
                game = 1,
                status = new Status
                {
                    total_kills = 2,
                    players = new List<Player>()
                    {
                        new Player( 1, "joao",   0,  new string[] {""}),
                        new Player(2, "jgsdago",   2, new string[] {"asckjen"} )
                    }
                },
            };
            */

            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(games, options);
            File.WriteAllText("Quake.json", jsonString);
            // Imprime o json para testar
            //Console.WriteLine(jsonString);
        }
    }
}
