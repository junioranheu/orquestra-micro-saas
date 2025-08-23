'use client';
import useUserContext from '@/app/hooks/contexts/useUserContext';
import useTitle from '@/app/hooks/useTitle';
import { useRouter } from 'next/navigation';
import styles from './page.module.scss';

export default function Erro403() {

    useTitle('Sem acesso');

    const router = useRouter();
    const [auth, setAuth] = useUserContext();

    return (
        <section className={styles.main}>
            <h1>Sem acesso</h1>
        </section>
    )
}