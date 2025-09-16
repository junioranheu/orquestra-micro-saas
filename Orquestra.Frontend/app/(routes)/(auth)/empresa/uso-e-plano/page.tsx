'use client';
import useTitle from '@/app/hooks/useTitle';
import styles from './page.module.scss';

export default function EmpresaUsoEPlano() {

    useTitle('Uso e plano');

    return (
        <section className={styles.main}>
            <h1>Olá... Uso e plano</h1>
        </section>
    )
}