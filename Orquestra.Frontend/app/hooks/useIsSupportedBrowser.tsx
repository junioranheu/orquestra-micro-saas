import SYSTEM from '@/app/consts/system';
import swal from '@/app/functions/swal';
import { useEffect, useState } from 'react';

export default function useIsSupportedBrowser() {

    const [isSupported, setIsSupported] = useState<boolean>(true);

    useEffect(() => {
        const userAgent = navigator.userAgent.toLowerCase();

        const isChrome = /chrome/.test(userAgent) && !/edge|edg|opr|opera/.test(userAgent);
        const isEdge = /edg|edge/.test(userAgent);

        if (!isChrome && !isEdge) {
            setIsSupported(false);

            swal({
                title: 'Navegador não suportado',
                content: `O ${SYSTEM.NAME} só é suportado nos navegadores Chrome e Edge.`,
                confirmBtnText: 'Entendi',
                icon: 'error',
                allowOutsideClick: false
            });
        }
    }, []);

    return isSupported;

}