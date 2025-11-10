import confetti from 'canvas-confetti';

export function handleLaunchConfetti(durationMs: number = 5000) {
    const end = Date.now() + durationMs;

    const myCanvas = document.createElement('canvas');
    myCanvas.style.position = 'fixed';
    myCanvas.style.top = '0';
    myCanvas.style.left = '0';
    myCanvas.style.width = '100vw';
    myCanvas.style.height = '100vh';
    myCanvas.style.pointerEvents = 'none';
    myCanvas.style.zIndex = '999999999999999';
    document.body.appendChild(myCanvas);

    const myConfetti = confetti.create(myCanvas, { resize: true, useWorker: true });

    (function frame() {
        myConfetti({
            particleCount: 4,
            angle: 60,
            spread: 55,
            origin: { x: 0 },
            colors: ['#4CAF50', '#00BCD4', '#FFC107']
        });

        myConfetti({
            particleCount: 4,
            angle: 120,
            spread: 55,
            origin: { x: 1 },
            colors: ['#4CAF50', '#00BCD4', '#FFC107']
        });

        if (Date.now() < end) {
            requestAnimationFrame(frame);
        } else {
            // remove o canvas depois da animação pra não acumular
            setTimeout(() => myCanvas.remove(), 500);
        }
    })();
}