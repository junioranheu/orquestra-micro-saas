'use client';
import EmpresaClientesModalFollowUp from '@/app/(routes)/(auth)/empresa/clientes/modal/follow-up';
import EmpresaClientesModalView from '@/app/(routes)/(auth)/empresa/clientes/modal/view';
import { handleDisable } from '@/app/(routes)/(auth)/empresa/clientes/page';
import { iMe } from '@/app/api/consts/auth';
import { CONSTS_CLIENT, iClient } from '@/app/api/consts/client';
import { CONSTS_CLIENT_FOLLOW_UP, iClientFollowUp, iClientFollowUpPaginated } from '@/app/api/consts/client-follow-up';
import { CONSTS_SCHEDULE, iSchedule } from '@/app/api/consts/schedule';
import { Fetch } from '@/app/api/fetch';
import ImgThought from '@/app/assets/svg/thought.svg';
import Icon from '@/app/components/icon';
import Button from '@/app/components/input/button';
import TemplatePageHeader from '@/app/components/template/template-page-header';
import SYSTEM from '@/app/consts/system';
import { DATE_STYLE, handleFormatDate } from '@/app/functions/format.date';
import { handleGetFirstName } from '@/app/functions/get.formatUserName';
import { handleGuessGender } from '@/app/functions/get.guessGender';
import swal from '@/app/functions/swal';
import toast from '@/app/functions/toast';
import { handleOpenBase64InNewTab } from '@/app/functions/transform.base64';
import useApiGetEnum from '@/app/hooks/api/useApiGetEnum';
import useApiGetMe from '@/app/hooks/api/useApiGetMe';
import { useFakeLoading } from '@/app/hooks/useFakeLoader';
import useTitle from '@/app/hooks/useTitle';
import Image from 'next/image';
import { useParams } from 'next/navigation';
import { Dispatch, Fragment, SetStateAction, useEffect, useState } from 'react';
import Swal from 'sweetalert2';
import styles from './page.module.scss';

// Interfaces
interface iClientHeaderProps {
    me: iMe | undefined;
    name: string;
    memberSince: string;
    onEdit: () => void;
    handleOpenModalFollowUp: () => void;
    client: iClient;
    setTrigger: Dispatch<SetStateAction<Date>>;
}

interface iContactInfoProps {
    email: string;
    phone: string;
    birthDate: string;
    address: string;
}

interface iAppointmentHistoryProps {
    schedules: iSchedule[];
}

interface iFollowUpHistoryProps {
    me: iMe | undefined;
    clientsFollowUps: iClientFollowUp[];
    handleOpenModalFollowUp: (followUp: iClientFollowUp | undefined, type: 'edit' | 'create') => void;
    setTrigger: Dispatch<SetStateAction<Date>>;
}

// Componente principal;
export default function ClientProfile() {

    useTitle('Detalhes');

    const me = useApiGetMe({});
    const isLoading = useFakeLoading();
    const params = useParams();
    const query = params.clientId;

    const [client, setClient] = useState<iClient | null>();
    const [schedules, setSchedules] = useState<iSchedule[]>([]);
    const [clientsFollowUps, setClientsFollowUps] = useState<iClientFollowUpPaginated>();
    const [trigger, setTrigger] = useState<Date>(new Date());

    useEffect(() => {
        async function handleFetch() {
            const clientId = query?.toString();

            const [client, schedules, clientsFollowUps] = await Promise.all([
                Fetch.get({ url: `${CONSTS_CLIENT.get}?clientId=${clientId}` }) as Promise<iClient>,
                Fetch.get({ url: `${CONSTS_SCHEDULE.getAllByClientId}?companyId=${clientId}&clientId=${clientId}` }) as Promise<iSchedule[]>,
                Fetch.get({ url: `${CONSTS_CLIENT_FOLLOW_UP.get}?clientId=${clientId}` }) as Promise<iClientFollowUpPaginated>
            ]);

            setClient(client);
            setSchedules(schedules);
            setClientsFollowUps(clientsFollowUps);
        }

        handleFetch();
    }, [query, trigger]);

    const [isModalViewOpen, setIsModalViewOpen] = useState<boolean>(false);
    const [clientClicked, setClientClicked] = useState<iClient | undefined>(undefined);
    const [typeModal, setTypeModal] = useState<('edit')>('edit');

    function handleOpenModalView(client: iClient | undefined) {
        setTypeModal('edit');
        setClientClicked(client);
        setIsModalViewOpen(true);
    }

    const [isModalFollowUpOpen, setIsModalFollowUpOpen] = useState<boolean>(false);
    const [followUpClicked, setFollowUpClicked] = useState<iClientFollowUp | undefined>(undefined);
    const [typeModalFollowUp, setTypeModalFollowUp] = useState<('create' | 'edit')>('create');

    function handleOpenModalFollowUp(followUp: iClientFollowUp | undefined, type: 'create' | 'edit') {
        setTypeModalFollowUp(type);
        setFollowUpClicked(followUp);
        setIsModalFollowUpOpen(true);
    }

    if (!client || isLoading) {
        return (
            <TemplatePageHeader title='Carregando informações do cliente...' isLoading={true}></TemplatePageHeader>
        )
    }

    return (
        <Fragment>
            <TemplatePageHeader title={`Detalhes • ${handleGetFirstName(client.fullName)}`}>
                <div className={styles.clientProfile}>
                    <div className={styles.clientProfile__container}>
                        <ClientHeader
                            me={me}
                            name={client?.fullName}
                            memberSince={handleFormatDate(client?.createdDate, DATE_STYLE.DIA_DA_SEMANA_E_DIA_DO_MES_E_ANO)}
                            onEdit={() => handleOpenModalView(client)}
                            handleOpenModalFollowUp={() => handleOpenModalFollowUp(undefined, 'create')}
                            client={client}
                            setTrigger={setTrigger}
                        />

                        <div className={styles.clientProfile__grid}>
                            <div className={styles.clientProfile__sidebar}>
                                <ContactInfo
                                    email={client?.email ? client?.email : '-'}
                                    phone={client?.phone ? client?.phone : '-'}
                                    birthDate={client?.dateOfBirth ? handleFormatDate(client?.dateOfBirth ? new Date(new Date(client.dateOfBirth).getTime() + 3 * 60 * 60 * 1000) : '-', DATE_STYLE.DIA_MES_ANO) : '-'}
                                    address={client?.address ? client?.address : '-'}
                                />

                                <div className={`${styles.card} ${styles.internalNotes}`}>
                                    <h2 className={styles.card__title}>Notas internas</h2>
                                    <p className={styles.internalNotes__text}>{client?.notes ? client?.notes : 'Nenhuma anotação no momento.'}</p>
                                </div>

                                <AppointmentHistory schedules={schedules} />
                            </div>

                            <div className={styles.clientProfile__main}>
                                <FollowUpHistory
                                    me={me}
                                    clientsFollowUps={clientsFollowUps?.output ?? []}
                                    handleOpenModalFollowUp={handleOpenModalFollowUp}
                                    setTrigger={setTrigger}
                                />
                            </div>
                        </div>
                    </div>
                </div>
            </TemplatePageHeader>

            <EmpresaClientesModalView
                isModalOpen={isModalViewOpen}
                setIsModalOpen={setIsModalViewOpen}
                type={typeModal}
                client={clientClicked}
                companyId={me?.currentMainCompany?.companyId}
                setTrigger={setTrigger}
            />

            <EmpresaClientesModalFollowUp
                isModalOpen={isModalFollowUpOpen}
                setIsModalOpen={setIsModalFollowUpOpen}
                type={typeModalFollowUp}
                clientId={client?.clientId}
                followUpClicked={followUpClicked}
                setTrigger={setTrigger}
            />
        </Fragment>
    )
}

// Componente de Header do Cliente;
function ClientHeader({ me, name, memberSince, onEdit, handleOpenModalFollowUp, client, setTrigger }: iClientHeaderProps) {

    const [emoji, setEmoji] = useState<string>('👤');

    useEffect(() => {
        async function handleGetEmoji() {
            const gender = await handleGuessGender(handleGetFirstName(name));

            if (gender) {
                setEmoji(gender === 'F' ? '👩' : '👨');
            }
        }

        handleGetEmoji();
    }, [name]);

    return (
        <div className={styles.clientHeader}>
            <div className={styles.clientHeader__info}>
                <div className={`${styles.clientHeader__avatar} notSelectable`}>
                    <span className={`${styles.clientHeader__avatarEmoji} ${SYSTEM.ANIMATE_DELAY_0_5s}`}>{emoji}</span>
                </div>

                <div className={styles.clientHeader__details}>
                    <h1 className={styles.clientHeader__name}>{name}</h1>
                    <p className={styles.clientHeader__since}>Cliente desde {memberSince}</p>
                </div>
            </div>

            <div className={styles.clientHeader__actions}>
                {me?.isUserAdmOfCurrentMainCompany && <Button label='Remover cliente' handleFunction={() => handleDisable(client, setTrigger)} isStyleSimple={true} style={{ background: 'var(--white-og)' }} icon_feather={<Icon icon='user-x' />} />}
                <Button label='Criar novo acompanhamento' handleFunction={() => handleOpenModalFollowUp()} isStyleSimple={true} style={{ background: 'var(--white-og)' }} icon_feather={<Icon icon='repeat' />} />
                <Button label='Editar cliente' handleFunction={() => onEdit()} icon_feather={<Icon icon='edit' />} />
            </div>
        </div>
    )
}

// Componente de Informações de Contato;
function ContactInfo({ email, phone, birthDate, address }: iContactInfoProps) {
    return (
        <div className={`${styles.card} ${styles.contactInfo}`}>
            <h2 className={styles.card__title}>Informações de contato</h2>

            <div className={styles.contactInfo__list}>
                <div className={styles.contactInfo__item}>
                    <Icon icon='mail' className={styles.contactInfo__icon} />
                    <div className={styles.contactInfo__content}>
                        <p className={styles.contactInfo__label}>E-mail</p>
                        <p className={styles.contactInfo__value}>{email}</p>
                    </div>
                </div>

                <div className={styles.contactInfo__item}>
                    <Icon icon='phone' className={styles.contactInfo__icon} />
                    <div className={styles.contactInfo__content}>
                        <p className={styles.contactInfo__label}>Telefone</p>
                        <p className={styles.contactInfo__value}>{phone}</p>
                    </div>
                </div>

                <div className={styles.contactInfo__item}>
                    <Icon icon='calendar' className={styles.contactInfo__icon} />
                    <div className={styles.contactInfo__content}>
                        <p className={styles.contactInfo__label}>Data de nascimento</p>
                        <p className={styles.contactInfo__value}>{birthDate}</p>
                    </div>
                </div>

                <div className={styles.contactInfo__item}>
                    <Icon icon='home' className={styles.contactInfo__icon} />
                    <div className={styles.contactInfo__content}>
                        <p className={styles.contactInfo__label}>Endereço</p>
                        <p className={styles.contactInfo__value}>{address}</p>
                    </div>
                </div>
            </div>
        </div>
    )
}

// Componente de Histórico de Agendamentos;
function AppointmentHistory({ schedules }: iAppointmentHistoryProps) {

    const scheduleStatusEnum = useApiGetEnum({ enumName: 'ScheduleStatusEnum' });

    return (
        <div className={`${styles.card} ${styles.appointmentHistory}`}>
            <h2 className={styles.card__title}>Histórico de agendamentos</h2>

            <div
                className={styles.appointmentHistory__list}
                style={{ maxHeight: '50vh' }}
            >
                {
                    schedules?.length ? schedules.map((schedule, index) => (
                        <div key={index} className={`${styles.appointmentItem} ${styles[`appointmentItem--${schedule.scheduleStatus}`]}`}>
                            <div className={styles.appointmentItem__content}>
                                <div className={styles.appointmentItem__info}>
                                    <h3 className={styles.appointmentItem__title}>
                                        {schedule.customTitle ? schedule.customTitle : `Agendamento para ${handleFormatDate(schedule.dateStart, DATE_STYLE.DIA_MES_ANO)}`}
                                    </h3>

                                    <p className={styles.appointmentItem__datetime}>
                                        {handleFormatDate(schedule.dateStart, DATE_STYLE.DETALHADO)}
                                    </p>
                                </div>

                                <div className={styles.appointmentItem__meta}>
                                    <span className={styles.appointmentItem__price}>
                                        R$ {schedule.amountReceived ?? 0}
                                    </span>

                                    <span className={`${styles.appointmentItem__status} ${styles[`appointmentItem__status--${schedule.scheduleStatus}`]}`}>
                                        {scheduleStatusEnum?.find(x => x.value === schedule.scheduleStatus)?.label?.toString() ?? ''}
                                    </span>
                                </div>
                            </div>
                        </div>
                    )) : (
                        <p className={styles.appointmentHistory__empty}>
                            Nenhum agendamento encontrado.
                        </p>
                    )
                }
            </div>
        </div>
    )
}

// Componente de Histórico de Follow-ups;
function FollowUpHistory({ me, clientsFollowUps, handleOpenModalFollowUp, setTrigger }: iFollowUpHistoryProps) {

    const clientFollowUpStatusEnum = useApiGetEnum({ enumName: 'ClientFollowUpStatusEnum' });

    function handleOpenFiles(followUp: iClientFollowUp) {
        const imagesHtml = followUp.imagesBase64?.map((base64, idx) => `
        <div style="display:flex;flex-direction:column;align-items:center;margin:6px;">
            <img 
                src="${base64}" 
                style="
                    width: 140px;
                    height: 140px;
                    object-fit: cover;
                    border-radius: var(--border-radius-xs);
                    box-shadow: 0 2px 6px rgba(0,0,0,0.15);
                "
            />
            <button id="btn-open-${idx}" style="
                margin-top: 8px !important;
                padding: 8px !important;
                border: 1px solid var(--gray) !important;
                border-radius: 4px !important;
                background: transparent !important;
                color: var(--gray-dark) !important;
                cursor: pointer !important;
                box-shadow: none !important;
                font-family: inherit !important;
            ">Abrir em outra aba</button>
        </div>
    `).join('') ?? '';

        Swal.fire({
            // title: 'Anexos',
            html: `<div style="display:flex;flex-wrap:wrap;justify-content:center;">${imagesHtml}</div>`,
            width: 700,
            showConfirmButton: true,
            confirmButtonText: 'Voltar',
            didOpen: () => {
                followUp.imagesBase64?.forEach((base64, idx) => {
                    const btn = document.getElementById(`btn-open-${idx}`);
                    btn?.addEventListener('click', () => handleOpenBase64InNewTab(base64));
                });
            }
        });
    }

    async function handleDisable(followUp: iClientFollowUp) {
        swal({
            content: 'Você tem certeza que deseja remover este acompanhamento? Este processo é irreversível.',
            confirmBtnText: 'Sim, desejo remover',
            mustConfirm: true,
            checkboxLabel: 'Sim, confirmo',
            confirmFunction: async () => {
                const output = await Fetch.put({ url: `${CONSTS_CLIENT_FOLLOW_UP.disable}?clientFollowUpId=${followUp.clientFollowUpId}` });

                if (output) {
                    toast({ content: 'Acompanhamento removido com sucesso.' });
                    setTrigger(new Date());
                    return;
                }

                toast({ content: 'Não foi possível remover este acompanhamento. Tente novamente mais tarde.' });
            },
            cancelBtnText: 'Voltar',
            icon: 'question'
        });
    }

    return (
        <div className={`${styles.card} ${styles.appointmentHistory}`}>
            <h2 className={styles.card__title}>Acompanhamentos</h2>

            <div
                className={styles.appointmentHistory__list}
                style={{ maxHeight: '80vh' }}
            >
                {
                    clientsFollowUps?.length ? clientsFollowUps.map((followUp, index) => (
                        <div key={index} className={styles.appointmentItem}>
                            <div className={styles.appointmentItem__content}>
                                <div className={styles.appointmentItem__info}>
                                    <h3 className={styles.appointmentItem__title}>
                                        {handleFormatDate(followUp.createdDate, DATE_STYLE.DETALHADO)}
                                    </h3>

                                    <p className={styles.appointmentItem__datetime}>
                                        {followUp.observation}
                                    </p>
                                </div>

                                <div
                                    className={styles.appointmentItem__meta}
                                    style={{ flexDirection: 'column' }}
                                >
                                    <span className={`${styles.appointmentItem__status_follow_up} ${styles[`appointmentItem__status_follow_up--${followUp.clientFollowUpStatus}`]}`}>
                                        {clientFollowUpStatusEnum?.find(x => x.value === followUp.clientFollowUpStatus)?.label?.toString() ?? ''}
                                    </span>

                                    {
                                        followUp.imagesBase64?.length ? (
                                            <Fragment>
                                                <span
                                                    className={`${styles.appointmentItem__status_follow_up} ${styles[`appointmentItem__status_follow_up--999`]}`}
                                                    onClick={() => handleOpenFiles(followUp)}
                                                >
                                                    <Icon icon='paperclip' size='small' /> {followUp.imagesBase64?.length > 1 ? 'Ver anexos' : 'Ver anexo'}
                                                </span>
                                            </Fragment>
                                        ) : (
                                            <Fragment></Fragment>
                                        )
                                    }

                                    {
                                        // clientFollowUpStatus 1 = Em progresso
                                        followUp?.clientFollowUpStatus?.toString() === '1' && (
                                            <span
                                                className={`${styles.appointmentItem__status_follow_up} ${styles[`appointmentItem__status_follow_up--999`]}`}
                                                onClick={() => handleOpenModalFollowUp(followUp, 'edit')}
                                            >
                                                <Icon icon='edit' size='small' /> Editar
                                            </span>
                                        )
                                    }

                                    {
                                        me?.isUserAdmOfCurrentMainCompany && (
                                            <span
                                                className={`${styles.appointmentItem__status_follow_up} ${styles[`appointmentItem__status_follow_up--999`]}`}
                                                onClick={() => handleDisable(followUp)}
                                            >
                                                <Icon icon='trash' size='small' /> Excluir
                                            </span>
                                        )
                                    }
                                </div>
                            </div>
                        </div>
                    )) : (
                        <div className={styles.appointmentHistory__empty}>
                            <p>
                                Nenhum acompanhamento encontrado. Adicione um novo registro clicando no botão &quot;Criar novo acompanhamento&quot; acima.
                            </p>

                            <div className={styles.center}>
                                <Image src={ImgThought} alt='' priority={true} />
                            </div>
                        </div>
                    )
                }
            </div>
        </div>
    )
}