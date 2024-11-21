const stripe = Stripe("pk_test_51QN2BoHINAMn88z1HPUmAwp3YwEkznv4oYok7VMU4grVRHvAicXbkcrmYQVEQgthtSiwbLHuMaXRU0j1uCR6kFug00P4PdD85s");

initialize();

async function initialize() {
    const fetchClientSecret = async () => {
        const response = await fetch("/create-checkout-session", {
            method: "POST",
        });
        const { clientSecret } = await response.json();
        return clientSecret;
    };

    const handleComplete = async (result) => {
        if (result.success) {
            // Consultar el estado de la sesión
            const sessionId = new URLSearchParams(window.location.search).get("session_id");
            const response = await fetch(`/session-status?session_id=${sessionId}`);
            const session = await response.json();

            if (session.status === "complete" && session.payment_status === "paid") {
                // Redirigir a la página de reservas
                window.location.href = "/Reservacion/ConsultarReservas";
            }
        } else if (result.canceled) {
            alert("El pago fue cancelado.");
        }
    };

    const checkout = await stripe.initEmbeddedCheckout({
        fetchClientSecret,
        onComplete: handleComplete,
    });

    checkout.mount("#checkout");
}
