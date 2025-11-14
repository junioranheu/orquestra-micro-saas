import { iMe } from '@/app/api/consts/auth';
import ChatBot from '@/app/components/chat-bot';
import Icon from '@/app/components/icon';
import SYSTEM from '@/app/consts/system';
import { PACIFICO } from '@/app/fonts/fonts';
import { useShowChatbot, useShowExpandedSidebar } from '@/app/hooks/contexts/useGlobalContext';
import { useMenuGroups } from '@/app/hooks/useGetMenuGroups';
import Tippy from '@tippyjs/react';
import { usePathname, useRouter } from 'next/navigation';
import { Fragment, useEffect, useRef, useState } from 'react';
import styles from './index.module.scss';

interface iProps {
    me: iMe | undefined;
}

export default function Sidebar({ me }: iProps) {

    const router = useRouter();
    const pathname = usePathname();
    const menu = useMenuGroups({ me });

    const [active, setActive] = useState<string>('');
    const [showExpandedSidebar,] = useShowExpandedSidebar();
    const [showChatbot,] = useShowChatbot();

    const [openPopover, setOpenPopover] = useState<string | null>(null);
    const [popoverPos, setPopoverPos] = useState<{ top: number; left: number }>({ top: 0, left: 0 });
    const popoverRef = useRef<HTMLDivElement>(null);

    function handleClickOutside(e: MouseEvent) {
        if (popoverRef.current && !popoverRef.current.contains(e.target as Node)) {
            setOpenPopover(null);
        }
    }

    useEffect(() => {
        setActive(pathname);
        document.addEventListener('mousedown', handleClickOutside);
        return () => document.removeEventListener('mousedown', handleClickOutside);
    }, [pathname]);


    function handleGroupClick(e: React.MouseEvent, groupLabel: string) {
        const rect = (e.currentTarget as HTMLElement).getBoundingClientRect();
        setPopoverPos({ top: rect.top, left: rect.right + 0 }); // Posição ao lado;
        setOpenPopover(openPopover === groupLabel ? null : groupLabel);
    }

    return (
        <Fragment>
            <aside className={`${styles.sidebar} notSelectable`}>
                <h1>{showExpandedSidebar}</h1>

                <div className={styles.brand}>
                    <Icon icon='calendar' weight='bold' />
                    <span className={PACIFICO.className}>{SYSTEM.NAME}</span>
                </div>

                <nav className={SYSTEM.ANIMATE_DELAY_0_5s}>
                    {
                        menu?.map((group, gIndex) => {
                            const visibleItems = group.items.filter(x => x.hasAccess);

                            if (visibleItems.length === 0) {
                                return null;
                            }

                            const isExpanded = group.label === 'Geral' || group.label === 'Sistema' || showExpandedSidebar;

                            return (
                                <div key={gIndex} className={styles.group}>
                                    <div
                                        className={styles.groupHeader}
                                        onClick={isExpanded ? undefined : (e) => handleGroupClick(e, group.label)}
                                    >
                                        <span className={styles.groupLabel}>{group.label}</span>
                                        {!isExpanded && <Icon icon='chevron-right' />}
                                    </div>

                                    {
                                        isExpanded && (
                                            <ul>
                                                {
                                                    visibleItems.map((item, index) => (
                                                        <Tippy key={index} content={item.description} placement='right'>
                                                            <li
                                                                id={item.id}
                                                                className={active === item.route ? styles.active : ''}
                                                                onClick={() => router.push(item.route)}
                                                            >
                                                                <Icon icon={item.icon} />
                                                                <span>{item.label}</span>
                                                            </li>
                                                        </Tippy>
                                                    ))
                                                }
                                            </ul>
                                        )
                                    }
                                </div>
                            );
                        })
                    }
                </nav>

                {
                    openPopover && (
                        <div
                            ref={popoverRef}
                            className={styles.popover}
                            style={{ top: popoverPos.top, left: popoverPos.left }}
                        >
                            {
                                menu.find(g => g.label === openPopover)?.items.filter(x => x.hasAccess).map((item, index) => (
                                    <Tippy key={index} content={item.description} placement='right'>
                                        <div
                                            className={`${styles.popoverItem} ${active === item.route ? styles.active : ''}`}
                                            onClick={() => { router.push(item.route); setOpenPopover(null); }}
                                        >
                                            <Icon icon={item.icon} />
                                            <span>{item.label}</span>
                                        </div>
                                    </Tippy>
                                ))
                            }
                        </div>
                    )
                }
            </aside>

            {
                showChatbot && (
                    <ChatBot me={me} showButtonAbsolute={false} />
                )
            }
        </Fragment>
    )
}