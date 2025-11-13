'use client';
import LayoutTemplateMessage from '@/app/components/template/template-message';
import ROUTES from '@/app/consts/routes';
import useTitle from '@/app/hooks/useTitle';

export default function EmpresaAcompanhamento() {

    useTitle('Acompanhamento');

    return (
        <LayoutTemplateMessage
            variant='info'
            title='Opa, parado aí'
            description='Para acompanhar o follow-up dos clientes, acesse a tela de gestão de clientes clicando no botão abaixo e selecione o cliente que deseja visualizar.'
            customButtonLabel='Visualizar clientes'
            customButtonRoute={ROUTES.EMPRESA_CLIENTES}
        />
    )
}