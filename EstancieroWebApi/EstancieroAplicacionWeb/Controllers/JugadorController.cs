
using EstancieroReponse;
using EstancieroRequest;
using EstancieroService;
using Microsoft.AspNetCore.Mvc;

namespace EstancieroWebApi.Controllers
{
    [Route("Jugador")]
    [ApiController]
    public class JugadorController : ControllerBase
    {
        private JugadorService jugador = new JugadorService();
        [HttpPost]
        public IActionResult CrearJugador([FromBody]CrearJugadorRequest request)
        {
            ApiResponse<JugadorResponse> resultado = new ApiResponse<JugadorResponse>();
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            resultado = jugador.CrearJugador(request);
            if (resultado.Errors.Count > 0 )
            {
                return BadRequest();
            }
            return Ok(resultado);
        }
        [HttpGet("{dniJugador}")]
        public IActionResult BuscarJugadorPorDni(int dniJugador)
        {
            ApiResponse<JugadorResponse> resultado = new ApiResponse<JugadorResponse>();
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            resultado = jugador.ObtenerJugadorPorDni(dniJugador);
            if (resultado.Errors.Count > 0)
            {
                return BadRequest();
            }
            return Ok(resultado);
        }
        [HttpPut]
        public IActionResult ActualizarEstadisticasJugador(int dniJugador,bool gano, bool derrotado, bool pendiente)
        {
            ApiResponse<JugadorResponse> resultado = new ApiResponse<JugadorResponse>();
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            resultado = jugador.ActualizarEstadisticasJugador(dniJugador, gano, derrotado, pendiente);
            if (resultado.Errors.Count > 0 )
            {
                return BadRequest();
            }
            return Ok(resultado);
        }
        [HttpGet]
        public IActionResult ObtenerTodosLosJugadores()
        {
            ApiResponse<List<JugadorResponse>> resultado = new ApiResponse<List<JugadorResponse>>();
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            resultado = jugador.ObtenerTodosLosJugadores();
            if (resultado.Errors.Count > 0)
            {
                return BadRequest();
            }
            return Ok(resultado);
        }
    }
}
