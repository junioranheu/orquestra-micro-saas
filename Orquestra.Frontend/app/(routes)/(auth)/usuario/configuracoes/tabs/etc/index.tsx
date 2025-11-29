'use client';
import { CONSTS_UTILITY, iControllerInfo } from '@/app/api/consts/utility';
import { Fetch } from '@/app/api/fetch';
import SvgAuth from '@/app/assets/svg/auth.svg';
import SvgCode from '@/app/assets/svg/code.svg';
import SvgVersion from '@/app/assets/svg/version.svg';
import CardSimple from '@/app/components/card/simple';
import Footer from '@/app/components/footer';
import ROUTES from '@/app/consts/routes';
import SYSTEM from '@/app/consts/system';
import { USER_ROLE_ENUM } from '@/app/enums/userRoleEnum';
import { DATE_STYLE, handleFormatDate } from '@/app/functions/format.date';
import useApiGetBuildVersion from '@/app/hooks/api/useApiGetBuildVersion';
import useUserContext from '@/app/hooks/contexts/useUserContext';
import { useRouter } from 'next/navigation';
import { useEffect, useState } from 'react';
import styles from './index.module.scss';

export default function UsuarioConfiguracoesTabEtc() {

    const [auth,] = useUserContext();

    return (
        <section className={styles.main}>
            <CardSession />
            <CardBuild />

            {
                auth?.role === USER_ROLE_ENUM.Administrator && (
                    <CardControllers />
                )
            }

            <Footer />
        </section>
    )
}

function CardSession() {

    const router = useRouter();
    const [auth,] = useUserContext();

    return (
        <CardSimple
            img={SvgAuth}
            title='Sessão'
            description={`Sua sessão é válida até dia ${handleFormatDate(auth?.refreshTokenExpirationDate, DATE_STYLE.DETALHADO_APENAS_REFERENCIA_DIA)}.`}
            buttonLabel='Finalizar sessão'
            buttonFunction={() => router.push(ROUTES.LOGOUT)}
            buttonStyle={{ background: 'var(--contrast)', opacity: 0.8 }}
            buttonIcon='log-out'
        />
    )
}

function CardBuild() {

    const versionBuild = useApiGetBuildVersion();

    return (
        <CardSimple
            img={SvgVersion}
            title={`Versão atual do ${SYSTEM.NAME}`}
            description={`${versionBuild?.configuration} ${versionBuild?.buildVersion}.`}
        />
    )
}

function CardControllers() {

    const [controllers, setControllers] = useState<string>('');

    useEffect(() => {
        async function handleFetch() {
            const controllers = await Fetch.get({ url: CONSTS_UTILITY.getControllers }) as iControllerInfo[];

            const text = controllers?.sort((a, b) => a.controller.localeCompare(b.controller)).map(x => {
                const sortedActions = x.actions.sort((a, b) => a.localeCompare(b)).map((action, idx, arr) => idx === arr.length - 1 ? `${action}.` : action);

                return `
                <div style="line-height: 1.5; margin-bottom: 0.25rem;">
                    <strong>${x.controller}</strong>: 
                    <span>${sortedActions.join(', ')}</span>
                </div>`;
            }).join('');

            setControllers(text);
        }

        handleFetch();
    }, []);

    return (
        <CardSimple
            img={SvgCode}
            title='End-points disponíveis na aplicação'
            description={controllers}
        />
    )
}