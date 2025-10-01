'use client';
import useTitle from '@/app/hooks/useTitle';
import styles from './page.module.scss';

export default function Logs() {

    useTitle('Logs');

    return (
        <section className={styles.main}>
            <span>Logs</span>
        </section>
    )
}