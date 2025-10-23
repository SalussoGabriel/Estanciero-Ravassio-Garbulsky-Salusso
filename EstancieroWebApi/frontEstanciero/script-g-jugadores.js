let jugadoresRegistrados = [];
const API_BASE_URL = 'http://localhost:5203'; 
//Manejo de erorres, copiado del ejemplo del ejemplo que nos paso el profe
function mostrarAlerta(mensaje, tipo = 'error') {
    if (tipo === 'error') {
        console.error('ERROR:', mensaje);
        alert(`ERROR DE LA API:\n${mensaje}`);
    } else {
        console.log('ÉXITO:', mensaje);
        alert(mensaje);
    }
}
//Registro del jugador con POST
async function manejarRegistroJugador(event) {
    event.preventDefault(); 
    const form = event.target;
    const dniInput = form.dniNuevoJugador.value.trim();
    const nombre = form.nombreNuevoJugador.value.trim();
    const email = form.emailNuevoJugador.value.trim(); 
    if (!dniInput || !nombre || !email) {
        mostrarAlerta('Todos los campos son obligatorios.', 'advertencia');
        return;
    }   
    const dniNumero = parseInt(dniInput, 10);
    if (isNaN(dniNumero) || dniNumero <= 0) {
        mostrarAlerta('El DNI debe ser un número entero positivo.', 'advertencia');
        return;
    }
    const nuevoJugador = {
        "dniJugador": dniNumero, 
        "nombreJugador": nombre,
        "mailJugador": email 
    };
    try {
        const url = `${API_BASE_URL}/Jugador`; 
        const response = await fetch(url, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(nuevoJugador)
        });
        if (response.ok) {
            mostrarAlerta(`Jugador ${nombre} registrado con éxito.`, 'éxito');
            form.reset(); 
            await cargarJugadoresRegistrados(); 
        } else {
            let mensajeError = `Error ${response.status} al registrar el jugador.`;
            try {
                const errorData = await response.json();
                if (errorData.errors) {
                    let detalles = ['Fallo de Validación (Bad Request 400):'];
                    for (const key in errorData.errors) {
                        detalles.push(`- Campo ${key}: ${errorData.errors[key].join('; ')}`);
                    }
                    mensajeError = detalles.join('\n');
                } else if (errorData.title) {
                    mensajeError = errorData.title;
                }
            } catch (e) {
                console.warn("La respuesta de error no es un JSON estándar de .NET. Estado:", response.status);
                mensajeError = `Error desconocido (${response.status}). Revise la consola.`;
            }
            mostrarAlerta(mensajeError, 'error');
        }
    } catch (error) {
        console.error('Error de conexión o fallo de red:', error);
        mostrarAlerta('Error de conexión con la API. Verifique que el servidor esté activo.', 'error');
    }
}
//Aca lo que hace es que carga los jugadores registrados con el GET
async function cargarJugadoresRegistrados() {
    const listaContainer = document.getElementById('listaJugadoresRectangulo');
    listaContainer.innerHTML = '<p class="texto-carga">Cargando lista de jugadores...</p>'; 
    try {
        const url = `${API_BASE_URL}/Jugador/ObtenerTodosLosJugadores`; 
        const response = await fetch(url);
        if (response.ok) {
            const apiResponse = await response.json();
            let jugadores = Array.isArray(apiResponse) ? apiResponse : [];
            if (jugadores.length === 0 && apiResponse && typeof apiResponse === 'object') {
                if (Array.isArray(apiResponse.data)) {
                    jugadores = apiResponse.data;
                } 
                else if (Array.isArray(apiResponse.jugadores)) {
                    jugadores = apiResponse.jugadores;
                }
            }
            jugadoresRegistrados = jugadores;
            mostrarJugadores();
        } else {
            listaContainer.innerHTML = `<p class="texto-error">Error ${response.status} al obtener jugadores.</p>`;
            console.error(`Error ${response.status} al obtener jugadores.`, await response.text());
        }
    } catch (error) {
        listaContainer.innerHTML = '<p class="texto-error">No se puede cargar la lista de jugadores. (API Caída o Error de Red)</p>';
        console.error('Error al cargar jugadores:', error);
    }
}
function mostrarJugadores() {
    const listaContainer = document.getElementById('listaJugadoresRectangulo');
    listaContainer.innerHTML = ''; 
    if (jugadoresRegistrados.length === 0) {
        listaContainer.innerHTML = '<p class="texto-advertencia">No hay jugadores registrados.</p>';
        return;
    }
    jugadoresRegistrados.forEach(jugador => {
        const jugadorDiv = document.createElement('div');
        jugadorDiv.className = 'jugador-item';
        const dni = jugador.dniJugador || jugador.DniJugador || 'N/A';
        const nombre = jugador.nombreJugador || jugador.NombreJugador || 'N/A';
        const pj = jugador.partidasJugadas || jugador.PartidasJugadas || 0;
        const pg = jugador.partidasGanadas || jugador.PartidasGanadas || 0;
        const pp = jugador.partidasPerdidas || jugador.PartidasPerdidas || 0;
        const ppe = jugador.partidasPendientes || jugador.PartidasPendientes || 0;
        jugadorDiv.innerHTML = `
            <p class="jugador-nombre">👤 ${nombre}</p>
            <p class="jugador-detalle">DNI: ${dni}</p>
            <div class="stats-jugador">
                <span class="stat">Played: ${pj}</span>
                <span class="stat">Won: ${pg}</span>
                <span class="stat">Lost: ${pp}</span>
                <span class="stat">Pending: ${ppe}</span>
            </div>
        `;
        listaContainer.appendChild(jugadorDiv);
    });
}
function inicializarGestionJugadores() {
    cargarJugadoresRegistrados(); 
    const formRegistro = document.getElementById('formRegistroJugador');
    if (formRegistro) {
        formRegistro.addEventListener('submit', manejarRegistroJugador);
    }
}
document.addEventListener('DOMContentLoaded', inicializarGestionJugadores);
window.cargarJugadoresRegistrados = cargarJugadoresRegistrados;
window.manejarRegistroJugador = manejarRegistroJugador;
