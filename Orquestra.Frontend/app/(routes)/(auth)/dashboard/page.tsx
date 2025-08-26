'use client';
import { CONSTS_AUTH } from '@/app/api/consts/auth';
import { CONSTS_LOG } from '@/app/api/consts/log';
import { Fetch } from '@/app/api/fetch';
import Button from '@/app/components/input/button/button';
import ROUTES from '@/app/consts/routes';
import { useIsRequestLoading } from '@/app/hooks/contexts/useGlobalContext';
import useUserContext from '@/app/hooks/contexts/useUserContext';
import useTitle from '@/app/hooks/useTitle';
import { useRouter } from 'next/navigation';
import styles from './page.module.scss';

export default function Dashboard() {

    useTitle('Dashboard');

    const router = useRouter();
    const [auth, setAuth] = useUserContext();
    const [isRequestLoading, setIsRequestLoading] = useIsRequestLoading();

    async function handleLogout() {
        router.push(ROUTES.LOGOUT);
    }

    async function handleXD() {
        setIsRequestLoading(true);
        console.clear();
        const result = await Fetch.get(CONSTS_AUTH.me);
        console.log(CONSTS_AUTH.me, result);
        setIsRequestLoading(false);
    }

    async function handleLog() {
        setTimeout(async () => {
            console.clear();
            const result = await Fetch.get(CONSTS_LOG.get);
            console.log(CONSTS_LOG.get, result);
        }, 1500);
    }

    return (
        <section className={styles.main}>
            <h1>Olá... {auth?.fullName}</h1>

            <br />
            <Button label={'/me'} handleFunction={() => handleXD()} />
            <br />
            <Button label={'/log'} handleFunction={() => handleLog()} />
            <br />
            <Button label={'Logout'} handleFunction={() => handleLogout()} />
        </section>
    )
}