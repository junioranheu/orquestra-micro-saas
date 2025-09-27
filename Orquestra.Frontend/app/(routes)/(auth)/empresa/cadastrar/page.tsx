'use client';
import useTitle from '@/app/hooks/useTitle';
import styles from './page.module.scss';

export default function EmpresaCadastrar() {

    useTitle('Cadastrar nova empresa');

    return (
        <section className={styles.main}>
            <h1>Olá... Cadastrar nova empresa</h1>
        </section>
    )
}