'use client';
import useTitle from '@/app/hooks/useTitle';
import styles from './page.module.scss';

export default function Dashboard() {

    useTitle('Bem-vindo');

    return (
        <section className={styles.main}>
            <h1>Olá... bem-vindo!</h1>
        </section>
    )
}