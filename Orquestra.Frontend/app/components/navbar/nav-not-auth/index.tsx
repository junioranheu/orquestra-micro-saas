import Icon from '@/app/components/icon';
import styles from '@/app/components/navbar/nav/index.module.scss';
import ROUTES from '@/app/consts/routes';
import { useRouter } from 'next/navigation';

export default function NavbarNotAuth() {

    const router = useRouter();

    return (
        <nav className={styles.nav}>
            <div className={styles.inner}>
                <div className={styles.left}>
                    <span className={styles.altColor} onClick={() => router.push(ROUTES.DASHBOARD)}><Icon icon='home' weight='bold' /><span>Voltar ao início</span></span>
                </div>

                <div className={styles.right}>
                </div>
            </div>
        </nav>
    )
}