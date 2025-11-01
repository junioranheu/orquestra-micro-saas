'use client';
import useTitle from '@/app/hooks/useTitle';
import styles from './page.module.scss';

export default function EmpresaEstoque() {

    useTitle('Estoque');

    return (
        <section className={styles.main}>
            <h1>Olá... Estoque</h1>
        </section>
    )
}