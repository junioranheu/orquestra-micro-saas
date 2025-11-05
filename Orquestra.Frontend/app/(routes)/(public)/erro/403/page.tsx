'use client';
import LayoutTemplateMessage from '@/app/components/template/template-message';
import useTitle from '@/app/hooks/useTitle';
import styles from './page.module.scss';

export default function Erro403() {

    useTitle('Acesso negado');

    return (
        <section className={styles.main}>
            <LayoutTemplateMessage
                variant='error'
                code='#403'
                title='Acesso negado'
                description='Você não tem permissão para acessar este recurso. Por favor, verifique suas credenciais ou entre em contato com o suporte caso acredite que isto seja um erro.'
                showHelpPage={true}
            />
        </section>
    )
}