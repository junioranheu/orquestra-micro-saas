'use client';
import LayoutTemplateOne from '@/app/components/template/template-one';
import SYSTEM from '@/app/consts/system';
import { handleLaunchConfetti } from '@/app/functions/effect.confetti';
import useTitle from '@/app/hooks/useTitle';
import { useEffect } from 'react';

export default function EmpresaVerificada() {

    useTitle('Bem-vindo');

    useEffect(() => {
        handleLaunchConfetti();
    }, []);

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