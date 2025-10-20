'use client';
import ROUTES from '@/app/consts/routes';
import useTitle from '@/app/hooks/useTitle';
import { useRouter } from 'next/navigation';
import { useEffect } from 'react';

export default function TermosDeUso() {

    useTitle('Termos de uso');
    const router = useRouter();

    useEffect(() => {
        router.push(`${ROUTES.ETC_AJUDA}/topico?t=termos-de-uso`);
    }, [router]);

    return (
        <section>
        </section>
    )
}