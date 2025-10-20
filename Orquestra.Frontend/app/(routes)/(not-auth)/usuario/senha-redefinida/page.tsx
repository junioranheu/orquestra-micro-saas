'use client';
import LayoutTemplateOne from '@/app/components/template/template-one';
import swal from '@/app/functions/swal';
import useTitle from '@/app/hooks/useTitle';
import { useEffect } from 'react';
import styles from './page.module.scss';

export default function UsuarioSenhaRedefinida() {

    useTitle('Sua senha foi redefinida!');

    useEffect(() => {
        swal({
            content: `Sua senha foi redefinida e agora ela é idêntica ao seu e-mail de cadastro.<br/><br/>
                Por exemplo, se seu e-mail é joaozinho@gmail.com, sua nova senha será joaozinho@gmail.com.
                <br/><br/>Faça login rapidamente e troque a senha manualmente nas configurações para manter sua conta mais segura.`,
            icon: 'success',
            mustConfirm: true,
            checkboxLabel: 'Entendi!'
        });
    }, []);

    return (
        <section className={styles.main}>
            <LayoutTemplateOne
                svg='success'
                title='Uhu!'
                description={
                    `Senha redefinida com sucesso!<br/>
                Agora você já pode voltar ao início para realizar o login na plataforma.`
                }
                showSupportContact={false}
                isCentralized={true}
            />
        </section>
    )
}