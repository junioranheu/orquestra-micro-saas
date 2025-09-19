'use client';
import useTitle from '@/app/hooks/useTitle';
import styles from './page.module.scss';

export default function Seguranca() {

    useTitle('Segurança');

    return (
        <section className={styles.main}>
            <h1>Olá... Segurança</h1>
        </section>
    )
}