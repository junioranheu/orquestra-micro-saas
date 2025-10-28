'use client';
import SvgUserArrow from '@/app/assets/svg/user-arrow.svg';
import SvgUserEnvelope from '@/app/assets/svg/user-envelope.svg';
import CardCalendar from '@/app/components/card/calendar';
import CardSimple from '@/app/components/card/simple';
import ContentLoaderText from '@/app/components/content-loader/text';
import Footer from '@/app/components/footer';
import Mascot from '@/app/components/mascot';
import WhatsappHyperlink from '@/app/components/whatsapp/hyperlink';
import ROUTES from '@/app/consts/routes';
import SYSTEM from '@/app/consts/system';
import { handleGetFirstName } from '@/app/functions/get.formatUserName';
import handleGetRandomNumber from '@/app/functions/get.randomNumber';
import swalUnauthorized from '@/app/functions/swal.unauthorized';
import useApiGetMe from '@/app/hooks/api/useApiGetMe';
import useUserContext from '@/app/hooks/contexts/useUserContext';
import useTitle from '@/app/hooks/useTitle';
import Link from 'next/link';
import { useRouter } from 'next/navigation';
import { useEffect } from 'react';
import CardDailyAgenda from './components/card-daily-agenda';
import CardNotifications from './components/card-notifications';
import styles from './page.module.scss';

export default function Dashboard() {

    useTitle('Dashboard');

    const router = useRouter();
    const [auth,] = useUserContext();
    const me = useApiGetMe({});

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
            <span className={styles.hello}>
                <ContentLoaderText text={`Olá, ${handleGetFirstName(auth?.fullName)}`} />

                <Mascot
                    isCentralized={false}
                    tippyContent={
                        <div>
                            Oi! Eu sou o {SYSTEM.MASCOT}. 🐱✨<br /><br />
                            Se pintar alguma dúvida, dá uma passadinha na <Link href={ROUTES.ETC_AJUDA}>central de ajuda</Link> ou fala com a gente pelo <WhatsappHyperlink showIcon={false} />.<br /><br />
                            Caso queira me dispensar por um tempo, é só ajustar isso na aba de personalização, nas <Link href={ROUTES.USUARIO_CONFIGURACOES}>configurações</Link> do {SYSTEM.NAME}. 😅
                        </div>
                    }
                    tippyPlacement='bottom'
                    flipPeriodic={true}
                    flipInterval={handleGetRandomNumber(5000, 10000)}
                />
            </span>

            <div className={styles.flex}>
                <CardCalendar me={me} />
            </div>

            {
                me?.currentMainCompany && (
                    <CardDailyAgenda me={me} />
                )
            }

            {
                me?.currentMainCompany && (
                    <CardNotifications />
                )
            }

            {
                !me?.currentMainCompany ? (
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
                ) : (
                    <div className={styles.flex}>
                        <CardSimple
                            img={SvgUserArrow}
                            title='Colaboradores'
                            description='Adicione, gerencie e defina permissões para os profissionais da sua empresa.<br/>Mantenha sua equipe organizada e com os acessos certos.'
                            buttonLabel='Gerenciar equipe'
                            buttonFunction={() => router.push(ROUTES.EMPRESA_COLABORADORES)}
                        />

                        <CardSimple
                            img={SvgUserEnvelope}
                            title='Clientes'
                            description='Acompanhe e gerencie os dados dos seus clientes em um só lugar.<br/>Visualize histórico de atendimentos, contatos e informações importantes com facilidade.'
                            buttonLabel='Gerenciar clientes'
                            buttonFunction={() => router.push(ROUTES.EMPRESA_CLIENTES)}
                        />
                    </div>
                )
            }

            <Footer />
        </section>
    )
}