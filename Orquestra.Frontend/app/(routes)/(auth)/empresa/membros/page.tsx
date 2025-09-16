'use client';
import useTitle from '@/app/hooks/useTitle';
import styles from './page.module.scss';

export default function EmpresaMembros() {

    useTitle('Membros da empresa');

    return (
        <section className={styles.main}>
            <h1>Olá... Membros da empresa</h1>
        </section>
    )
}