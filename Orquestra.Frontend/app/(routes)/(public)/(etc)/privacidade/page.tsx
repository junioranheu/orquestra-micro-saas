'use client';
import ROUTES from '@/app/consts/routes';
import useTitle from '@/app/hooks/useTitle';
import { useRouter } from 'next/navigation';
import { useEffect } from 'react';

export default function Privacidade() {

    useTitle('Privacidade');
    const router = useRouter();

    useEffect(() => {
        router.push(`${ROUTES.ETC_AJUDA}/topico?t=privacidade-e-lgpd`);
    }, [router]);

    return (
        <section>
        </section>
    )
}