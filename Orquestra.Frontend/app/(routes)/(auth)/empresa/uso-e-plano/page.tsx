'use client';
import useTitle from '@/app/hooks/useTitle';
import styles from './page.module.scss';

export default function EmpresaUsoEPlano() {

    useTitle('Uso, plano e faturas');

    return (
        <section className={styles.main}>
            <h1>Olá... Uso, plano e faturas</h1>
        </section>
    )
}