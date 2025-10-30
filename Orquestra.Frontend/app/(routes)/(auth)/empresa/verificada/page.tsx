'use client';
import LayoutTemplateOne from '@/app/components/template/template-one';
import SYSTEM from '@/app/consts/system';
import useTitle from '@/app/hooks/useTitle';

export default function EmpresaVerificada() {

    useTitle('Bem-vindo');

    return (
        <section>
            <LayoutTemplateOne
                variant='success'
                title='Uhu!'
                description={
                    `Estamos muito felizes por sua empresa estar aqui no <b>${SYSTEM.NAME}</b>!<br/>
                Agora você já pode voltar ao dashboard para dar continuidade nos seus afazres dentro da plataforma.`
                }
                showHelpPage={false}
            />
        </section>
    )
}