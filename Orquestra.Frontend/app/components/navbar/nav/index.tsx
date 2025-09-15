import Icon from '@/app/components/icon';
import ROUTES from '@/app/consts/routes';
import useApiGetMe from '@/app/hooks/api/useApiGetMe';
import useUserContext from '@/app/hooks/contexts/useUserContext';
import { useRouter } from 'next/navigation';
import { useState } from 'react';
import styles from './index.module.scss';

export default function Navbar() {

    const router = useRouter();
    const [auth, setAuth] = useUserContext();
    const me = useApiGetMe();
    const [isMenuOpen, setIsMenuOpen] = useState<boolean>(false);

    function handleRedirect() {
        router.push(auth ? ROUTES.DASHBOARD : ROUTES.LOGIN);
    }

    return (
        <nav className={styles.nav}>
            <div className={styles.inner}>
                <div className={styles.right}>
                    <span><Icon icon='tag' weight='bold' /> Uso e plano</span>
                    <span><Icon icon='help-circle' weight='bold' /></span>
                    <span><Icon icon='alert-circle' weight='bold' /></span>
                    <span><Icon icon='briefcase' weight='bold' />{me && me.currentMainCompany?.name} <Icon icon='chevron-down' weight='bold' /></span>
                </div>
            </div>
        </nav>
    )
}