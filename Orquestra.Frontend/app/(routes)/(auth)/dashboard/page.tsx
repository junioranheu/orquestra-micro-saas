'use client';
import { CONSTS_AUTH } from '@/app/api/consts/auth';
import { Fetch } from '@/app/api/fetch';
import Button from '@/app/components/input/button/button';
import useUserContext from '@/app/hooks/contexts/useUserContext';
import useTitle from '@/app/hooks/useTitle';
import { useRouter } from 'next/navigation';
import { useState } from 'react';
import styles from './page.module.scss';

export default function Dashboard() {

    useTitle('Dashboard');

    const router = useRouter();
    const [auth, setAuth] = useUserContext();
    const [aea, setAea] = useState<string>('');

    async function handleXD() {
        console.clear();
        const result = await Fetch.get(CONSTS_AUTH.me);
        console.log(CONSTS_AUTH.me, result);

        setAea(result.userName);
    }

    return (
        <section className={styles.main}>
            <h1>Olá... {aea}</h1>

            <Button label={'XD'} handleFunction={() => handleXD()} />
        </section>
    )
}