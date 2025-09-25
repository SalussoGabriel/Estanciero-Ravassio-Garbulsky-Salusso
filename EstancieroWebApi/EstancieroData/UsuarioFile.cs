using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using EstancieroEntities;

namespace Estanciero.Data
{
    public class JugadorFile
    {
        public static string archivo = Path.GetFullPath("jugadores.json");

        public static void EscribirJugador(JugadorEntity jugador)
        {
            List<JugadorEntity> jugadores = LeerJugadores();

            // si existe lo reemplaza
            jugadores.RemoveAll(x => x.DniUsuario == jugador.DniUsuario);
            jugadores.Add(jugador);

            string json = JsonConvert.SerializeObject(jugadores, Formatting.Indented);
            File.WriteAllText(archivo, json);
        }

        public static List<JugadorEntity> LeerJugadores()
        {
            if (File.Exists(archivo))
            {
                string json = File.ReadAllText(archivo);
                return JsonConvert.DeserializeObject<List<JugadorEntity>>(json);
            }
            return new List<JugadorEntity>();
        }
    }
}

