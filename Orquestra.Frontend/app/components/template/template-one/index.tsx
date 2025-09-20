import Icon from '@/app/components/icon';
import Button from '@/app/components/input/button';
import ROUTES from '@/app/consts/routes';
import SYSTEM from '@/app/consts/system';
import useDisableScroll from '@/app/hooks/useDisableScroll';
import { useRouter } from 'next/navigation';
import styles from './index.module.scss';

interface iProps {
    svg?: 'error' | 'success';
    code?: string;
    title?: string;
    description: string;
    showSupportContact: boolean;
    isCentralized: boolean;
}

export default function LayoutTemplateOne({ svg, code, title, description, showSupportContact, isCentralized }: iProps) {

    const router = useRouter();
    useDisableScroll();

    return (
        <main
            className={styles.container}
            style={isCentralized ? { alignItems: 'center' } : { alignItems: 'flex-start' }}
        >
            <div className={styles.inner}>
                <div className={styles.iconWrapper}>
                    {
                        svg === 'error' && (
                            <svg className={styles.errorIcon} xmlns='http://www.w3.org/2000/svg' viewBox='0 0 24 24' fill='none' stroke='currentColor' strokeWidth={1.5} strokeLinecap='round' strokeLinejoin='round'>
                                <circle cx='12' cy='12' r='10' />
                                <line x1='4.93' y1='4.93' x2='19.07' y2='19.07' />
                            </svg>
                        )
                    }

                    {
                        svg === 'success' && (
                            <svg className={styles.successIcon} xmlns='http://www.w3.org/2000/svg' viewBox='0 0 24 24' fill='none' stroke='currentColor' strokeWidth={1.5} strokeLinecap='round' strokeLinejoin='round'>
                                <circle cx='12' cy='12' r='10' />
                                <path d='M9 12l2 2 4-4' />
                            </svg>
                        )
                    }
                </div>

                {code && <h1 className={styles.code}>{code}</h1>}
                {title && <h2 className={styles.title} dangerouslySetInnerHTML={{ __html: title }} />}

                <p className={styles.description} dangerouslySetInnerHTML={{ __html: description }} />

                <div className={styles.actions}>
                    <Button
                        label='Voltar ao início'
                        icone_feather={<Icon icon='home' />}
                        handleFunction={() => router.push(ROUTES.CRIAR_CONTA)}
                        isBig={true}
                    />

                    {
                        showSupportContact && (
                            <Button
                                label='Contatar suporte'
                                icone_feather={<Icon icon='mail' />}
                                handleFunction={() => window.location.href = `mailto:${SYSTEM.EMAIL_SUPPORT}`}
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