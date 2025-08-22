export default function handleDisableAllElements(disable: boolean = true) {
    const btns = document.querySelectorAll('button');

    btns.forEach((x) => {
        x.disabled = disable;
    });

    const inputs = document.querySelectorAll('input');

    inputs.forEach((x) => {
        x.disabled = disable;
    });

    const links = document.querySelectorAll('a');

    links.forEach((x) => {
        if (disable) {
            x.style.pointerEvents = 'none';
            x.style.color = 'gray';
        } else {
            x.style.pointerEvents = 'auto';
            x.style.color = '';
        }
    });

    const selects = document.querySelectorAll('select');

    selects.forEach((x) => {
        x.disabled = disable;
    });
}