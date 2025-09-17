'use client';
import useTitle from '@/app/hooks/useTitle';
import styles from './page.module.scss';

export default function EmpresaGerenciar() {

    useTitle('Gerenciar empresas');

    return (
        <section className={styles.main}>
            <h1>Olá... Gerenciar empresas</h1>
        </section>
    )
}