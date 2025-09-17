'use client';
import useTitle from '@/app/hooks/useTitle';
import styles from './page.module.scss';

export default function RecuperarSenha() {

    useTitle('Esqueci minha senha');

    return (
        <section className={styles.main}>
            <h1>Olá... Recuperar senha</h1>
        </section>
    )
}