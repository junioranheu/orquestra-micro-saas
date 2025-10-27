'use client';
import useTitle from '@/app/hooks/useTitle';
import styles from './page.module.scss';

export default function EmpresaFollowUp() {

    useTitle('Follow-up');

    return (
        <section className={styles.main}>
            <h1>Olá... Follow-up</h1>
        </section>
    )
}