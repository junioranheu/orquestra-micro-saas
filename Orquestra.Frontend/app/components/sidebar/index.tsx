import Icon from '@/app/components/icon';
import ROUTES from '@/app/consts/routes';
import SYSTEM from '@/app/consts/system';
import feather from 'feather-icons';
import { useRouter } from 'next/navigation';
import styles from './index.module.scss';

export default function Sidebar() {

    const router = useRouter();

    const menuItems = [
        { label: 'Início', icon: 'home' as keyof typeof feather.icons, route: ROUTES.DASHBOARD },
        { label: 'Contatos', icon: 'users' as keyof typeof feather.icons, route: ROUTES.DASHBOARD },
        { label: 'Campanhas', icon: 'check-square' as keyof typeof feather.icons, route: ROUTES.DASHBOARD },
        { label: 'Automação', icon: 'settings' as keyof typeof feather.icons, route: ROUTES.DASHBOARD },
        { label: 'Transacional', icon: 'send' as keyof typeof feather.icons, route: ROUTES.DASHBOARD },
        { label: 'Conversações', icon: 'message-square' as keyof typeof feather.icons, route: ROUTES.DASHBOARD },
        { label: 'Oportunidades', icon: 'briefcase' as keyof typeof feather.icons, route: ROUTES.DASHBOARD },
        { label: 'Meetings', icon: 'video' as keyof typeof feather.icons, route: ROUTES.DASHBOARD },
        { label: 'Telefone', icon: 'phone' as keyof typeof feather.icons, route: ROUTES.DASHBOARD },
    ];

    return (
        <aside className={`${styles.sidebar} ${styles.open}`}>
            <div className={styles.brand}>{SYSTEM.NAME}</div>

            <nav>
                <ul>
                    {
                        menuItems.map((item) => (
                            <li key={item.route} onClick={() => router.push(item.route)}>
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