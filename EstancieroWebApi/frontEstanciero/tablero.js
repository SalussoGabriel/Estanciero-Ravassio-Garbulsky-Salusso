console.log("🧩 tablero.js cargado.");
const API_BASE = "http://localhost:5203";
const TABLERO_DATA_LOCAL = {
  "tablero": [
    { "nroCasillero": 0, "tipo": "inicio", "nombre": "Salida" },
    { "nroCasillero": 1, "tipo": "provincia", "nombre": "Buenos Aires", "precioCompra": 300000, "precioAlquiler": 25000 },
    { "nroCasillero": 2, "tipo": "provincia", "nombre": "Catamarca", "precioCompra": 120000, "precioAlquiler": 10000 },
    { "nroCasillero": 3, "tipo": "provincia", "nombre": "Chaco", "precioCompra": 150000, "precioAlquiler": 12000 },
    { "nroCasillero": 4, "tipo": "multa", "nombre": "Multa", "monto": 5000 },
    { "nroCasillero": 5, "tipo": "provincia", "nombre": "Chubut", "precioCompra": 180000, "precioAlquiler": 15000 },
    { "nroCasillero": 6, "tipo": "provincia", "nombre": "Córdoba", "precioCompra": 250000, "precioAlquiler": 20000 },
    { "nroCasillero": 7, "tipo": "provincia", "nombre": "Corrientes", "precioCompra": 160000, "precioAlquiler": 13000 },
    { "nroCasillero": 8, "tipo": "multa", "nombre": "Multa", "monto": 10000 },
    { "nroCasillero": 9, "tipo": "provincia", "nombre": "Entre Ríos", "precioCompra": 170000, "precioAlquiler": 14000 },
    { "nroCasillero": 10, "tipo": "provincia", "nombre": "Formosa", "precioCompra": 130000, "precioAlquiler": 11000 },
    { "nroCasillero": 11, "tipo": "premio", "nombre": "Premio Banco", "monto": 50000 },
    { "nroCasillero": 12, "tipo": "provincia", "nombre": "Jujuy", "precioCompra": 140000, "precioAlquiler": 12000 },
    { "nroCasillero": 13, "tipo": "provincia", "nombre": "La Pampa", "precioCompra": 150000, "precioAlquiler": 13000 },
    { "nroCasillero": 14, "tipo": "multa", "nombre": "Multa", "monto": 15000 },
    { "nroCasillero": 15, "tipo": "provincia", "nombre": "La Rioja", "precioCompra": 125000, "precioAlquiler": 10000 },
    { "nroCasillero": 16, "tipo": "provincia", "nombre": "Mendoza", "precioCompra": 220000, "precioAlquiler": 18000 },
    { "nroCasillero": 17, "tipo": "provincia", "nombre": "Misiones", "precioCompra": 160000, "precioAlquiler": 13000 },
    { "nroCasillero": 18, "tipo": "premio", "nombre": "Premio Banco", "monto": 100000 },
    { "nroCasillero": 19, "tipo": "provincia", "nombre": "Neuquén", "precioCompra": 200000, "precioAlquiler": 17000 },
    { "nroCasillero": 20, "tipo": "provincia", "nombre": "Río Negro", "precioCompra": 190000, "precioAlquiler": 16000 },
    { "nroCasillero": 21, "tipo": "multa", "nombre": "Multa", "monto": 20000 },
    { "nroCasillero": 22, "tipo": "provincia", "nombre": "Salta", "precioCompra": 140000, "precioAlquiler": 12000 },
    { "nroCasillero": 23, "tipo": "provincia", "nombre": "San Juan", "precioCompra": 180000, "precioAlquiler": 15000 },
    { "nroCasillero": 24, "tipo": "provincia", "nombre": "San Luis", "precioCompra": 170000, "precioAlquiler": 14000 },
    { "nroCasillero": 25, "tipo": "multa", "nombre": "Multa", "monto": 25000 },
    { "nroCasillero": 26, "tipo": "provincia", "nombre": "Santa Cruz", "precioCompra": 200000, "precioAlquiler": 17000 },
    { "nroCasillero": 27, "tipo": "provincia", "nombre": "Santa Fe", "precioCompra": 240000, "precioAlquiler": 20000 },
    { "nroCasillero": 28, "tipo": "provincia", "nombre": "Santiago del Estero", "precioCompra": 150000, "precioAlquiler": 12000 },
    { "nroCasillero": 29, "tipo": "provincia", "nombre": "Tierra del Fuego", "precioCompra": 210000, "precioAlquiler": 18000 },
    { "nroCasillero": 30, "tipo": "provincia", "nombre": "Tucumán", "precioCompra": 180000, "precioAlquiler": 15000 }
  ]
};
let jugadores = [];
let numeroPartida = 1;
let partidaData = null;
let posicionesMap = {};
let turnoActualDni = null;
// Inicialización
document.addEventListener("DOMContentLoaded", async () => {
  numeroPartida = parseInt(obtenerNumeroPartidaDesdeURL(), 10) || 1;
  console.log("Cargando Partida ID:", numeroPartida);
  await inicializarTablero();
  configurarBotones();
});
// Obtener parametros de URL
function obtenerNumeroPartidaDesdeURL() {
  const params = new URLSearchParams(window.location.search);
  return params.get("id") || params.get("partidaId") || params.get("numeroPartida");
}
// Inicializar tablero completo
async function inicializarTablero() {
  await cargarJugadores(numeroPartida);
  await obtenerEstadoPartida(numeroPartida);
  renderTableroVisual();
  mostrarListaJugadores();
}
// Cargar jugadores
async function cargarJugadores(num) {
  try {
    const resp = await fetch(`${API_BASE}/partidas/DePartida/${num}`);
    const json = await resp.json();
    if (!resp.ok || !json.success) throw new Error(json.message || "Error al cargar jugadores");
    jugadores = json.data || [];
    console.log("🔧 DEBUG: Jugadores cargados:", jugadores);
  } catch (err) {
    console.error("Error cargando jugadores:", err);
    jugadores = [];
  }
}
// Obtener estado de partida
async function obtenerEstadoPartida(num) {
  try {
    const resp = await fetch(`${API_BASE}/partidas/${num}`);
    const json = await resp.json();
    if (!resp.ok || !json.success) throw new Error("Error obteniendo estado");

    partidaData = json.data;
    posicionesMap = {};
    if (partidaData.jugadoresEnPartida) {
      partidaData.jugadoresEnPartida.forEach(j => {
        // Leemos la Posicion con P mayúscula (PascalCase)
        posicionesMap[j.dniJugador] = j.Posicion;
      });
    }

    turnoActualDni = partidaData.turnoActual || null;
    const turnoElem = document.getElementById("turnoInfo");
    if (turnoElem) {
      const nombreJugador = jugadores.find(j => j.dniJugador === turnoActualDni)?.nombreJugador || "Desconocido";
      turnoElem.textContent = `Turno actual: ${nombreJugador}`;
    }

    const estadoElem = document.getElementById("estado-partida");
    if (estadoElem) estadoElem.textContent = `Estado: ${partidaData.estadoPartida}`;
  } catch (err) {
    console.error("Error al obtener estado:", err);
  }
}
// Renderizar tablero visual
function renderTableroVisual() {
    const tableroInfo = TABLERO_DATA_LOCAL.tablero;
    
    const superior = document.getElementById("fila-superior");
    const inferior = document.getElementById("fila-inferior");
    const izq = document.getElementById("col-izquierda");
    const der = document.getElementById("col-derecha");

    if (!superior || !inferior || !izq || !der) return;
    
    superior.innerHTML = '';
    inferior.innerHTML = '';
    izq.innerHTML = '';
    der.innerHTML = '';

    const crearCasillero = (cas) => {
        const cell = document.createElement("div");
        cell.className = `casillero casillero-${cas.tipo}`;
        cell.innerHTML = `<div class="casillero-num">${cas.nroCasillero}</div><div class="casillero-nombre">${cas.nombre}</div>`;
        
        const tokens = jugadores.filter(j => Number(posicionesMap[j.dniJugador]) === cas.nroCasillero);
        tokens.forEach(j => {
            const token = document.createElement("span");
            token.className = "player-token";
            token.textContent = j.nombreJugador ? j.nombreJugador[0] : 'J';
            if (Number(j.dniJugador) === Number(turnoActualDni)) {
                 token.classList.add("turno-activo");
            }
            cell.appendChild(token);
        });
        return cell;
    };
    
    tableroInfo.forEach(cas => {
        if (cas.nroCasillero <= 8) inferior.appendChild(crearCasillero(cas));
        else if (cas.nroCasillero <= 16) izq.appendChild(crearCasillero(cas));
        else if (cas.nroCasillero <= 24) superior.appendChild(crearCasillero(cas));
        else der.appendChild(crearCasillero(cas));
    });
}
// Lista lateral de jugadores
function mostrarListaJugadores() {
  const contIzq = document.getElementById("jugadoresIzq");
  const contDer = document.getElementById("jugadoresDer");
  contIzq.innerHTML = "";
  contDer.innerHTML = "";

  jugadores.forEach(j => {
    const dni = j.dniJugador ?? j.dni;
    const nombre = j.nombreJugador ?? j.nombre ?? "Jugador";
    
    
    const pos = j.posicion ?? 0;
    const saldo = j.saldo ?? 0.0;
    const estado = j.estadoJugador ?? "Activo";
    const isTurno = dni === turnoActualDni;

    const itemIzq = document.createElement("div");
    itemIzq.className = `jugador-item ${isTurno ? "turno-activo" : ""}`;
    itemIzq.innerHTML = `
      <div>${nombre} (DNI: ${dni})</div>
      <div>Posición: ${pos}</div>
      <div>Saldo: $${Number(saldo).toLocaleString('es-AR', { minimumFractionDigits: 0, maximumFractionDigits: 0 })}</div>
    `;
    contIzq.appendChild(itemIzq);

    const itemDer = document.createElement("div");
    itemDer.className = `jugador-item ${isTurno ? "turno-activo" : ""}`;
    itemDer.innerHTML = `<div>${nombre}</div><div>Estado: ${estado}</div>`;
    contDer.appendChild(itemDer);
  });
}
// Tirar dado
async function tirarDado() {
  try {
    const respTurno = await fetch(`${API_BASE}/partidas/${numeroPartida}/turnoActual`);
    const dniJugador = await respTurno.json();
    const resp = await fetch(`${API_BASE}/partidas/${numeroPartida}/lanzarDado/${dniJugador}`, { method: "POST" });
    const data = await resp.json();
    if (!resp.ok) throw new Error(data.message);
    alert(`🎲 ${data.message}`);
    await inicializarTablero();
  } catch (err) {
    console.error("Error tirando dado:", err);
    alert(`❌ ${err.message}`);
  }
}
//Avanzar turno
async function avanzarTurno() {
  try {
    const resp = await fetch(`${API_BASE}/partidas/${numeroPartida}/avanzarTurno`, { method: "PUT" });
    const data = await resp.json();
    if (!resp.ok) throw new Error(data.message);
    alert(`🔄 Turno avanzado. Nuevo jugador DNI: ${data.data}`);
    await inicializarTablero();
  } catch (err) {
    console.error("Error avanzarTurno:", err);
    alert(`❌ ${err.message}`);
  }
}
// Pausar / Reanudar / Suspender
async function cambiarEstado(accion) {
    if (isNaN(numeroPartida)) {
        alert("Error: No se encontró el ID de partida. Asegúrate de cargar la partida correctamente.");
        return;
    }
    
    try {
        // Llama directamente al endpoint PUT /partidas/{numeroPartida}/{accion}
        const url = `${API_BASE}/partidas/${numeroPartida}/${accion}`; 
        
        const resp = await fetch(url, { method: "PUT" });
        const data = await resp.json();
        
        if (!resp.ok || !data.success) {
            // Manejar errores como "Partida no encontrada"
            throw new Error(data.message || `Error al cambiar estado a ${accion}.`);
        }
        
        alert(`✅ Partida ${accion} correctamente.`);
        // Vuelve a cargar el estado de la partida para actualizar la UI
        await inicializarTablero(); 

    } catch (error) {
        console.error(`Error al cambiar estado a ${accion}:`, error);
        alert(`❌ Error: ${error.message}`);
    }
}
// Configuración de botones
function configurarBotones() {
  document.getElementById("btnTirar").addEventListener("click", tirarDado);
  document.getElementById("btnSiguiente").addEventListener("click", avanzarTurno);
  document.getElementById("btnPausar").addEventListener("click", () => cambiarEstado("pausar"));
  document.getElementById("btnSuspender").addEventListener("click", () => cambiarEstado("suspender"));
  document.getElementById("btnReanudar").addEventListener("click", () => cambiarEstado("reanudar"));
}