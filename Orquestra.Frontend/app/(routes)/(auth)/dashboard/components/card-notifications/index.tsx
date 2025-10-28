'use client';
import styles from '@/app/(routes)/(auth)/dashboard/components/card-daily-agenda/index.module.scss';
import { CONSTS_LOG, iLogNotificationOutput, iLogNotificationOutputPaginated } from '@/app/api/consts/log';
import { Fetch } from '@/app/api/fetch';
import { DATE_STYLE, handleFormatDate } from '@/app/functions/format.date';
import { useEffect, useState } from 'react';

export default function CardNotifications() {

    const [notifications, setNotifications] = useState<iLogNotificationOutput[]>([]);

    useEffect(() => {
        async function handleFetchNotifications() {
            const items = await Fetch.get({ url: `${CONSTS_LOG.getNotification}?isDashboard=true` }) as iLogNotificationOutputPaginated;

            console.log(items);
            setNotifications(items.output ?? []);
        }

        handleFetchNotifications();
    }, []);

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
        <div className={styles.dailyAgenda}>
            <h2 className={styles.title}>Notificações do sistema</h2>

            {
                notifications.length > 0 && (
                    <div className={styles.section}>
                        <h3 className={styles.sectionTitle}>Atualizado a cada 10 minutos</h3>
                        {notifications.map((notification) => handleRenderNotification(notification))}
                    </div>
                )
            }

            {
                notifications?.length === 0 && (
                    <div className={styles.empty}>
                        <p>Nenhum agendamento para hoje</p>
                    </div>
                )
            }
        </div>
    );
}