'use client';
import PngServer from '@/app/assets/png/server.png';
import SvgBuy from '@/app/assets/svg/buy.svg';
import SvgUserArrow from '@/app/assets/svg/user-arrow.svg';
import CardSimple from '@/app/components/card/simple';
import ROUTES from '@/app/consts/routes';
import SYSTEM from '@/app/consts/system';
import { DATE_STYLE, handleFormatDate } from '@/app/functions/format.date';
import useApiGetBuildVersion from '@/app/hooks/api/useApiGetBuildVersion';
import useUserContext from '@/app/hooks/contexts/useUserContext';
import { useRouter } from 'next/navigation';
import { useEffect } from 'react';
import styles from './index.module.scss';

export default function UsuarioConfiguracoesTabEtc() {
    return (
        <section className={styles.main}>
            <CardSession />
            <CardBuild />
            <CardControllers />
        </section>
    )
}

function CardSession() {

    const router = useRouter();
    const [auth,] = useUserContext();

    return (
        <CardSimple
            img={PngServer}
            title='Sessão'
            description={`Sua sessão é válida até dia ${handleFormatDate(auth?.refreshTokenExpirationDate, DATE_STYLE.DETALHADO_APENAS_REFERENCIA_DIA)}.`}
            buttonLabel='Finalizar sessão'
            buttonFunction={() => router.push(ROUTES.LOGOUT)}
            buttonStyle={{ background: 'var(--red)', opacity: 0.8 }}
            buttonIcon='log-out'
        />
    )
}

function CardBuild() {

    const versionBuild = useApiGetBuildVersion();

    return (
        <CardSimple
            img={SvgUserArrow}
            title={`Build do ${SYSTEM.NAME}`}
            description={`Versão atual em execução: ${versionBuild?.configuration.toLowerCase()} versão ${versionBuild?.buildVersion}.`}
        />
    )
}

function CardControllers() {

    useEffect(() => {
        alert('CHAMAR O END-POINT DE CONTROLLERS');
    }, []);

    return (
        <CardSimple
            img={SvgBuy}
            title='Configurações avançadas'
            description='Personalize a plataforma do seu jeito: gerencie preferências, permissões e integrações em um só lugar.'
        />
    )
}