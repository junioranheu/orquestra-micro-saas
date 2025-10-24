import SYSTEM from '@/app/consts/system';
import { handleCheckIsSupportedBrowser } from '@/app/functions/check.browser';
import swal from '@/app/functions/swal';
import { useEffect, useState } from 'react';

export default function useIsSupportedBrowser() {

    const [isSupported, setIsSupported] = useState<boolean>(true);

    useEffect(() => {
        const checkIsSupported = handleCheckIsSupportedBrowser();

        if (!checkIsSupported) {
            setIsSupported(false);

            swal({
                title: 'Navegador não suportado.',
                content: `O ${SYSTEM.NAME} atualmente só é suportado nos navegadores Chrome e Edge.`,
                confirmBtnText: 'Entendi',
                confirmFunction: () => location.reload(),
                icon: 'error',
                allowOutsideClick: false
            });
        }
    }, []);

    return isSupported;

}