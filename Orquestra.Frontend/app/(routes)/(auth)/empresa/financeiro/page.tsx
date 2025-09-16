'use client';
import useTitle from '@/app/hooks/useTitle';
import styles from './page.module.scss';

export default function EmpresaFinanceiro() {

    useTitle('Financeiro');

    return (
        <section className={styles.main}>
            <h1>Olá... Financeiro</h1>
        </section>
    )
}