'use client';
import useTitle from '@/app/hooks/useTitle';
import styles from './page.module.scss';

export default function UsuarioConfiguracoes() {

    useTitle('Configurações');

    return (
        <section className={styles.main}>
            <h1>Olá... Configurações</h1>
        </section>
    )
}