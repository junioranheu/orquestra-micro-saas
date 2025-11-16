'use client';
import { CONSTS_INTEGRATION_WHATSAPP, iIntegrationWhatsapp } from '@/app/api/consts/integration-whatsapp';
import { Fetch } from '@/app/api/fetch';
import CardCreamWithChildren from '@/app/components/card/cream-with-children';
import Icon from '@/app/components/icon';
import Button from '@/app/components/input/button';
import Mascot from '@/app/components/mascot';
import TemplatePageHeader from '@/app/components/template/template-page-header';
import ROUTES from '@/app/consts/routes';
import SYSTEM from '@/app/consts/system';
import { handleGetFirstName } from '@/app/functions/get.formatUserName';
import swal from '@/app/functions/swal';
import useApiGetMe from '@/app/hooks/api/useApiGetMe';
import useApiRequestToSetterOnUrlChange from '@/app/hooks/api/useApiRequestToSetterOnUrlChange';
import useTitle from '@/app/hooks/useTitle';
import { Guid } from 'guid-typescript';
import { useRouter } from 'next/navigation';
import { useEffect, useState } from 'react';
import styles from './page.module.scss';

export default function EmpresaIntegracaoWhatsapp() {

    useTitle('Integração com o Whatsapp');
    const me = useApiGetMe({});
    const router = useRouter();

    const [isLoading, setIsLoading] = useState<boolean>(true);
    const [hasPermitionToSave, setHasPermitionToSave] = useState<boolean>(true);

    useEffect(() => {
        if (!me || !me?.currentMainCompany) {
            return;
        }

        const checkHasPermitionToSave = me?.currentMainCompany.planType?.toString() !== '1';
        setHasPermitionToSave(checkHasPermitionToSave);

        if (!checkHasPermitionToSave) {
            swal({
                content: `O plano <b>grátis</b> não tem acesso às integrações do WhatsApp. Veja os planos do ${SYSTEM.NAME} clicando no botão abaixo.`,
                confirmBtnText: 'Ver planos',
                confirmFunction: () => router.push(ROUTES.EMPRESA_USO_E_PLANO),
                cancelBtnText: 'Voltar',
                icon: 'warning'
            });
        }
    }, [me, router]);

    const [integration, setIntegration] = useState<iIntegrationWhatsapp>();
    useApiRequestToSetterOnUrlChange<iIntegrationWhatsapp>({ apiUrlRequest: `${CONSTS_INTEGRATION_WHATSAPP.get}?companyId=${me?.currentMainCompany?.companyId ?? Guid.EMPTY}`, setter: setIntegration });

    const [formData, setFormData] = useState<iIntegrationWhatsapp>({
        messageReminderBeforeSchedule: '',
        messageBeforeScheduleAlert: '',
        messageOnScheduleConfirmed: '',
        messageOnScheduleCanceled: '',
        companyId: Guid.EMPTY
    });

    useEffect(() => {
        setTimeout(() => {
            setIsLoading(false);

            if (!integration || !me || !me?.currentMainCompany) {
                return;
            }

            setFormData(prev => ({
                ...prev,
                messageReminderBeforeSchedule: integration?.messageReminderBeforeSchedule,
                messageBeforeScheduleAlert: integration?.messageBeforeScheduleAlert,
                messageOnScheduleConfirmed: integration?.messageOnScheduleConfirmed,
                messageOnScheduleCanceled: integration?.messageOnScheduleCanceled,
                companyId: me?.currentMainCompany?.companyId
            }));
        }, 1000);
    }, [integration, me]);

    function handleReset() {
        swal({
            content: 'Você tem certeza que deseja redefinir todas as mensagens automáticas?',
            confirmBtnText: 'Sim',
            confirmFunction: () => {
                setFormData(prev => ({
                    ...prev,
                    messageReminderBeforeSchedule: 'Olá, {cliente}. Você tem um agendamento amanhã às {hora}. Nós, {empresa}, estamos te esperando!',
                    messageBeforeScheduleAlert: 'Olá, {cliente}. Seu agendamento em {data} às {hora} está chegando! Nós, {empresa}, estamos te esperando!',
                    messageOnScheduleConfirmed: 'Olá, {cliente}. Seu agendamento em {data} às {hora} foi confirmado! Nós, {empresa}, estamos te esperando!',
                    messageOnScheduleCanceled: 'Olá, {cliente}. Seu agendamento em {data} foi cancelado. Entre em contato para reagendar. Nós, {empresa}, estamos te esperando!',
                    companyId: me?.currentMainCompany?.companyId
                }));
            },
            cancelBtnText: 'Não, quero voltar',
            icon: 'question'
        });
    }

    async function handleSave() {
        const output = await Fetch.post({ url: CONSTS_INTEGRATION_WHATSAPP.post, body: formData });

        if (output) {
            swal({
                content: 'Configurações salvas com sucesso.',
                icon: 'success'
            });

            return;
        }

        return;
    }

    if (isLoading) {
        return (
            <TemplatePageHeader title='Carregando informações...' isLoading={true}></TemplatePageHeader>
        )
    }

    return (
        <TemplatePageHeader
            title='Integração com WhatsApp'
            actions={[
                me?.isUserAdmOfCurrentMainCompany && (
                    <Button
                        key='add'
                        label='Redefinir tudo'
                        handleFunction={() => handleReset()}
                        icon_feather={<Icon icon='rotate-ccw' size='small' />}
                        isStyleSimple={true}
                        isDisabled={!hasPermitionToSave}
                    />
                ),
                me?.isUserAdmOfCurrentMainCompany && (
                    <Button
                        key='add'
                        label='Salvar configurações de integração'
                        handleFunction={() => handleSave()}
                        icon_feather={<Icon icon='check' size='small' />}
                        isDisabled={!hasPermitionToSave}
                    />
                )
            ]}
        >
            <CardCreamWithChildren
                title={<span className={SYSTEM.ANIMATE_DELAY_0_5s}>{handleGetFirstName(me?.userName)}, {hasPermitionToSave ? 'deixe sua marca nas mensagens' : 'espera aí!'}</span>}
                subtitle={hasPermitionToSave ? <span>Para isso, <b>configure as mensagens padrão</b> que serão enviadas de forma automática aos seus clientes pelo WhatsApp.</span> : <span>O plano <b>grátis</b> não tem acesso às integrações do WhatsApp.</span>}
            >
                <div className={styles.container}>
                    <div className={styles.header}>
                        <h2>Mensagens automáticas</h2>
                    </div>

                    <div className={styles.grid}>
                        <div className={styles.card}>
                            <h3>Mensagem enviada um dia antes:</h3>
                            <textarea value={formData.messageReminderBeforeSchedule ?? ''} readOnly={!hasPermitionToSave} maxLength={512} onChange={(e) => setFormData((prev: typeof formData) => ({ ...prev, messageReminderBeforeSchedule: e.target.value }))} />
                        </div>

                        <div className={styles.card}>
                            <h3>Mensagem enviada pouco antes do horário do agendamento:</h3>
                            <textarea value={formData.messageBeforeScheduleAlert ?? ''} readOnly={!hasPermitionToSave} maxLength={512} onChange={(e) => setFormData((prev: typeof formData) => ({ ...prev, messageBeforeScheduleAlert: e.target.value }))} />
                        </div>

                        <div className={styles.card}>
                            <h3>Mensagem enviada quando o agendamento é confirmado:</h3>
                            <textarea value={formData.messageOnScheduleConfirmed ?? ''} readOnly={!hasPermitionToSave} maxLength={512} onChange={(e) => setFormData((prev: typeof formData) => ({ ...prev, messageOnScheduleConfirmed: e.target.value }))} />
                        </div>

                        <div className={styles.card}>
                            <h3>Mensagem enviada quando o agendamento é cancelado:</h3>
                            <textarea value={formData.messageOnScheduleCanceled ?? ''} readOnly={!hasPermitionToSave} maxLength={512} onChange={(e) => setFormData((prev: typeof formData) => ({ ...prev, messageOnScheduleCanceled: e.target.value }))} />
                        </div>

                        <div className={styles.tipSection}>
                            <Mascot
                                tippyContent='Segue essa dica!'
                                flip={true}
                                elementIfShowMascotIsFalse={<Icon icon='info' />}
                            />

                            <div>
                                Você pode usar as variáveis dinâmicas:
                                <code>&#123;cliente&#125;</code>,
                                <code>&#123;data&#125;</code>,
                                <code>&#123;hora&#125;</code>,
                                <code>&#123;empresa&#125;</code>.
                                Elas serão substituídas automaticamente pelas informações do agendamento.
                            </div>
                        </div>
                    </div>
                </div>
            </CardCreamWithChildren>
        </TemplatePageHeader>
    )
}