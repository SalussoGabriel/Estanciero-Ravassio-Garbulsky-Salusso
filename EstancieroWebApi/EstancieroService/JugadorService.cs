using EstancieroReponse;
using EstancieroRequest;
using Estanciero.Data;
using EstancieroEntities;
namespace EstancieroService
{
        public class JugadorService
        {
            public ApiResponse<JugadorResponse> CrearJugador(CrearJugadorRequest request)
            {
                var jugadores = JugadorFile.LeerJugadores();
                bool dniDuplicado = jugadores.Any(j => j.DniUsuario == request.DniUsuario);
                bool mailDuplicado = jugadores.Any(j => j.Mail.Equals(request.MailUsuario, StringComparison.OrdinalIgnoreCase));

                if (dniDuplicado || mailDuplicado)
                { 
                    return new ApiResponse<JugadorResponse>
                    {
                        Success = false,
                        Message = "Error: Ya existe un jugador con ese DNI o Correo Electrónico."
                    };
                }
                var nuevoJugador = new JugadorEntity
                {
                    DniUsuario = request.DniUsuario,
                    NombreUsuario = request.NombreUsuario,
                    Mail = request.MailUsuario,
                };
                JugadorFile.EscribirJugador(nuevoJugador);
                var jugadorResponse = new JugadorResponse
                {
                    DniUsuario = nuevoJugador.DniUsuario,
                    NombreUsuario = nuevoJugador.NombreUsuario,
                };
                return new ApiResponse<JugadorResponse>
                {
                    Success = true,
                    Message = "Jugador creado correctamente.",
                    Data = jugadorResponse
                };
            }
        }
    }
}
}
