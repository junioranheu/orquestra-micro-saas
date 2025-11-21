import SYSTEM from '@/app/consts/system';
import swal from '@/app/functions/swal';
import detectIncognito from 'detectincognitojs';
import { useEffect, useState } from 'react';

interface iProps {
    mustShowModalIfIncognito: boolean;
}

export default function useIsIncognito({ mustShowModalIfIncognito }: iProps) {

    const [isIncognito, setIsIncognito] = useState(false);

    useEffect(() => {
        detectIncognito().then((result) => {
            setIsIncognito(result.isPrivate);
        }).catch(() => {
            setIsIncognito(false);
        });
    }, []);

    useEffect(() => {
        if (isIncognito && mustShowModalIfIncognito) {
            swal({
                content: `Não é possível acessar o ${SYSTEM.NAME} em <b>modo anônimo</b>.`,
                confirmBtnText: 'Saiba mais',
                confirmFunction: () => {
                    swal({
                        title: `Por que não é permitido o modo anônimo no ${SYSTEM.NAME}?`,
                        content: 'O sistema não pode ser usado em modo anônimo ou privado porque, nesse modo, os cookies e dados de autenticação não são salvos permanentemente. ' +
                            'Isso significa que sua sessão pode ser perdida a qualquer momento, e você não conseguiria acessar suas informações de forma segura. ' +
                            '<br/><br/><b>⚠️ Para garantir que tudo funcione corretamente, use o navegador no modo normal.</b>',
                        confirmBtnText: '',
                        confirmFunction: () => { window.location.reload() },
                        icon: 'info'
                    });
                },
                icon: 'error',
                allowOutsideClick: false
            });
        }
    }, [isIncognito, mustShowModalIfIncognito]);

    return isIncognito;
}