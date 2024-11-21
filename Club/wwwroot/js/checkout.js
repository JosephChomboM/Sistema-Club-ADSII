const stripe = Stripe("pk_test_51QN2BoHINAMn88z1HPUmAwp3YwEkznv4oYok7VMU4grVRHvAicXbkcrmYQVEQgthtSiwbLHuMaXRU0j1uCR6kFug00P4PdD85s");

// Función para inicializar el formulario de Stripe
document.addEventListener("DOMContentLoaded", () => {
    // Evento para abrir el modal
    const modal = document.getElementById("stripeModal");

    modal.addEventListener("shown.bs.modal", async () => {
        const fetchClientSecret = async () => {
            const response = await fetch("/create-checkout-session", {
                method: "POST",
            });
            const { clientSecret } = await response.json();
            return clientSecret;
        };

        const checkout = await stripe.initEmbeddedCheckout({
            fetchClientSecret,
        });

        // Montar el formulario de Stripe dentro del modal
        checkout.mount("#checkout");
    });
});
