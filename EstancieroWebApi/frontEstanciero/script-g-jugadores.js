let jugadoresRegistrados = []; 
const API_BASE_URL = 'http://localhost:5203'; 
document.addEventListener('DOMContentLoaded', inicializarGestionJugadores);
function inicializarGestionJugadores() {
    // 1. Conectar el formulario de registro con la función manejadora.
    const form = document.getElementById('formRegistroJugador');
    if (form) {
        form.addEventListener('submit', manejarRegistroJugador);
    }
    // 2. Cargar la lista de jugadores existentes al abrir la página.
    cargarJugadoresRegistrados();
}
async function manejarRegistroJugador(event) {
    event.preventDefault(); // Evita que el formulario recargue la página
    // Obtener y limpiar los valores de los campos
    const dni = document.getElementById('dniNuevoJugador').value.trim();
    const nombre = document.getElementById('nombreNuevoJugador').value.trim();
    const email = document.getElementById('emailNuevoJugador').value.trim();
    // Validación básica
    if (!dni || !nombre || !email) {
        alert("Todos los campos DNI, Nombre y Email son obligatorios.");
        return;
    }
    // Crear el objeto JSON con la estructura que espera la API
    const nuevoJugador = {
        dni: dni,
        nombre: nombre,
        email: email
    };
    try {
        // Realizar la solicitud POST a la API
        const response = await fetch(`${API_BASE_URL}/Jugador`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(nuevoJugador) // Convertir el objeto JS a JSON para el cuerpo de la solicitud
        });
        if (response.ok) {
            alert(`Jugador ${nombre} (DNI: ${dni}) registrado con éxito!`);
            document.getElementById('formRegistroJugador').reset(); // Limpia el formulario
            await cargarJugadoresRegistrados(); // Recargar la lista para mostrar el nuevo jugador
        } else {
            // Manejo de errores de la API (ej: DNI duplicado, validación)
            const errorData = await response.json().catch(() => ({ message: response.statusText }));
            alert('Error al registrar jugador: ' + errorData.message);
        }
    } catch (error) {
        // Manejo de errores de red o conexión
        console.error('Error de conexión al registrar jugador:', error);
        alert('Error de conexión con la API. Verifique que el servidor esté activo.');
    }
}
async function cargarJugadoresRegistrados() {
    const listaContainer = document.getElementById('listaJugadoresRectangulo');
    listaContainer.innerHTML = '<p>Cargando lista de jugadores...</p>'; 
    try {
        // Realizar la solicitud GET a la API
        const response = await fetch(`${API_BASE_URL}/Jugadores`); 
        
        if (response.ok) {
            const apiResponse = await response.json();
            // Asumimos que la lista viene en la propiedad 'data' (patrón común del profe)
            jugadoresRegistrados = Array.isArray(apiResponse.data) ? apiResponse.data : [];
            mostrarJugadores();
        } else {
            listaContainer.innerHTML = '<p style="color: red; font-weight: bold;">Error: No se pudo cargar la lista de jugadores.</p>';
        }
    } catch (error) {
        console.error('Error al cargar jugadores:', error);
        listaContainer.innerHTML = '<p style="color: red; font-weight: bold;">Error de conexión con la API.</p>';
    }
}
function mostrarJugadores() {
    const listaContainer = document.getElementById('listaJugadoresRectangulo');
    listaContainer.innerHTML = ''; // Limpia el contenido
    if (jugadoresRegistrados.length === 0) {
        listaContainer.innerHTML = '<p>No hay jugadores registrados aún. ¡Crea uno!</p>';
        return;
    }
    // Recorrer la lista y generar el HTML para cada jugador
    jugadoresRegistrados.forEach(jugador => {
        const jugadorDiv = document.createElement('div');
        jugadorDiv.classList.add('jugador-item'); // Clase CSS para el estilo
        
        // Estructura de visualización: Nombre (en h4), DNI y Email (en p)
        jugadorDiv.innerHTML = `
            <h4>${jugador.nombre}</h4>
            <p>DNI: ${jugador.dni}</p>
            <p>Email: ${jugador.email}</p>
        `;
        listaContainer.appendChild(jugadorDiv);
    });
}