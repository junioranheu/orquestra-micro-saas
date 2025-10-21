'use client';
import Tabs from '@/app/components/tabs';
import useTitle from '@/app/hooks/useTitle';
import styles from './page.module.scss';
import EmpresaUsoEPlanoTabFaturas from './tabs/faturas';
import EmpresaUsoEPlanoTabPlano from './tabs/plano';

export default function EmpresaUsoEPlano() {

    useTitle('Plano e faturas');

    return (
        <section className={styles.main}>
            <Tabs
                tabs={['Plano', 'Faturas']}
                contents={[
                    <EmpresaUsoEPlanoTabPlano />,
                    <EmpresaUsoEPlanoTabFaturas />,
                ]}
            />
        </section>
    )
}