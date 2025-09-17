'use client';
import useTitle from '@/app/hooks/useTitle';
import styles from './page.module.scss';

export default function Dashboard() {

    useTitle('Sem acesso');

    return (
        <section className={styles.main}>
            <h1>Olá... 403!</h1>
        </section>
    )
}