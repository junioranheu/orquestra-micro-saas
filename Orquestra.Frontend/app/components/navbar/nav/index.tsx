import Icon from '@/app/components/icon';
import ROUTES from '@/app/consts/routes';
import useApiGetMe from '@/app/hooks/api/useApiGetMe';
import useUserContext from '@/app/hooks/contexts/useUserContext';
import Tippy from '@tippyjs/react';
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

                    <Tippy content="Ajuda">
                        <span><Icon icon='help-circle' weight='bold' /></span>
                    </Tippy>


                    <Tippy content="Notificações">
                        <span><Icon icon='bell' weight='bold' /></span>
                    </Tippy>


                    <Tippy content="Gerencie seu perfil, plano, configurações e muito mais.">
                        <span><Icon icon='briefcase' weight='bold' />{me && me?.currentMainCompany ? me.currentMainCompany?.name : 'Olá, tudo bem?'} <Icon icon='chevron-down' weight='bold' /></span>
                    </Tippy>
                </div>
            </div>
        </nav>
    )
}