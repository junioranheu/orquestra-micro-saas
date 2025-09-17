'use client';
import LayoutTemplateOne from '@/app/components/layout/template-one';
import SYSTEM from '@/app/consts/system';
import useTitle from '@/app/hooks/useTitle';

export default function UsuarioVerificado() {

    useTitle('Bem-vindo');

    return (
        <LayoutTemplateOne
            svg='success'
            title='Uhu!'
            description={
                `Estamos muito felizes por você estar aqui no <b>${SYSTEM.NAME}</b>!<br/>
                Agora você já pode voltar ao início para realizar o login na plataforma.`
            }
            showSupportContact={false}
        />
    )
}