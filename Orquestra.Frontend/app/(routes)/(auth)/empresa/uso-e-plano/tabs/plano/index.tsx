'use client';
import { iMe } from '@/app/api/consts/auth';
import styles from './index.module.scss';

interface iProps {
    me: iMe;
}

export default function EmpresaUsoEPlanoTabPlano({ me }: iProps) {
    return (
        <section className={styles.main}>
            <h1>Plano {me?.currentMainCompany?.name}</h1>
        </section>
    )
}