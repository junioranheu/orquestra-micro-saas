import ImgLogo from '@/app/assets/png/logo.png';
import ImgHamburguer from '@/app/assets/svg/hamburguer.svg';
import ROUTES from '@/app/consts/routes';
import SYSTEM from '@/app/consts/system';
import useUserContext from '@/app/hooks/contexts/useUserContext';
import Image from 'next/image';
import { useRouter } from 'next/navigation';
import { Fragment, useState } from 'react';
import styles from './navbar.module.scss';

export default function Navbar() {

    const router = useRouter();
    const [auth, setAuth] = useUserContext();
    const [isMenuOpen, setIsMenuOpen] = useState<boolean>(false);

    function handleRedirect() {
        router.push(auth ? ROUTES.DASHBOARD : ROUTES.ENTRAR);
    }

    return (
        <Fragment>
            <nav className={styles.nav}>
                <div className={styles.inner}>
                    <div className={styles.left}>
                        <picture title='Menu' className={styles.hamburguer} onClick={() => setIsMenuOpen(!isMenuOpen)}>
                            <Image src={ImgHamburguer} alt={SYSTEM.NAME} priority={true} />
                        </picture>

                        <picture title='Dashboard' onClick={() => handleRedirect()}>
                            <Image src={ImgLogo} alt={SYSTEM.NAME} priority={true} />
                        </picture>
                    </div>

                    <div className={styles.right}>
                        <span className={styles.hideMobile}>{SYSTEM.NAME}</span>

                        {
                            auth && (
                                <Fragment>
                                    <span>a</span>
                                    <span className={`${styles.separator} ${styles.hideMobile}`}></span>
                                    <span>b</span>
                                    <span className={`${styles.separator} ${styles.hideMobile}`}></span>
                                    <span>c</span>
                                </Fragment>
                            )
                        }
                    </div>
                </div>
            </nav>
        </Fragment>
    )
}