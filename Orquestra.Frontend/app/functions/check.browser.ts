import SYSTEM from '@/app/consts/system';
import swal from './swal';

export function handleCheckIsSupportedBrowser(showSwal: boolean = true) {
    const userAgent = navigator.userAgent.toLowerCase();

    const isChrome = /chrome/.test(userAgent) && !/edge|edg|opr|opera/.test(userAgent);
    const isEdge = /edg|edge/.test(userAgent);
    const isFirefox = /firefox/.test(userAgent);
    const isSafari = /safari/.test(userAgent) && !/chrome|chromium|edg|opr|opera/.test(userAgent);

    const isSupported = isChrome || isEdge || isFirefox || isSafari;

    if (showSwal && !isSupported) {
        swal({
            title: 'Navegador não suportado.',
            content: `O ${SYSTEM.NAME} atualmente só é suportado nos navegadores Chrome, Firefox, Edge e Safari.`,
            confirmBtnText: 'Entendi',
            confirmFunction: () => location.reload(),
            icon: 'error',
            allowOutsideClick: false
        });
    }

    return isSupported;
}