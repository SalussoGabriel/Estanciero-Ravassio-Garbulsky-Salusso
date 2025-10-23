using EstancieroReponse;
using EstancieroRequest;
using EstancieroService;
using Microsoft.AspNetCore.Mvc;

namespace EstancieroWebApi.Controllers
{
    [Route("partidas")]
    [ApiController]
    public class PartidaController : ControllerBase
    {
        private PartidaService partidas = new PartidaService();
        private MotorDeJuegoService motorJuego = new MotorDeJuegoService();
        [HttpPost]
        public IActionResult CrearPartida([FromBody] CrearPartidaRequest request)
        {
            if (request == null || request.DniJugadores == null)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "Solicitud inválida. Envíe una lista de DNI de jugadores."
                });
            }
            try
            {
                var resultado = partidas.CrearPartida(request.DniJugadores);

                if (!resultado.Success)
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = resultado.Message
                    });
                }

                return Ok(resultado);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = $"Error interno del servidor: {ex.Message}"
                });
            }
        }
        [HttpGet("{numeroPartida}")]
        public IActionResult ObtenerEstadoPartida(int numeroPartida)
        {
            ApiResponse<PartidaResponse> resultado = new ApiResponse<PartidaResponse>();
            if (!ModelState.IsValid)
            {
                return BadRequest(resultado);
            }
            resultado = partidas.ObtenerEstadoPartida(numeroPartida);
            if (resultado.Errors.Count > 0)
            {
                return BadRequest(resultado);
            }
            return Ok(resultado);
        }
        [HttpPut("{numeroPartida}/pausar")]
        public IActionResult PausarPartida(int numeroPartida)
        {
            var resultado = partidas.CambiarEstadoPartida(numeroPartida, true, false, false);
            return resultado.Success ? Ok(resultado) : BadRequest(resultado);
        }
        [HttpPut("{numeroPartida}/reanudar")]
        public IActionResult ReanudarPartida(int numeroPartida)
        {
            var resultado = partidas.CambiarEstadoPartida(numeroPartida, false, false, true);
            return resultado.Success ? Ok(resultado) : BadRequest(resultado);
        }
        [HttpPut("{numeroPartida}/suspender")]
        public IActionResult SuspenderPartida(int numeroPartida)
        {
            var resultado = partidas.CambiarEstadoPartida(numeroPartida, false, true, false);
            return resultado.Success ? Ok(resultado) : BadRequest(resultado);
        }
        [HttpGet("pausadas")] 
        public IActionResult ObtenerPartidasPausadas()
        {
            var resultado = partidas.ObtenerPartidasPausadas();
            if (resultado.Success)
            {
                return Ok(resultado);
            }
            return StatusCode(500, resultado);
        }
        [HttpGet("DePartida/{numeroPartida}")]
        public IActionResult ObtenerJugadoresDePartida(int numeroPartida)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var resultado = partidas.ObtenerJugadoresDePartida(numeroPartida);
            if (!resultado.Success)

                return BadRequest(resultado);

            return Ok(resultado);
        }
        [HttpPost("{numeroPartida}/lanzarDado/{dniJugador}")]
        public IActionResult LanzarDadoParaMoverJugador(int numeroPartida, int dniJugador)
        {
            var resultado = motorJuego.MoverJugador(numeroPartida, dniJugador);
            if (resultado.Success)
            {
                return Ok(resultado);
            }
            return BadRequest(resultado);
        }
        [HttpPut("{numeroPartida}/avanzarTurno")]
        public IActionResult AvanzarTurno(int numeroPartida)
        {
            try
            {
                var dniSiguiente = motorJuego.AvanzarTurno(numeroPartida);
                var resultado = new ApiResponse<int>
                {
                    Success = true,
                    Message = "Turno avanzado con éxito.",
                    Data = dniSiguiente
                };
                return Ok(resultado);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new ApiResponse<string> { Success = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string> { Success = false, Message = ex.Message });
            }
        }
        [HttpGet("{numeroPartida}/turnoActual")]
        public IActionResult ConsultarTurnoActual(int numeroPartida)
        {
            var resultado = motorJuego.ConsultarTurnoActual(numeroPartida);
            if (resultado > 0)
            {
                return Ok(resultado);
            }
            return NotFound(new ApiResponse<string> { Success = false, Message = "Turno no asignado o partida inexistente." });
        }
    }
}
