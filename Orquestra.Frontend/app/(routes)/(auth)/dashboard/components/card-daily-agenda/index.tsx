'use client';
import { iMe } from '@/app/api/consts/auth';
import iSchedule, { CONSTS_SCHEDULE } from '@/app/api/consts/schedule';
import { Fetch } from '@/app/api/fetch';
import ImgThought from '@/app/assets/webp/thought.webp';
import ArrowUpRight from '@/app/components/arrow-up-right';
import { ContentLoaderCard } from '@/app/components/content-loader/card';
import WhatsappWebShortcut from '@/app/components/whatsapp/whatsapp-web-shortcut';
import ROUTES from '@/app/consts/routes';
import SYSTEM from '@/app/consts/system';
import { DATE_STYLE, handleFormatDate } from '@/app/functions/format.date';
import { handleGetDateBrazil, handleToBrazilDate } from '@/app/functions/get.date.brazil';
import { handleGetNameInitials } from '@/app/functions/get.formatUserName';
import { useFakeLoading } from '@/app/hooks/useFakeLoader';
import Image from 'next/image';
import { useEffect, useState } from 'react';
import styles from './index.module.scss';

interface iProps {
    me: iMe;
}

export default function CardDailyAgenda({ me }: iProps) {

    const now = handleGetDateBrazil();
    const [schedules, setSchedules] = useState<iSchedule[]>([]);
    const isLoading = useFakeLoading();

    useEffect(() => {
        async function handleFetchSchedules() {
            const items = await Fetch.get({ url: `${CONSTS_SCHEDULE.getAllByCompanyId}?companyId=${me?.currentMainCompany?.companyId}&getOnlyNearbyDates=true` }) as iSchedule[];
            setSchedules(items);
        }

        if (me?.currentMainCompany?.companyId) {
            handleFetchSchedules();
        }
    }, [me]);

    function handleCategorizeSchedules() {
        const current: iSchedule[] = [];
        const upcoming: iSchedule[] = [];
        const past: iSchedule[] = [];

        schedules.forEach((schedule) => {
            const startDate = handleToBrazilDate(schedule.dateStart);
            const endDate = handleToBrazilDate(schedule.dateEnd);

            if (now >= startDate && now <= endDate) {
                current.push(schedule);
            } else if (startDate > now) {
                upcoming.push(schedule);
            } else {
                past.push(schedule);
            }
        });

        return { current, upcoming, past };
    }

    function handleFormatTimeRange(start: string, end: string) {
        return `${start.substring(0, 5)} - ${end.substring(0, 5)}`;
    }

    function handleRenderScheduleItem(schedule: iSchedule, isActive = false) {
        return (
            <div key={schedule.scheduleId.toString()} className={`${styles.scheduleItem} ${isActive ? styles.active : ''}`}>
                <div className={`${styles.avatar} notSelectable`}>
                    {handleGetNameInitials(schedule.client?.fullName ?? '')}
                </div>

                <div className={styles.info}>
                    <div className={styles.name}>{schedule.client?.fullName}</div>

                    <div className={styles.service}>
                        <span>{schedule.customTitle || 'Agendamento'}{schedule.isRestrictForSpecificUsers ? ` • Agendamento específico para ${schedule?.usersIds?.length} colaborador${schedule?.usersIds?.length > 1 ? 'es' : ''}` : ''}</span>
                        <span>{handleFormatDate(schedule.dateStart, DATE_STYLE.DETALHADO_APENAS_REFERENCIA_DIA)}, {handleFormatTimeRange(schedule.timeStart, schedule.timeEnd)}</span>
                    </div>
                </div>

                <div className={styles.right}>
                    <WhatsappWebShortcut phone={schedule.client?.phone} message={schedule.messageIntegrationWhatsapp} clientId={schedule.clientId} />
                </div>
            </div>
        )
    }

    const { current, upcoming, past } = handleCategorizeSchedules();

    if (isLoading) {
        return (
            <ContentLoaderCard />
        )
    }

    return (
        <div
            className={`${styles.dailyAgenda} ${SYSTEM.ANIMATE}`}
            style={{ overflow: !schedules?.length ? 'hidden' : 'auto' }}
        >
            <h2 className={styles.title}>Resumo da agenda <ArrowUpRight href={ROUTES.EMPRESA_AGENDAMENTOS} tippyContent='Acessar agenda' /></h2>

            {
                current.length > 0 && (
                    <div className={styles.section}>
                        <h3 className={styles.sectionTitle}>Em andamento</h3>
                        {current.map((schedule) => handleRenderScheduleItem(schedule, true))}
                    </div>
                )
            }

            {
                upcoming.length > 0 && (
                    <div className={styles.section}>
                        <h3 className={styles.sectionTitle}>Próximos</h3>
                        {upcoming.map((schedule) => handleRenderScheduleItem(schedule))}
                    </div>
                )
            }

            {
                past.length > 0 && (
                    <div className={styles.section}>
                        <h3 className={styles.sectionTitle}>Anteriores</h3>
                        {past.map((schedule) => handleRenderScheduleItem(schedule))}
                    </div>
                )
            }

            {
                schedules?.length === 0 && (
                    <div className={styles.empty}>
                        <p>Nenhum agendamento para hoje</p>

                        <div className={styles.center}>
                            <Image src={ImgThought} alt='' priority={true} />
                        </div>
                    </div>
                )
            }
        </div>
    )
}