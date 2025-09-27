'use client';
import SvgUserArrow from '@/app/assets/svg/user-arrow.svg';
import SvgUserEnvelope from '@/app/assets/svg/user-envelope.svg';
import CardCalendar from '@/app/components/card/calendar';
import CardSimple from '@/app/components/card/simple';
import Footer from '@/app/components/footer';
import ROUTES from '@/app/consts/routes';
import { handleGetFirstName } from '@/app/functions/get.formatUserName';
import swalUnauthorized from '@/app/functions/swal.unauthorized';
import useApiGetMe from '@/app/hooks/api/useApiGetMe';
import useUserContext from '@/app/hooks/contexts/useUserContext';
import useTitle from '@/app/hooks/useTitle';
import { useRouter } from 'next/navigation';
import { useEffect } from 'react';
import styles from './page.module.scss';

export default function Dashboard() {

    useTitle('Dashboard');

    const router = useRouter();
    const [auth,] = useUserContext();
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
                    title='Configurações avançadas'
                    description='Personalize a plataforma do seu jeito: gerencie preferências, permissões e integrações em um só lugar.'
                    buttonLabel='Abrir configurações'
                    buttonFunction={() => router.push(ROUTES.USUARIO_CONFIGURACOES)}
                />

                <CardSimple
                    img={SvgUserEnvelope}
                    title='Central de ajuda'
                    description='Encontre respostas rápidas, tutoriais e suporte para tirar suas dúvidas e aproveitar ao máximo a plataforma.'
                    buttonLabel='Acessar a central de ajuda'
                    buttonFunction={() => router.push(ROUTES.ETC_AJUDA)}
                />
            </div>

            <Footer />
        </section>
    )
}