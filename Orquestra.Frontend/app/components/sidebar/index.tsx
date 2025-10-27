import Icon from '@/app/components/icon';
import { MODULES } from '@/app/consts/modules';
import ROUTES from '@/app/consts/routes';
import SYSTEM from '@/app/consts/system';
import { PACIFICO } from '@/app/fonts/fonts';
import { handleCheckShowElement } from '@/app/functions/check.permission';
import useApiGetMe from '@/app/hooks/api/useApiGetMe';
import feather from 'feather-icons';
import { usePathname, useRouter } from 'next/navigation';
import { useEffect, useState } from 'react';
import styles from './index.module.scss';

interface iMenuItem {
    label: string;
    icon: keyof typeof feather.icons;
    route: (typeof ROUTES)[keyof typeof ROUTES];
    hasAccess: boolean;
}

export default function Sidebar() {

    const router = useRouter();
    const pathname = usePathname();
    const me = useApiGetMe({});
    const [active, setActive] = useState<string>('');

    const menuItems = [
        { label: 'Início', icon: 'home', route: ROUTES.DASHBOARD, hasAccess: true },
        { label: 'Agenda', icon: 'calendar', route: ROUTES.EMPRESA_AGENDAMENTOS, hasAccess: handleCheckShowElement({ me, rolesRequired: [MODULES.Scheduling] }) },
        { label: 'Colaboradores', icon: 'users', route: ROUTES.EMPRESA_COLABORADORES, hasAccess: handleCheckShowElement({ me, rolesRequired: [] }) },
        { label: 'Clientes', icon: 'user-check', route: ROUTES.EMPRESA_CLIENTES, hasAccess: handleCheckShowElement({ me, rolesRequired: [] }) },
        { label: 'Financeiro', icon: 'dollar-sign', route: ROUTES.EMPRESA_FINANCEIRO, hasAccess: handleCheckShowElement({ me, rolesRequired: [MODULES.Sales] }) },
        { label: 'Logs', icon: 'file-text', route: ROUTES.LOGS, hasAccess: handleCheckShowElement({ me, rolesRequired: [], mustBeSystemAdmin: true }) }
    ] as iMenuItem[];

    useEffect(() => {
        setActive(pathname);
    }, [pathname]);

    return (
        <aside className={`${styles.sidebar} notSelectable`}>
            <div className={styles.brand}>
                <Icon icon='calendar' weight='bold' />
                <span className={PACIFICO.className}>{SYSTEM.NAME}</span>
                {/* <small>{versionBuild?.buildVersion}</small> */}
            </div>

            <nav>
                <ul>
                    {
                        menuItems?.filter(x => x.hasAccess)?.map((item, index) => (
                            <li
                                key={index}
                                className={active === item.route ? styles.active : ''}
                                onClick={() => router.push(item.route)}
                            >
                                <Icon icon={item.icon} />
                                <span>{item.label}</span>
                            </li>
                        ))
                    }
                </ul>
            </nav>
        </aside>
    )
}