document.addEventListener("DOMContentLoaded", async () => {
    // Extraer el parámetro session_id de la URL
    const queryString = window.location.search;
    const urlParams = new URLSearchParams(queryString);
    const sessionId = urlParams.get("session_id");

    if (sessionId) {
        try {
            // Llamar al servidor para verificar el estado de la sesión
            const response = await fetch(`/session-status?session_id=${sessionId}`);
            const session = await response.json();

            if (session.status === "complete") {
                document.getElementById("success").classList.remove("hidden");
                document.getElementById("customer-email").textContent = session.customer_email;
            } else {
                // Redirigir nuevamente al checkout si el pago no se completó
                window.location.replace("/Pago/Resumen");
            }
        } catch (error) {
            console.error("Error al obtener el estado de la sesión:", error);
        }
    } else {
        console.error("No se encontró session_id en la URL.");
    }
});
