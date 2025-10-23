function inicializarMenu() {
    const btnCrear = document.getElementById('btn-inicio-juego');
    if (btnCrear) {
        // Esto te lleva a la pagina que pones en el window.location.href
        btnCrear.addEventListener('click', () => {
            window.location.href = 'crear-partida.html'; 
        });
    }
    const btnCargar = document.getElementById('btn-cargar-juego');
    if (btnCargar) {
        btnCargar.addEventListener('click', () => {
            window.location.href = 'cargar-partida.html'; 
        });
    }
    const btnGestion = document.getElementById('btn-gestion-jugadores');
    if (btnGestion) {
        btnGestion.addEventListener('click', () => {
            window.location.href = 'jugadores.html';
        });
    }
    console.log("Listeners de navegación inicializados.");
}
document.addEventListener('DOMContentLoaded', inicializarMenu);