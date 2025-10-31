'use client';
import CardCreamWithChildren from '@/app/components/card/cream-with-children';
import Icon from '@/app/components/icon';
import Button from '@/app/components/input/button';
import TemplatePageHeader from '@/app/components/template/page-header';
import useApiGetMe from '@/app/hooks/api/useApiGetMe';
import useTitle from '@/app/hooks/useTitle';

export default function EmpresaIntegracaoWhatsapp() {

    useTitle('Integração com o Whatsapp');
    const me = useApiGetMe({});

    return (
        <TemplatePageHeader
            title='Integração com WhatsApp'
            actions={[
                me?.isUserAdmOfCurrentMainCompany && (
                    <Button
                        key='add'
                        label='Salvar configurações de integração com WhatsApp'
                        handleFunction={() => null}
                        icon_feather={<Icon icon='check' size='small' />}
                    />
                )
            ]}
        >
            <CardCreamWithChildren
                title='Integração com WhatsApp'
                subtitle='Configure as mensagens padrão que serão enviadas aos seus clientes.'
            >
                <h1>xd</h1>
            </CardCreamWithChildren>
        </TemplatePageHeader>
    )
}