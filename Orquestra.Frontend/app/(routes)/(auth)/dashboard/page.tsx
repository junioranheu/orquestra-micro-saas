'use client';
import { CONSTS_AUTH } from '@/app/api/consts/auth';
import { Fetch } from '@/app/api/fetch';
import Button from '@/app/components/input/button/button';
import ROUTES from '@/app/consts/routes';
import SYSTEM from '@/app/consts/system';
import useUserContext from '@/app/hooks/contexts/useUserContext';
import useTitle from '@/app/hooks/useTitle';
import Cookies from 'js-cookie';
import { useRouter } from 'next/navigation';
import { useEffect } from 'react';
import styles from './page.module.scss';

export default function Dashboard() {

    useTitle('Dashboard');

    const router = useRouter();
    const [auth, setAuth] = useUserContext();

    useEffect(() => {
        console.log(auth);
    }, [auth]);

    async function handleLogout() {
        await Fetch.delete(CONSTS_AUTH.logout);
        Cookies.remove(SYSTEM.COOKIE_NAME, { path: '/' });
        setAuth(null);
        router.push(ROUTES.ENTRAR);
    }

    return (
        <section className={styles.main}>
            <h1>Olá... {auth?.fullName}</h1>

            <br />
            <Button label={'Logout'} handleFunction={() => handleLogout()} />
        </section>
    )
}