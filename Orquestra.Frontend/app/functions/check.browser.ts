import SYSTEM from '@/app/consts/system';
import swal from './swal';

export function handleCheckIsSupportedBrowser(showSwal: boolean = true) {
    const userAgent = navigator.userAgent.toLowerCase();
    const isChrome = /chrome/.test(userAgent) && !/edge|edg|opr|opera/.test(userAgent);
    const isEdge = /edg|edge/.test(userAgent);

    const isSupported = isChrome || isEdge;

    if (showSwal && !isSupported) {
        swal({
            title: 'Navegador não suportado.',
            content: `O ${SYSTEM.NAME} atualmente só é suportado nos navegadores Chrome e Edge.`,
            confirmBtnText: 'Entendi',
            confirmFunction: () => location.reload(),
            icon: 'error',
            allowOutsideClick: false
        });
    }

    return isSupported;
}