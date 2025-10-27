'use client';
import useTitle from '@/app/hooks/useTitle';
import styles from './page.module.scss';

export default function EmpresaIntegracaoWhatsapp() {

    useTitle('Integração com o Whatsapp');

    return (
        <section className={styles.main}>
            <h1>Olá... Integração com o Whatsapp</h1>
        </section>
    )
}