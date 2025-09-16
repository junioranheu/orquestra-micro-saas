'use client';
import useTitle from '@/app/hooks/useTitle';
import styles from './page.module.scss';

export default function UsuarioNotificacoes() {

    useTitle('Notificações');

    return (
        <section className={styles.main}>
            <h1>Olá... Notificações</h1>
        </section>
    )
}