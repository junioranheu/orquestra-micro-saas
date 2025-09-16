import Icon from '@/app/components/icon';
import ROUTES from '@/app/consts/routes';
import SYSTEM from '@/app/consts/system';
import feather from 'feather-icons';
import { useRouter } from 'next/navigation';
import styles from './index.module.scss';

interface iMenuItem {
    label: string;
    icon: keyof typeof feather.icons;
    route: (typeof ROUTES)[keyof typeof ROUTES];
    hasAccess: boolean;
}

export default function Sidebar() {

    const router = useRouter();

    const menuItems = [
        { label: 'Início', icon: 'home', route: ROUTES.DASHBOARD, hasAccess: true },
        { label: 'Clientes', icon: 'user-check', route: ROUTES.EMPRESA_CLIENTES, hasAccess: true },
        { label: 'Agendamentos', icon: 'calendar', route: ROUTES.EMPRESA_AGENDAMENTOS, hasAccess: true },
        { label: 'Financeiro', icon: 'dollar-sign', route: ROUTES.EMPRESA_FINANCEIRO, hasAccess: true },
        { label: 'Faturas', icon: 'file-text', route: ROUTES.EMPRESA_USO_E_PLANO, hasAccess: true },
        { label: 'Membros', icon: 'users', route: ROUTES.EMPRESA_MEMBROS, hasAccess: true }
    ] as iMenuItem[];

    return (
        <aside className={styles.sidebar}>
            <div className={styles.brand}><Icon icon='calendar' weight='bold' /> {SYSTEM.NAME}</div>

            <nav>
                <ul>
                    {
                        menuItems?.filter(x => x.hasAccess)?.map((item, index) => (
                            <li key={index} onClick={() => router.push(item.route)}>
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