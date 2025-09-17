'use client';
import LayoutTemplateOne from '@/app/components/layout/template-one';
import useTitle from '@/app/hooks/useTitle';

export default function Erro403() {

    useTitle('Acesso negado');

    return (
        <LayoutTemplateOne
            svg='error'
            code='#403'
            title='Acesso negado'
            description='Você não tem permissão para acessar este recurso. Por favor, verifique suas credenciais ou entre em contato com o suporte caso acredite que isto seja um erro.'
            showSupportContact={true}
        />
    )
}