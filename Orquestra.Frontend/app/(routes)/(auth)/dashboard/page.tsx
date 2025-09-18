'use client';
import SvgUserArrow from '@/app/assets/svg/user-arrow.svg';
import SvgUserEnvelope from '@/app/assets/svg/user-envelope.svg';
import CardCalendar from '@/app/components/card/calendar';
import CardSimple from '@/app/components/card/simple';
import { handleGetFirstName } from '@/app/functions/get.formatUserName';
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
            <span className={styles.hello}>Olá, {handleGetFirstName(auth?.fullName)}</span>

            <div className={styles.flex}>
                <CardCalendar />
            </div>

            <div className={styles.flex}>
                <CardSimple
                    img={SvgUserArrow}
                    title='Organize seus contatos para personalizar as mensagens'
                    description='Com segmentação e listas, crie campanhas altamente direcionadas com base no comportamento ou na demografia de seus clientes.'
                    buttonLabel='AEA'
                    buttonFunction={() => alert('xd')}
                />

                <CardSimple
                    img={SvgUserEnvelope}
                    title='XD'
                    description='Huh?'
                    buttonLabel='Oi, né?'
                    buttonFunction={() => alert('xdxd')}
                />
            </div>
        </section>
    )
}