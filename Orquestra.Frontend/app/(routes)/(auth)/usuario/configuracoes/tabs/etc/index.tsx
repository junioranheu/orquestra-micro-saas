'use client';
import { iMe } from '@/app/api/consts/auth';
import FontScaler from '@/app/components/font-scaler';
import styles from './index.module.scss';

interface iProps {
    me: iMe;
}

export default function UsuarioConfiguracoesTabEtc({ me }: iProps) {
    return (
        <section className={styles.main}>
            <h1>Etc {me?.currentMainCompany?.name}</h1>

            <FontScaler />
        </section>
    )
}