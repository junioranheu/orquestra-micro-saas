'use client';
import useTitle from '@/app/hooks/useTitle';
import styles from './page.module.scss';

export default function EmpresaOrdemDeServico() {

    useTitle('Ordem de serviço');

    return (
        <section className={styles.main}>
        </section>
    )
}