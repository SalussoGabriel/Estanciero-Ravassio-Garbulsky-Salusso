using EstancieroEntities;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace Estanciero.Data
{
    public class TableroFile
    {
        public static string archivo = Path.GetFullPath("datos/tablero.json");

        public static List<CasilleroEntity> LeerTablero()
        {
            if (File.Exists(archivo))
            {
                string json = File.ReadAllText(archivo);
                return JsonConvert.DeserializeObject<List<CasilleroEntity>>(json);
            }
            else
            {
                return new List<CasilleroEntity>();
            }
        }
    }
}

