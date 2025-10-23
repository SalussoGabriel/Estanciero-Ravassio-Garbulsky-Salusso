using EstancieroEntities;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace Estanciero.Data
{
    public class MovimientoFile
    {
        public static string archivo = Path.GetFullPath("movimientos.json");
        public static void EscribirMovimiento(MovimientoEntity mov)
        {
            List<MovimientoEntity> lista = LeerMovimientos();
            lista.Add(mov);

            string json = JsonConvert.SerializeObject(lista, Formatting.Indented);
            File.WriteAllText(archivo, json);
        }
        public static List<MovimientoEntity> LeerMovimientos()
        {
            if (File.Exists(archivo))
            {
                string json = File.ReadAllText(archivo);
                return JsonConvert.DeserializeObject<List<MovimientoEntity>>(json);
            }
            return new List<MovimientoEntity>();
        }
    }
}

