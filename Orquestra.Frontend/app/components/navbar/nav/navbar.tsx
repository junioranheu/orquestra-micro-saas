import Icon from '@/app/components/icon/icon';
import ROUTES from '@/app/consts/routes';
import useUserContext from '@/app/hooks/contexts/useUserContext';
import { useRouter } from 'next/navigation';
import { useState } from 'react';
import styles from './navbar.module.scss';

export default function Navbar() {

    const router = useRouter();
    const [auth, setAuth] = useUserContext();
    const [isMenuOpen, setIsMenuOpen] = useState<boolean>(false);

    function handleRedirect() {
        router.push(auth ? ROUTES.DASHBOARD : ROUTES.LOGIN);
    }

    return (
        <nav className={styles.nav}>
            <div className={styles.inner}>
                <div className={styles.right}>
                    <span><Icon icon='tag' size='small' /> Uso e plano</span>
                    <span>icone ajuda</span>
                    <span>iconem alerta</span>
                    <span>nome empresa com um dropdown</span>
                </div>
            </div>
        </nav>
    )
}