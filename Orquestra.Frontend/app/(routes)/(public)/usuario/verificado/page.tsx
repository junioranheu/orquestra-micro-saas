'use client';
import LayoutTemplateMessage from '@/app/components/template/template-message';
import SYSTEM from '@/app/consts/system';
import { handleLaunchConfetti } from '@/app/functions/effect.confetti';
import useTitle from '@/app/hooks/useTitle';
import { useEffect } from 'react';
import styles from './page.module.scss';

export default function UsuarioVerificado() {

    useTitle('Bem-vindo');

    useEffect(() => {
        handleLaunchConfetti();
    }, []);

    return (
        <section className={styles.main}>
            <LayoutTemplateMessage
                variant='success'
                title='Uhu!'
                description={
                    `Estamos muito felizes por você estar aqui no <b>${SYSTEM.NAME}</b>!<br/>
                Agora você já pode voltar ao início para realizar o login na plataforma.`
                }
                showHelpPage={false}
            />
        </section>
    )
}