'use client';
import useTitle from '@/app/hooks/useTitle';
import styles from './page.module.scss';

export default function EmpresaNotaFiscal() {

    useTitle('Nota fiscal');

    return (
        <section className={styles.main}>
        </section>
    )
}