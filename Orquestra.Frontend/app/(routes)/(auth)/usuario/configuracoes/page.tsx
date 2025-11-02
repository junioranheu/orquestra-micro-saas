'use client';
import Tabs from '@/app/components/tabs';
import TemplatePageHeader from '@/app/components/template/page-header';
import handleGetRandomNumber from '@/app/functions/get.randomNumber';
import useApiGetMe from '@/app/hooks/api/useApiGetMe';
import useTitle from '@/app/hooks/useTitle';
import { useEffect, useState } from 'react';
import styles from './page.module.scss';
import UsuarioConfiguracoesTabConta from './tabs/conta';
import UsuarioConfiguracoesTabPersonalizacao from './tabs/personalizacao';

export default function UsuarioConfiguracoes() {

    useTitle('Configurações');
    const me = useApiGetMe({});
    const [isLoading, setIsLoading] = useState<boolean>(true);

    useEffect(() => {
        const timer = setTimeout(() => {
            setIsLoading(false);
        }, handleGetRandomNumber(1000, 1250));

        return () => clearTimeout(timer);
    }, []);

    if (!me || isLoading) {
        return (
            <TemplatePageHeader title='Configurações' isLoading={isLoading}>
            </TemplatePageHeader>
        )
    }

    return (
        <section className={styles.main}>
            <Tabs
                tabs={['Personalização', 'Configurações da conta']}
                contents={[
                    <UsuarioConfiguracoesTabPersonalizacao key={1} />,
                    <UsuarioConfiguracoesTabConta me={me} key={2} />
                ]}
                isBig={true}
            />
        </section>
    )
}