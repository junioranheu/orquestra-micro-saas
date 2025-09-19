import Icon from '@/app/components/icon';
import SYSTEM from '@/app/consts/system';
import Tippy from '@tippyjs/react';
import Link from 'next/link';
import Styles from './index.module.scss';

export default function Footer() {
    return (
        <footer className={Styles.footer}>
            <div className={Styles.wrapper}>
                <span>
                    Copyright © {new Date().getFullYear()} — {SYSTEM.NAME} — Desenvolvido por <Tippy content='LinkedIn'><Link href={SYSTEM.URL_LINKEDIN} target='_blank'>@junioranheu</Link></Tippy>
                </span>

                <div className={Styles.right}>
                    <div className={Styles.icons}>
                        <Tippy content='GitHub'>
                            <Link href={SYSTEM.URL_GITHUB} target='_blank'>
                                <Icon icon='github' color='var(--gray-dark)' />
                            </Link>
                        </Tippy>

                        <Tippy content='LinkedIn'>
                            <Link href={SYSTEM.URL_LINKEDIN} target='_blank'>
                                <Icon icon='linkedin' color='var(--gray-dark)' />
                            </Link>
                        </Tippy>
                    </div>
                </div>
            </div>
        </footer>
    )
} 