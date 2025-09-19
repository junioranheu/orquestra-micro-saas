'use client';
import ImgLoading from '@/app/assets/gif/loading.gif';
import { handleRemoveCookieAndLogout } from '@/app/functions/set.cookies';
import useUserContext from '@/app/hooks/contexts/useUserContext';
import useDisableScroll from '@/app/hooks/useDisableScroll';
import useTitle from '@/app/hooks/useTitle';
import Image from 'next/image';
import { useRouter } from 'next/navigation';
import { useEffect } from 'react';
import styles from './page.module.scss';

export default function Logout() {

    useTitle('Encerrando sessão...');

    useDisableScroll();
    const router = useRouter();
    const [_, setAuth] = useUserContext();

    async function handleLogout() {
        await handleRemoveCookieAndLogout({ setAuth: setAuth, router: router });
    }

    useEffect(() => {
        handleLogout();
    }, []);

    return (
        <section className={styles.main}>
            <Image src={ImgLoading} alt='Carregando' width={50} priority={true} unoptimized={true} />
        </section>
    )
}