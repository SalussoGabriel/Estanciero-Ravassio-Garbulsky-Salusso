document.addEventListener("DOMContentLoaded", () => {
    const form = document.getElementById("formCrearPartida");
    if (!form) {
        console.error("No se encontró el formulario de creación de partida.");
        return;
    }
    console.log("Listener de formulario de creación de partida activo.");
    form.addEventListener("submit", async (event) => {
        event.preventDefault();
        const dniInputs = [
            document.getElementById("dniNuevoJugador1"),
            document.getElementById("dniNuevoJugador2"),
            document.getElementById("dniNuevoJugador3"),
            document.getElementById("dniNuevoJugador4")
        ];
        const dnisJugadores = dniInputs
            .map(input => parseInt(input.value))
            .filter(dni => !isNaN(dni)); // solo tomamos los que tengan número

        console.log("📦 Datos enviados a la API:", dnisJugadores);
        if (dnisJugadores.length < 2) {
            alert("Debe ingresar al menos los DNIs de los jugadores obligatorios (Jugador 1 y 2).");
            return;
        }
        try {
            const response = await fetch("http://localhost:5203/partidas", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    "Accept": "application/json"
                },
                body: JSON.stringify({ dniJugadores: dnisJugadores })
            });
            if (!response.ok) {
                const errorData = await response.json();
                const mensajeError = errorData.message || 'Error desconocido al crear la partida.';
                alert(`Error al crear la partida: ${mensajeError}`);
                console.error('API Error:', errorData);
                return;
            }
            const data = await response.json();
            console.log("Respuesta API:", data);
            if (data.success) {
                const numeroPartida = data.data.numeroPartida;
                alert(`Partida #${numeroPartida} creada con éxito. ¡A jugar!`);
                window.location.href = `tablero.html?partidaId=${numeroPartida}`;
            } else {
                alert(`Error al crear la partida: ${data.message || 'Verifique los datos.'}`);
                console.error('API Error:', data.errors);
            }
        } catch (error) {
            console.error("Error al llamar a la API:", error);
            alert("Ocurrió un error al crear la partida. Revise la consola.");
        }
    });
});
