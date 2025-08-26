'use client';
import { CONSTS_AUTH } from '@/app/api/consts/auth';
import { Fetch } from '@/app/api/fetch';
import ROUTES from '@/app/consts/routes';
import SYSTEM from '@/app/consts/system';
import useUserContext from '@/app/hooks/contexts/useUserContext';
import useTitle from '@/app/hooks/useTitle';
import Cookies from 'js-cookie';
import { useRouter } from 'next/navigation';
import { useEffect } from 'react';

export default function Logout() {

    useTitle('Encerrando sessão...');

    const router = useRouter();
    const [_, setAuth] = useUserContext();

    async function handleLogout() {
        await Fetch.delete(CONSTS_AUTH.logout);
        Cookies.remove(SYSTEM.COOKIE_NAME, { path: '/' });
        setAuth(null);
        router.push(ROUTES.LOGIN);
    }

    useEffect(() => {
        handleLogout();
    }, []);

    return (
        <section>
        </section>
    )
}