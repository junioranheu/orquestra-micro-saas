'use client';
import useUserContext from '@/app/hooks/contexts/useUserContext';
import useTitle from '@/app/hooks/useTitle';
import { useRouter } from 'next/navigation';
import styles from './page.module.scss';

export default function Dashboard() {

    useTitle('Dashboard');

    const router = useRouter();
    const [auth, setAuth] = useUserContext();

    return (
        <section className={styles.main}>
            <h1>Olá, mundo</h1>
        </section>
    )
}