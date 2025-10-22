'use client';
import Tabs from '@/app/components/tabs';
import useApiGetMe from '@/app/hooks/api/useApiGetMe';
import useTitle from '@/app/hooks/useTitle';
import styles from './page.module.scss';
import EmpresaUsoEPlanoTabFaturas from './tabs/faturas';
import EmpresaUsoEPlanoTabPlano from './tabs/plano';

export default function EmpresaUsoEPlano() {

    useTitle('Plano e faturas');
    const me = useApiGetMe({});

    if (!me) {
        return null;
    }

    return (
        <section className={styles.main}>
            <Tabs
                tabs={[`Plano atual da empresa ${me?.currentMainCompany?.name}`, 'Histórico de faturas']}
                contents={[
                    <EmpresaUsoEPlanoTabPlano me={me} key={1} />,
                    <EmpresaUsoEPlanoTabFaturas me={me} key={2} />
                ]}
            />
        </section>
    )
}