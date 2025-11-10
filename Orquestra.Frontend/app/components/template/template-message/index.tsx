import Icon from '@/app/components/icon';
import Button from '@/app/components/input/button';
import ROUTES from '@/app/consts/routes';
import useDisableScroll from '@/app/hooks/useDisableScroll';
import { useRouter } from 'next/navigation';
import { useEffect, useState } from 'react';
import styles from './index.module.scss';

interface iProps {
    variant?: 'error' | 'success' | 'warning' | 'info';
    code?: string;
    title?: string;
    description: string;
    showHelpPage?: boolean;
}

export default function LayoutTemplateMessage({
    variant = 'error',
    code,
    title,
    description,
    showHelpPage = false
}: iProps) {

    const router = useRouter();
    const [isVisible, setIsVisible] = useState(false);

    useDisableScroll();

    useEffect(() => {
        const timer = setTimeout(() => setIsVisible(true), 100);
        return () => clearTimeout(timer);
    }, []);

    function handleGetIcon() {
        const iconProps = {
            className: styles.icon,
            'data-variant': variant,
            xmlns: 'http://www.w3.org/2000/svg',
            viewBox: '0 0 24 24',
            fill: 'none',
            stroke: 'currentColor',
            strokeWidth: 2,
            strokeLinecap: 'round' as const,
            strokeLinejoin: 'round' as const
        };

        switch (variant) {
            case 'error':
                return (
                    <svg {...iconProps}>
                        <circle cx='12' cy='12' r='10' />
                        <line x1='15' y1='9' x2='9' y2='15' />
                        <line x1='9' y1='9' x2='15' y2='15' />
                    </svg>
                );

            case 'success':
                return (
                    <svg {...iconProps}>
                        <circle cx='12' cy='12' r='10' />
                        <path d='M9 12l2 2 4-4' />
                    </svg>
                );

            case 'warning':
                return (
                    <svg {...iconProps}>
                        <path d='M10.29 3.86L1.82 18a2 2 0 0 0 1.71 3h16.94a2 2 0 0 0 1.71-3L13.71 3.86a2 2 0 0 0-3.42 0z' />
                        <line x1='12' y1='9' x2='12' y2='13' />
                        <line x1='12' y1='17' x2='12.01' y2='17' />
                    </svg>
                );

            case 'info':
                return (
                    <svg {...iconProps}>
                        <circle cx='12' cy='12' r='10' />
                        <line x1='12' y1='16' x2='12' y2='12' />
                        <line x1='12' y1='8' x2='12.01' y2='8' />
                    </svg>
                );
        }
    }

    return (
        <main className={styles.container}>
            <div className={`${styles.card} ${isVisible ? styles.visible : ''}`} data-variant={variant}>
                <div className={styles.iconWrapper}>
                    <div className={styles.iconBackground} data-variant={variant}>
                        {handleGetIcon()}
                    </div>
                </div>

                {
                    code && (
                        <span className={styles.code} data-variant={variant}>
                            {code}
                        </span>
                    )
                }

                {
                    title && (
                        <div className={styles.divTitle}>
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

                    {showHelpPage && (
                        <Button
                            label='Central de ajuda'
                            icon_feather={<Icon icon='help-circle' />}
                            handleFunction={() => router.push(ROUTES.ETC_AJUDA)}
                            isStyleSimple={true}
                            isBig={true}
                        />
                    )}
                </div>
            </div>
        </main>
    )
}