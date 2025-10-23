using EstancieroEntities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Estanciero.Data
{
    public static class PartidaFile
    {
        public static string archivo = Path.Combine(AppContext.BaseDirectory, "partidas.json");
        //Solucion para el numero de partida que tiraba error
        public static int ObtenerSiguienteNumeroPartida()
        {
            List<PartidaEntity> partidas = LeerPartidas();
            if (partidas == null || partidas.Count == 0)
            {
                return 1;
            }
            return partidas.Max(p => p.NumeroPartida) + 1;
        }
        public static List<PartidaEntity> LeerPartidas()
        {
            if (File.Exists(archivo))
            {
                string json = File.ReadAllText(archivo);
                return JsonConvert.DeserializeObject<List<PartidaEntity>>(json) ?? new List<PartidaEntity>();
            }
            return new List<PartidaEntity>();
        }
        public static void EscribirPartida(PartidaEntity partida)
        {
            List<PartidaEntity> partidas = LeerPartidas();
            if (partida.NumeroPartida == 0)
            {
                partida.NumeroPartida = ObtenerSiguienteNumeroPartida();
            }
            else
            {
                partidas.RemoveAll(x => x.NumeroPartida == partida.NumeroPartida);
            }
            partidas.Add(partida);
            string json = JsonConvert.SerializeObject(partidas, Formatting.Indented);
            File.WriteAllText(archivo, json);
        }
    }
}

