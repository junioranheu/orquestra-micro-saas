'use client';
import EmpresaClientesModalView from '@/app/(routes)/(auth)/empresa/clientes/modal/view';
import { handleDisable } from '@/app/(routes)/(auth)/empresa/clientes/page';
import { iMe } from '@/app/api/consts/auth';
import iClient, { CONSTS_CLIENT } from '@/app/api/consts/client';
import iSchedule, { CONSTS_SCHEDULE } from '@/app/api/consts/schedule';
import { Fetch } from '@/app/api/fetch';
import Icon from '@/app/components/icon';
import Button from '@/app/components/input/button';
import TemplatePageHeader from '@/app/components/template/template-page-header';
import ROUTES from '@/app/consts/routes';
import SYSTEM from '@/app/consts/system';
import { DATE_STYLE, handleFormatDate } from '@/app/functions/format.date';
import { handleGetFirstName } from '@/app/functions/get.formatUserName';
import { handleGuessGender } from '@/app/functions/get.guessGender';
import useApiGetEnum from '@/app/hooks/api/useApiGetEnum';
import useApiGetMe from '@/app/hooks/api/useApiGetMe';
import useTitle from '@/app/hooks/useTitle';
import { useParams, useRouter } from 'next/navigation';
import { Dispatch, Fragment, SetStateAction, useEffect, useState } from 'react';
import styles from './page.module.scss';

// Interfaces
interface iClientHeaderProps {
    me: iMe | undefined;
    name: string;
    memberSince: string;
    onEdit: () => void;
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

// Componente Principal
export default function ClientProfile() {

    useTitle('Detalhes');

    const router = useRouter();
    const params = useParams();
    const query = params.clientId;

    const [client, setClient] = useState<iClient | null>();
    const [schedules, setSchedules] = useState<iSchedule[]>([]);
    const [trigger, setTrigger] = useState<Date>(new Date());

    useEffect(() => {
        async function handleFetch() {
            const clientId = query?.toString();

            const client = await Fetch.get({ url: `${CONSTS_CLIENT.get}?clientId=${clientId}` }) as iClient;
            const schedules = await Fetch.get({ url: `${CONSTS_SCHEDULE.getAllByClientId}?companyId=${client.companyId}&clientId=${clientId}` }) as iSchedule[];

            setTimeout(() => {
                setClient(client);
                setSchedules(schedules);
            }, 1000);
        }

        handleFetch();
    }, [query, router, trigger]);

    const me = useApiGetMe({});

    const [isModalViewOpen, setIsModalViewOpen] = useState<boolean>(false);
    const [clientClicked, setClientClicked] = useState<iClient | undefined>(undefined);
    const [typeModal, setTypeModal] = useState<('edit')>('edit');

    function handleOpenModalView(client: iClient | undefined) {
        setTypeModal('edit');
        setClientClicked(client);
        setIsModalViewOpen(true);
    }

    if (!client) {
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
                            </div>

                            <div className={styles.clientProfile__main}>
                                <AppointmentHistory schedules={schedules} />
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
        </Fragment>
    )
}

// Componente de Header do Cliente
function ClientHeader({ me, name, memberSince, onEdit, client, setTrigger }: iClientHeaderProps) {

    const router = useRouter();
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
                <Button label='Acompanhamento' handleFunction={() => router.push(`${ROUTES.EMPRESA_ACOMPANHAMENTO}/${client.clientId}`)} icon_feather={<Icon icon='repeat' />} />
                <Button label='Editar cliente' handleFunction={() => onEdit()} icon_feather={<Icon icon='edit' />} />
            </div>
        </div>
    )
}

// Componente de Informações de Contato
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

// Componente de Histórico de Agendamentos
function AppointmentHistory({ schedules }: iAppointmentHistoryProps) {

    const scheduleStatusEnum = useApiGetEnum({ enumName: 'ScheduleStatusEnum' });

    return (
        <div className={`${styles.card} ${styles.appointmentHistory}`}>
            <h2 className={styles.card__title}>Histórico de agendamentos</h2>

            <div className={styles.appointmentHistory__list}>
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