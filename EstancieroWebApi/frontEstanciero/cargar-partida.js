document.addEventListener("DOMContentLoaded", () => {
    const formCargarPartidaEl = document.getElementById("formCargarPartida");
    const inputNumeroPartidaEl = document.getElementById("numeroPartida"); 
    const errorNumeroPartidaEl = document.getElementById("errorNumeroPartida"); 
    const API_BASE_URL = 'http://localhost:5203'; // Definimos la base aquí
    if (formCargarPartidaEl) {
        formCargarPartidaEl.addEventListener("submit", manejarCarga);
    }
    async function verificarExistencia(id) {
        try {
            const response = await fetch(`${API_BASE_URL}/partidas/${id}`, { method: 'GET' });
            return response.ok; 
        } catch (e) {
            return true; 
        }
    }
    async function manejarCarga(event) {
        event.preventDefault(); 
        if (errorNumeroPartidaEl) errorNumeroPartidaEl.textContent = "";
        
        const partidaIdInput = inputNumeroPartidaEl.value.trim();
        if (!partidaIdInput) {
            if (errorNumeroPartidaEl) errorNumeroPartidaEl.textContent = "El número de partida es obligatorio.";
            return;
        }
        const partidaId = parseInt(partidaIdInput, 10);
        if (isNaN(partidaId) || partidaId <= 0) {
            if (errorNumeroPartidaEl) errorNumeroPartidaEl.textContent = "Por favor, ingresa un número de partida válido.";
            return;
        }
        window.location.href = `tablero.html?partidaId=${partidaId}`;
    }
});