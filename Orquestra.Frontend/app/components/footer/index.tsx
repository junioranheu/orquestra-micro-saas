import Icon from '@/app/components/icon';
import WhatsappHyperlink from '@/app/components/whatsapp/hyperlink';
import ROUTES from '@/app/consts/routes';
import SYSTEM from '@/app/consts/system';
import Tippy from '@tippyjs/react';
import Link from 'next/link';
import styles from './index.module.scss';

interface iProps {
    resetBorderRadius?: boolean;
}

export default function Footer({ resetBorderRadius = false }: iProps) {
    return (
        <footer
            className={styles.footer}
            style={resetBorderRadius ? { borderRadius: 0 } : {}}
        >
            <div className={styles.wrapper}>
                <span className={styles.content}>
                    <span>{SYSTEM.NAME}: {SYSTEM.DESCRIPTION}.</span>
                    <span>Todos os direitos reservados © {new Date().getFullYear()} — Desenvolvido e publicado por <Tippy content='LinkedIn'><Link href={SYSTEM.URL_LINKEDIN} target='_blank'>@junioranheu</Link></Tippy>.</span>
                </span>

                <div className={styles.right}>
                    <div className={styles.icons}>
                        <Tippy content={`Página de apresentação do ${SYSTEM.NAME}`}>
                            <Link href={ROUTES.LANDING_PAGE}>
                                <Icon icon='smile' color='var(--gray-dark)' className='contrastOnHover' />
                            </Link>
                        </Tippy>

                        <Tippy content='Contatar suporte via WhatsApp'>
                            <span>
                                <WhatsappHyperlink showIcon={true} />
                            </span>
                        </Tippy>

                        <Tippy content='Contatar suporte via e-mail'>
                            <Link
                                href='#'
                                onClick={(e) => {
                                    e.preventDefault();
                                    window.location.href = `mailto:${SYSTEM.EMAIL_SUPPORT}`;
                                }}
                            >
                                <Icon icon='mail' color='var(--gray-dark)' className='contrastOnHover' />
                            </Link>
                        </Tippy>

                        <Tippy content={`GitHub ${SYSTEM.AUTHOR}`}>
                            <Link href={SYSTEM.URL_GITHUB} target='_blank' rel='noopener noreferrer'>
                                <Icon icon='github' color='var(--gray-dark)' className='contrastOnHover' />
                            </Link>
                        </Tippy>

                        <Tippy content={`LinkedIn ${SYSTEM.AUTHOR}`}>
                            <Link href={SYSTEM.URL_LINKEDIN} target='_blank' rel='noopener noreferrer'>
                                <Icon icon='linkedin' color='var(--gray-dark)' className='contrastOnHover' />
                            </Link>
                        </Tippy>
                    </div>
                </div>
            </div>
        </footer>
    )
} 