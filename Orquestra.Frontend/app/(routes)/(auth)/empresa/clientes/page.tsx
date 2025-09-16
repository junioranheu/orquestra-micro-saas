'use client';
import useTitle from '@/app/hooks/useTitle';
import styles from './page.module.scss';

export default function EmpresaClientes() {

    useTitle('Clientes');

    return (
        <section className={styles.main}>
            <h1>Olá... Clientes</h1>
        </section>
    )
}