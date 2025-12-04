'use client';
import styles from '@/app/(routes)/(auth)/dashboard/components/card-daily-agenda/index.module.scss';
import { iMe } from '@/app/api/consts/auth';
import { CONSTS_LOG, iLogNotificationOutput, iLogNotificationOutputPaginated } from '@/app/api/consts/log';
import { Fetch } from '@/app/api/fetch';
import ArrowUpRight from '@/app/components/arrow-up-right';
import { ContentLoaderCard } from '@/app/components/content-loader/card';
import Icon from '@/app/components/icon';
import ROUTES from '@/app/consts/routes';
import SYSTEM from '@/app/consts/system';
import { DATE_STYLE, handleFormatDate } from '@/app/functions/format.date';
import { useShowLogsDashboard } from '@/app/hooks/contexts/useGlobalContext';
import { useFakeLoading } from '@/app/hooks/useFakeLoader';
import Tippy from '@tippyjs/react';
import Link from 'next/link';
import { useEffect, useState } from 'react';

interface iProps {
    me: iMe;
}

export default function CardNotifications({ me }: iProps) {

    const [showLogsDashboard,] = useShowLogsDashboard();

    const [notifications, setNotifications] = useState<iLogNotificationOutput[]>([]);
    const isLoading = useFakeLoading();

    useEffect(() => {
        async function handleFetchNotifications() {
            const items = await Fetch.get({ url: `${CONSTS_LOG.getNotification}?isDashboard=true` }) as iLogNotificationOutputPaginated;
            setNotifications(items.output ?? []);
        }

        if (showLogsDashboard) {
            handleFetchNotifications();
        }
    }, [showLogsDashboard]);

    if (isLoading) {
        return (
            <ContentLoaderCard />
        )
    }

    if (!showLogsDashboard || !notifications?.length) {
        return null;
    }

    return (
        <div className={`${styles.dailyAgenda} ${SYSTEM.ANIMATE}`}>
            <h2 className={styles.title}>
                <span>Notificações</span>

                <Tippy
                    placement='top'
                    interactive={true}
                    content={
                        <div>
                            Os dados das notificações são atualizados automaticamente a cada 10 minutos.<br /><br />
                            Você pode ocultar este painel nas <Link href={ROUTES.USUARIO_CONFIGURACOES}>configurações</Link>.
                        </div>
                    }
                >
                    <span style={{ marginLeft: '0.035rem', cursor: 'help' }}>
                        <Icon icon='info' size='small' color='var(--main)' weight='bold' />
                    </span>
                </Tippy>

                <ArrowUpRight href={ROUTES.EMPRESA_NOTIFICACOES} tippyContent='Visualizar todas as notificações.' tippyPlacement='top' />
            </h2>

            {
                notifications.length > 0 && (
                    <div className={styles.section}>
                        <h3 className={styles.sectionTitle}>Resumo das notificações • {me?.currentMainCompany.name}.</h3>

                        {
                            notifications.map(notification => (
                                <NotificationItem key={notification.logId.toString()} notification={notification} />
                            ))
                        }
                    </div>
                )
            }

            {
                notifications?.length === 0 && (
                    <div className={styles.empty}>
                        <p>Nenhuma notificação disponível.</p>
                    </div>
                )
            }
        </div>
    )
}

function NotificationItem({ notification }: { notification: iLogNotificationOutput }) {

    const [open, setOpen] = useState<boolean>(false);

    const changed = typeof notification.changedFields === 'object'
        ? JSON.stringify(notification.changedFields, null, 2)
        : notification.changedFields;

    return (
        <div key={notification.logId.toString()} className={styles.scheduleItem}>
            <div className={styles.info} onClick={() => setOpen(!open)}>
                <Tippy content={(open ? `${notification.emoji} ${notification.logType}` : 'Visualizar detalhes')}>
                    <div className={styles.name} style={{ cursor: 'help' }}>
                        {notification.story}
                        <Icon icon='info' size='small' color='var(--gray-dark)' />
                    </div>
                </Tippy>

                <div className={styles.service}>
                    <button
                        className={styles.toggleJson}
                        onClick={() => setOpen(!open)}
                    >
                        {open ? 'Esconder detalhes' : ''}
                    </button>

                    {
                        open && (
                            <pre style={{
                                whiteSpace: 'pre-wrap',
                                wordWrap: 'break-word',
                                margin: 0,
                                marginTop: '0.5rem',
                                padding: '0.5rem',
                                background: '#f4f4f4',
                                borderRadius: '6px',
                                maxHeight: '300px',
                                overflowY: 'auto'
                            }}>
                                {changed}
                            </pre>
                        )
                    }
                </div>
            </div>

            <div className={styles.right}>
                {handleFormatDate(notification.date, DATE_STYLE.DETALHADO)}
            </div>
        </div>
    )
}