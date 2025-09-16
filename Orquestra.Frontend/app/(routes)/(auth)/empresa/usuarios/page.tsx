'use client';
import useTitle from '@/app/hooks/useTitle';
import styles from './page.module.scss';

export default function EmpresaUsuarios() {

    useTitle('Usuários da empresa');

    return (
        <section className={styles.main}>
            <h1>Olá... Usuários da empresa</h1>
        </section>
    )
}