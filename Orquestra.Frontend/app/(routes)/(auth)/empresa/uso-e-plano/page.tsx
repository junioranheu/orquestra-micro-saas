'use client';
import Tabs from '@/app/components/tabs';
import TemplatePageHeader from '@/app/components/template/page-header';
import handleGetRandomNumber from '@/app/functions/get.randomNumber';
import useApiGetMe from '@/app/hooks/api/useApiGetMe';
import useTitle from '@/app/hooks/useTitle';
import { useEffect, useState } from 'react';
import styles from './page.module.scss';
import EmpresaUsoEPlanoTabFaturas from './tabs/faturas';
import EmpresaUsoEPlanoTabPlanos from './tabs/planos';

export default function EmpresaUsoEPlano() {

    useTitle('Plano e faturas');
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
            <TemplatePageHeader title='Uso e plano' isLoading={isLoading}>
            </TemplatePageHeader>
        )
    }

    return (
        <section className={styles.main}>
            <Tabs
                tabs={['Planos', 'Histórico de faturas']}
                contents={[
                    <EmpresaUsoEPlanoTabPlanos me={me} key={1} />,
                    <EmpresaUsoEPlanoTabFaturas me={me} key={2} />
                ]}
                isBig={true}
            />
        </section>
    )
}