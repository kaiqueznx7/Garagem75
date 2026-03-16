let deferredPrompt;

window.addEventListener('beforeinstallprompt', (e) => {

    e.preventDefault();
    deferredPrompt = e;

    const btn = document.getElementById("installApp");

    if (btn) {
        btn.style.display = "block";

        btn.addEventListener("click", () => {

            btn.style.display = "none";

            deferredPrompt.prompt();

            deferredPrompt.userChoice.then(() => {
                deferredPrompt = null;
            });
        });
    }
});