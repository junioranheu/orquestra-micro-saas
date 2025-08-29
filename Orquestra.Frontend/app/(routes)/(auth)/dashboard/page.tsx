'use client';
import iMe, { CONSTS_AUTH } from '@/app/api/consts/auth';
import { CONSTS_LOG } from '@/app/api/consts/log';
import { Fetch } from '@/app/api/fetch';
import Button from '@/app/components/input/button/button';
import ROUTES from '@/app/consts/routes';
import { DATE_STYLE, handleFormatDate } from '@/app/functions/format.date';
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
        console.clear();
        const result = await Fetch.get({ url: CONSTS_AUTH.me, setIsRequestLoading: setIsRequestLoading }) as iMe;
        console.log(CONSTS_AUTH.me, result);
    }

    async function handleLog() {
        console.clear();
        const result = await Fetch.get({ url: CONSTS_LOG.get, setIsRequestLoading: setIsRequestLoading });
        console.log(CONSTS_LOG.get, result);
    }

    return (
        <section className={styles.main}>
            <h1>Olá... {auth?.fullName}</h1>
            {auth && <h2>Refresh token válido até {handleFormatDate(auth?.refreshTokenExpirationDate, DATE_STYLE.DETALHADO)}</h2>}

            <br />
            <Button label={'/me'} handleFunction={() => handleXD()} />
            <br />
            <Button label={'/log'} handleFunction={() => handleLog()} />
            <br />
            <Button label={'Logout'} handleFunction={() => handleLogout()} />
            <h1>a</h1>
            <br />
            <h1>a</h1>
            <br />
            <h1>a</h1>
            <br />
            <h1>a</h1>
            <br />
            <h1>a</h1>
            <br />
            <h1>a</h1>
            <br />
            <h1>a</h1>
            <br />
            <h1>a</h1>
            <br />
            <h1>a</h1>
            <br />
            <h1>a</h1>
            <br />
            <h1>a</h1>
            <br />
            <h1>a</h1>
            <br />
            <h1>a</h1>
            <br />
            <h1>a</h1>
            <br />
            <h1>a</h1>
            <br />
            <h1>a</h1>
            <br />
            <h1>a</h1>
            <br />
            <h1>a</h1>
            <br />
            <h1>a</h1>
            <br />
            <h1>a</h1>
            <br />
            <h1>a</h1>
            <br />
            <h1>a</h1>
            <br />
            <h1>a</h1>
            <br />
            <h1>a</h1>
            <br />
            <h1>a</h1>
            <br />
            <h1>a</h1>
            <br />
            <h1>a</h1>
            <br />
            <h1>a</h1>
            <br />
            <h1>a</h1>
            <br />
            <h1>a</h1>
            <br />
        </section>
    )
}