import Icon from '@/app/components/icon';
import SYSTEM from '@/app/consts/system';
import Link from 'next/link';
import Styles from './index.module.scss';

export default function Footer() {
    return (
        <footer className={Styles.footer}>
            <div className={Styles.wrapper}>
                <span>
                    Copyright © {new Date().getFullYear()} — <b>{SYSTEM.NAME}</b> — Desenvolvido por <Link href={SYSTEM.URL_GITHUB} target='_blank'>@junioranheu</Link>
                </span>

                <div className={Styles.right}>
                    <div className={Styles.icons}>
                        <Link title='GitHub' href={SYSTEM.URL_GITHUB} target='_blank'>
                            <Icon icon='github' color='var(--gray-dark)' />
                        </Link>

                        <Link title='LinkedIn' href={SYSTEM.URL_LINKEDIN} target='_blank'>
                            <Icon icon='linkedin' color='var(--gray-dark)' />
                        </Link>
                    </div>
                </div>
            </div>
        </footer>
    )
} 