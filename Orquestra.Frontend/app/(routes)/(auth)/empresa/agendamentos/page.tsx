'use client';
import useTitle from '@/app/hooks/useTitle';
import styles from './page.module.scss';

export default function EmpresaAgendamentos() {

    useTitle('Agendamentos');

    return (
        <section className={styles.main}>
            <h1>Olá... Agendamentos</h1>
        </section>
    )
}