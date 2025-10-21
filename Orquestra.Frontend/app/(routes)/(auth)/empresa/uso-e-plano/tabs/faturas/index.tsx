'use client';
import { iMe } from '@/app/api/consts/auth';
import styles from './index.module.scss';

interface iProps {
    me: iMe;
}

export default function EmpresaUsoEPlanoTabFaturas({ me }: iProps) {
    return (
        <section className={styles.main}>
            <h1>Faturas</h1>
        </section>
    )
}