import Icon from '@/app/components/icon';
import ROUTES from '@/app/consts/routes';
import useApiGetMe from '@/app/hooks/api/useApiGetMe';
import Tippy from '@tippyjs/react';
import { useRouter } from 'next/navigation';
import { useState } from 'react';
import styles from './index.module.scss';

export default function Navbar() {

    const router = useRouter();
    const me = useApiGetMe();
    const [isMenuOpen, setIsMenuOpen] = useState<boolean>(false);

    return (
        <nav className={styles.nav}>
            <div className={styles.inner}>
                <div className={styles.right}>
                    <Tippy content="Entenda mais sobre plano atual e explore novos">
                        <span onClick={() => router.push(ROUTES.DASHBOARD)}><Icon icon='tag' weight='bold' /> Uso e plano</span>
                    </Tippy>

                    <Tippy content="Ajuda">
                        <span onClick={() => router.push(ROUTES.DASHBOARD)}><Icon icon='help-circle' weight='bold' /></span>
                    </Tippy>

                    <Tippy content="Notificações">
                        <span onClick={() => router.push(ROUTES.DASHBOARD)}><Icon icon='bell' weight='bold' /></span>
                    </Tippy>

                    <Tippy content="Gerencie seu perfil, plano, configurações e muito mais.">
                        <span onClick={() => setIsMenuOpen(!isMenuOpen)}>
                            <Icon icon='briefcase' weight='bold' />{me && me?.currentMainCompany ? me.currentMainCompany?.name : 'Olá, tudo bem?'} <Icon icon='chevron-down' weight='bold' />
                        </span>
                    </Tippy>

                    <h1>{isMenuOpen.toString()}</h1>
                </div>
            </div>
        </nav>
    )
}