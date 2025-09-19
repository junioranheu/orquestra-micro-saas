'use client';
import useTitle from '@/app/hooks/useTitle';
import styles from './page.module.scss';

export default function Ajuda() {

    useTitle('Ajuda');

    return (
        <section className={styles.main}>
            <h1>Olá... Ajuda</h1>
        </section>
    )
}