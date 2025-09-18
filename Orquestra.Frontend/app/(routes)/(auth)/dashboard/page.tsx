'use client';
import swalUnauthorized from '@/app/functions/swal.unauthorized';
import useApiGetMe from '@/app/hooks/api/useApiGetMe';
import useUserContext from '@/app/hooks/contexts/useUserContext';
import useTitle from '@/app/hooks/useTitle';
import { useEffect } from 'react';
import styles from './page.module.scss';

export default function Dashboard() {

    useTitle('Dashboard');

    const [auth, setAuth] = useUserContext();
    const me = useApiGetMe();

    // Verificar se o usuário autenticado pelo back e front são o mesmo;
    useEffect(() => {
        if (auth && me) {
            if (auth.userId !== me.userId) {
                swalUnauthorized();
            }
        }
    }, [auth, me]);

    return (
        <section className={styles.main}>
            <h1>Olá... {auth?.fullName}</h1>
        </section>
    )
}