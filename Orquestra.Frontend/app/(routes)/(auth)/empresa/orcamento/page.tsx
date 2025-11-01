'use client';
import useTitle from '@/app/hooks/useTitle';
import styles from './page.module.scss';

export default function EmpresaOrçamento() {

    useTitle('Orçamento');

    return (
        <section className={styles.main}>
            <h1>Olá... Orçamento</h1>
        </section>
    )
}