'use client';
import CardCreamWithChildren from '@/app/components/card/cream-with-children';
import Icon from '@/app/components/icon';
import Button from '@/app/components/input/button';
import TemplatePageHeader from '@/app/components/template/page-header';
import SYSTEM from '@/app/consts/system';
import { handleGetFirstName } from '@/app/functions/get.formatUserName';
import useApiGetMe from '@/app/hooks/api/useApiGetMe';
import useTitle from '@/app/hooks/useTitle';
import { useState } from 'react';
import styles from './page.module.scss';

export default function EmpresaIntegracaoWhatsapp() {

    useTitle('Integração com o Whatsapp');
    const me = useApiGetMe({});

    const [templates, setTemplates] = useState({
        lembreteSemana: 'Olá {cliente}! Este é um lembrete do seu agendamento no dia {{data}} às {{hora}}. Aguardamos você!',
        lembreteDia: 'Olá {cliente}, passando para lembrar do seu agendamento hoje às {{hora}}. Até breve!',
        cancelamento: 'Olá {cliente}. Confirmamos o cancelamento do seu agendamento que estava marcado para {{data}} às {{hora}}.'
    });

    function handleChange(key: string, value: string) {
        setTemplates(prev => ({ ...prev, [key]: value }));
    }

    function handleSave() {
        console.log('Mensagens salvas:', templates);
        alert('Mensagens salvas com sucesso!');
    }

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
                title={<span className={SYSTEM.ANIMATE_DELAY_0_5s}>{handleGetFirstName(me?.userName)}, seja criativo...</span>}
                subtitle={<span>E <b>configure as mensagens padrão</b> que serão enviadas aos seus clientes.</span>}
            >
                <div className={styles.container}>
                    <div className={styles.header}>
                        <h2>Mensagens automáticas</h2>
                    </div>

                    <div className={styles.grid}>
                        <div className={styles.card}>
                            <h3>Mensagem de lembrete (1 semana antes):</h3>
                            <textarea
                                value={templates.lembreteSemana}
                                onChange={e => handleChange('lembreteSemana', e.target.value)}
                            />
                        </div>

                        <div className={styles.card}>
                            <h3>Mensagem de confirmação de cancelamento:</h3>
                            <textarea
                                value={templates.cancelamento}
                                onChange={e => handleChange('cancelamento', e.target.value)}
                            />
                        </div>

                        <div className={`${styles.card} ${styles.full}`}>
                            <h3>Mensagem de lembrete (no dia):</h3>
                            <textarea
                                value={templates.lembreteDia}
                                onChange={e => handleChange('lembreteDia', e.target.value)}
                            />
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