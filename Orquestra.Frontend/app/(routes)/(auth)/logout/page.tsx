'use client';
import { handleRemoveCookieAndLogout } from '@/app/functions/set.cookies';
import useUserContext from '@/app/hooks/contexts/useUserContext';
import useDisableScroll from '@/app/hooks/useDisableScroll';
import useTitle from '@/app/hooks/useTitle';
import { useRouter } from 'next/navigation';
import { useEffect } from 'react';

export default function Logout() {

    useTitle('Encerrando sessão...');

    useDisableScroll();
    const router = useRouter();
    const [_, setAuth] = useUserContext();

    async function handleLogout() {
        await handleRemoveCookieAndLogout(setAuth, router);
    }

    useEffect(() => {
        handleLogout();
    }, []);

    return (
        <section>
        </section>
    )
}