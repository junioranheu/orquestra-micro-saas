'use client';
import { CONSTS_INTEGRATION_WHATSAPP, iIntegrationWhatsapp } from '@/app/api/consts/integration-whatsapp';
import CardCreamWithChildren from '@/app/components/card/cream-with-children';
import Icon from '@/app/components/icon';
import Button from '@/app/components/input/button';
import TemplatePageHeader from '@/app/components/template/page-header';
import SYSTEM from '@/app/consts/system';
import { handleGetFirstName } from '@/app/functions/get.formatUserName';
import useApiGetMe from '@/app/hooks/api/useApiGetMe';
import useApiRequestToSetterOnUrlChange from '@/app/hooks/api/useApiRequestToSetterOnUrlChange';
import useTitle from '@/app/hooks/useTitle';
import { Guid } from 'guid-typescript';
import { useState } from 'react';
import styles from './page.module.scss';

export default function EmpresaIntegracaoWhatsapp() {

    useTitle('Integração com o Whatsapp');
    const me = useApiGetMe({});

    const [integration, setIntegration] = useState<iIntegrationWhatsapp>();
    useApiRequestToSetterOnUrlChange<iIntegrationWhatsapp>({ apiUrlRequest: `${CONSTS_INTEGRATION_WHATSAPP.get}?companyId=${me?.currentMainCompany?.companyId ?? Guid.EMPTY}`, setter: setIntegration });

    function handleSave() {
        alert('AEA');
    }

    return (
        <TemplatePageHeader
            title='Integração com WhatsApp'
            actions={[
                me?.isUserAdmOfCurrentMainCompany && (
                    <Button
                        key='add'
                        label='Salvar configurações de integração com WhatsApp'
                        handleFunction={() => handleSave()}
                        icon_feather={<Icon icon='check' size='small' />}
                    />
                )
            ]}
        >
            <CardCreamWithChildren
                title={<span className={SYSTEM.ANIMATE_DELAY_0_5s}>{handleGetFirstName(me?.userName)}, seja criativo...</span>}
                subtitle={<span>E <b>configure as mensagens padrão</b> que serão enviadas aos seus clientes.</span>}
            >
                <div className={styles.container}>
                    <div className={styles.header}>
                        <h2>Mensagens automáticas</h2>
                    </div>

                    <div className={styles.grid}>
                        <div className={styles.card}>
                            <h3>Mensagem enviada um dia antes:</h3>
                            <textarea value={integration?.messageReminderBeforeSchedule} />
                        </div>

                        <div className={styles.card}>
                            <h3>Mensagem enviada pouco antes do horário do agendamento:</h3>
                            <textarea value={integration?.messageBeforeScheduleAlert} />
                        </div>

                        <div className={styles.card}>
                            <h3>Mensagem enviada quando o agendamento é confirmado:</h3>
                            <textarea value={integration?.messageOnScheduleConfirmed} />
                        </div>

                        <div className={styles.card}>
                            <h3>Mensagem enviada quando o agendamento é cancelado:</h3>
                            <textarea value={integration?.messageOnScheduleCanceled} />
                        </div>

                        <div className={styles.tipSection}>
                            <Icon icon='info' />
                            <div>
                                <strong>Dica:</strong>
                                &nbsp;você pode usar as variáveis dinâmicas
                                <code>&#123;cliente&#125;</code>,
                                <code>&#123;data&#125;</code> e
                                <code>&#123;hora&#125;</code>.
                                Elas serão substituídas automaticamente pelas informações do agendamento.
                            </div>
                        </div>
                    </div>
                </div>
            </CardCreamWithChildren>
        </TemplatePageHeader>
    )
}