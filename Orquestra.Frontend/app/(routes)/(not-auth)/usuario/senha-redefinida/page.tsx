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
            content: `Sua senha foi redefinida e agora ela é idêntica à sua resposta de recuperação de senha.
                    <br/><br/>Faça login rapidamente e troque a senha manualmente, nas configurações da sua conta, para manter sua conta mais segura.`,
            icon: 'success',
            mustConfirm: true,
            checkboxLabel: 'Li e estou de acordo'
        });
    }, []);

    return (
        <section className={styles.main}>
            <LayoutTemplateOne
                variant='success'
                title='Uhu!'
                description={
                    `Senha redefinida com sucesso!<br/>
                Agora você já pode voltar ao início para realizar o login na plataforma.`
                }
                showHelpPage={false}
            />
        </section>
    )
}