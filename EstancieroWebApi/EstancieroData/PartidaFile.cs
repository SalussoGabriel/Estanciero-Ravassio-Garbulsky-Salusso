using EstancieroEntities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Estanciero.Data
{
    public class PartidaFile
    {
        public static string archivo = Path.GetFullPath("partidas.json");

        public static void EscribirPartida(PartidaEntity partida)
        {
            List<PartidaEntity> partidas = LeerPartidas();

            if (partida.NumeroPartida == 0)
            {
                partida.NumeroPartida = partidas.Count + 1;
            }
            else
            {
                partidas.RemoveAll(x => x.NumeroPartida == partida.NumeroPartida);
            }

            partidas.Add(partida);

            string json = JsonConvert.SerializeObject(partidas, Formatting.Indented);
            File.WriteAllText(archivo, json);
        }

        public static List<PartidaEntity> LeerPartidas()
        {
            if (File.Exists(archivo))
            {
                string json = File.ReadAllText(archivo);
                return JsonConvert.DeserializeObject<List<PartidaEntity>>(json);
            }
            return new List<PartidaEntity>();
        }
    }
}

