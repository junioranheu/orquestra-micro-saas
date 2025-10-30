import Icon from '@/app/components/icon';
import Button from '@/app/components/input/button';
import Mascot from '@/app/components/mascot';
import ROUTES from '@/app/consts/routes';
import handleGetRandomNumber from '@/app/functions/get.randomNumber';
import useDisableScroll from '@/app/hooks/useDisableScroll';
import { useRouter } from 'next/navigation';
import styles from './index.module.scss';

interface iProps {
    variant?: 'error' | 'success' | 'warning' | 'info';
    code?: string;
    title?: string;
    description: string;
    showHelpPage?: boolean;
}

export default function LayoutTemplateOne({
    variant = 'error',
    code,
    title,
    description,
    showHelpPage: showSupportContact = false
}: iProps) {

    const router = useRouter();
    useDisableScroll();

    function handleGetIcon() {
        switch (variant) {
            case 'error':
                return (
                    <svg className={styles.icon} data-variant="error" xmlns='http://www.w3.org/2000/svg' viewBox='0 0 24 24' fill='none' stroke='currentColor' strokeWidth={2} strokeLinecap='round' strokeLinejoin='round'>
                        <circle cx='12' cy='12' r='10' />
                        <line x1='15' y1='9' x2='9' y2='15' />
                        <line x1='9' y1='9' x2='15' y2='15' />
                    </svg>
                );
            case 'success':
                return (
                    <svg className={styles.icon} data-variant="success" xmlns='http://www.w3.org/2000/svg' viewBox='0 0 24 24' fill='none' stroke='currentColor' strokeWidth={2} strokeLinecap='round' strokeLinejoin='round'>
                        <circle cx='12' cy='12' r='10' />
                        <path d='M9 12l2 2 4-4' />
                    </svg>
                );
            case 'warning':
                return (
                    <svg className={styles.icon} data-variant="warning" xmlns='http://www.w3.org/2000/svg' viewBox='0 0 24 24' fill='none' stroke='currentColor' strokeWidth={2} strokeLinecap='round' strokeLinejoin='round'>
                        <path d='M10.29 3.86L1.82 18a2 2 0 0 0 1.71 3h16.94a2 2 0 0 0 1.71-3L13.71 3.86a2 2 0 0 0-3.42 0z' />
                        <line x1='12' y1='9' x2='12' y2='13' />
                        <line x1='12' y1='17' x2='12.01' y2='17' />
                    </svg>
                );
            case 'info':
                return (
                    <svg className={styles.icon} data-variant="info" xmlns='http://www.w3.org/2000/svg' viewBox='0 0 24 24' fill='none' stroke='currentColor' strokeWidth={2} strokeLinecap='round' strokeLinejoin='round'>
                        <circle cx='12' cy='12' r='10' />
                        <line x1='12' y1='16' x2='12' y2='12' />
                        <line x1='12' y1='8' x2='12.01' y2='8' />
                    </svg>
                );
        }
    }

    return (
        <main className={styles.container}>
            <div className={styles.card}>
                <div className={styles.iconWrapper}>
                    {handleGetIcon()}
                </div>

                {
                    title && (
                        <div className={styles.divTitle}>
                            <Mascot
                                isCentralized={false}
                                tippyContent={
                                    <div style={{ fontSize: '0.8rem', padding: '0.5rem' }}>
                                        {title}
                                        {
                                            code && ` • código ${code}`
                                        }
                                    </div>
                                }
                                tippyPlacement='right'
                                flip={true}
                                flipPeriodic={true}
                                flipInterval={handleGetRandomNumber(4000, 7500)}
                            />

                            <h1 className={styles.title} dangerouslySetInnerHTML={{ __html: title }} />
                        </div>
                    )
                }

                <div className={styles.description} dangerouslySetInnerHTML={{ __html: description }} />

                <div className={styles.actions}>
                    <Button
                        label='Voltar ao início'
                        icon_feather={<Icon icon='home' />}
                        handleFunction={() => router.push(ROUTES.LOGIN)}
                        isBig={true}
                    />

                    {
                        showSupportContact && (
                            <Button
                                label='Central de ajuda'
                                icon_feather={<Icon icon='help-circle' />}
                                handleFunction={() => router.push(ROUTES.ETC_AJUDA)}
                                isStyleSimple={true}
                                isBig={true}
                            />
                        )
                    }
                </div>
            </div>
        </main>
    )
}