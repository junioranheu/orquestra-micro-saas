'use client';
import useTitle from '@/app/hooks/useTitle';
import styles from './page.module.scss';

export default function EmpresaAcompanhamento() {

    useTitle('Acompanhamento');

    return (
        <section className={styles.main}>
            <h1>Olá... SELECIONE UM CLIENTE ANTES PARA VER OS ACOMPANHAMENTOS</h1>
        </section>
    )
}