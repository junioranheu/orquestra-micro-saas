'use client';
import styles from '@/app/(routes)/(auth)/dashboard/components/card-daily-agenda/index.module.scss';
import { iMe } from '@/app/api/consts/auth';
import { CONSTS_LOG, iLogNotificationOutput, iLogNotificationOutputPaginated } from '@/app/api/consts/log';
import { Fetch } from '@/app/api/fetch';
import { ContentLoaderCard } from '@/app/components/content-loader/card';
import SYSTEM from '@/app/consts/system';
import { DATE_STYLE, handleFormatDate } from '@/app/functions/format.date';
import handleGetRandomNumber from '@/app/functions/get.randomNumber';
import Tippy from '@tippyjs/react';
import { useEffect, useState } from 'react';

interface iProps {
    me: iMe;
}

export default function CardNotifications({ me }: iProps) {

    const [notifications, setNotifications] = useState<iLogNotificationOutput[]>([]);
    const [isLoading, setIsLoading] = useState<boolean>(true);

    useEffect(() => {
        async function handleFetchNotifications() {
            const items = await Fetch.get({ url: `${CONSTS_LOG.getNotification}?isDashboard=true` }) as iLogNotificationOutputPaginated;
            setNotifications(items.output ?? []);

            setTimeout(() => {
                setIsLoading(false);
            }, handleGetRandomNumber(750, 1250));
        }

        handleFetchNotifications();
    }, []);

    if (isLoading) {
        return (
            <ContentLoaderCard />
        )
    }

    if (!notifications?.length) {
        return null;
    }

    function handleRenderNotification(notification: iLogNotificationOutput) {
        return (
            <div key={notification.logId.toString()} className={styles.scheduleItem}>
                <div className={styles.info}>
                    <div className={styles.name}>{notification.story}</div>

                    <div className={styles.service}>
                        {notification.description}
                    </div>
                </div>

                <div className={styles.time}>
                    {handleFormatDate(notification.date, DATE_STYLE.DETALHADO)}
                </div>
            </div>
        )
    }

    return (
        <div className={`${styles.dailyAgenda} ${SYSTEM.ANIMATE}`}>
            <Tippy content='Os dados são atualizados automaticamente a cada 10 minutos.'>
                <h2 className={styles.title} style={{ cursor: 'help' }}>Notificações do sistema</h2>
            </Tippy>

            {
                notifications.length > 0 && (
                    <div className={styles.section}>
                        <h3 className={styles.sectionTitle}>Resumo das últimas notificações da empresa {me?.currentMainCompany.name}</h3>
                        {notifications.map((notification) => handleRenderNotification(notification))}
                    </div>
                )
            }

            {
                notifications?.length === 0 && (
                    <div className={styles.empty}>
                        <p>Nenhuma notificação disponível</p>
                    </div>
                )
            }
        </div>
    )
}