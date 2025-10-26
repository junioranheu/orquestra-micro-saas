'use client';
import { iMe } from '@/app/api/consts/auth';
import iSchedule, { CONSTS_SCHEDULE } from '@/app/api/consts/schedule';
import { Fetch } from '@/app/api/fetch';
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
import { DATE_STYLE, handleFormatDate } from '@/app/functions/format.date';
import { handleGetDateBrazil, handleToBrazilDate } from '@/app/functions/get.date.brazil';
import { handleGetFirstName, handleGetNameInitials } from '@/app/functions/get.formatUserName';
import handleGetRandomNumber from '@/app/functions/get.randomNumber';
import swalUnauthorized from '@/app/functions/swal.unauthorized';
import useApiGetMe from '@/app/hooks/api/useApiGetMe';
import useUserContext from '@/app/hooks/contexts/useUserContext';
import useTitle from '@/app/hooks/useTitle';
import Link from 'next/link';
import { useRouter } from 'next/navigation';
import { useEffect, useState } from 'react';
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
                    flipInterval={handleGetRandomNumber(5000, 15000)}
                />
            </span>

            <div className={styles.flex}>
                <CardCalendar me={me} />
            </div>

            {
                me?.currentMainCompany && (
                    <DailyAgenda me={me} />
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
                            buttonLabel='Ver clientes'
                            buttonFunction={() => router.push(ROUTES.EMPRESA_CLIENTES)}
                        />
                    </div>
                )
            }

            <Footer />
        </section>
    )
}

interface iDailyAgendaProps {
    me: iMe;
}

function DailyAgenda({ me }: iDailyAgendaProps) {

    const now = handleGetDateBrazil();
    const [schedules, setSchedules] = useState<iSchedule[]>([]);

    useEffect(() => {
        async function handleFetchSchedules() {
            const items = await Fetch.get({
                url: `${CONSTS_SCHEDULE.getAllByCompanyId}?companyId=${me?.currentMainCompany?.companyId}&getOnlyNearbyDates=true`
            }) as iSchedule[];

            console.log(items);
            setSchedules(items);
        }

        if (me?.currentMainCompany?.companyId) {
            handleFetchSchedules();
        }
    }, [me]);

    function handleCategorizeSchedules() {
        const current: iSchedule[] = [];
        const upcoming: iSchedule[] = [];
        const past: iSchedule[] = [];

        schedules.forEach((schedule) => {
            const startDate = handleToBrazilDate(schedule.dateStart);
            const endDate = handleToBrazilDate(schedule.dateEnd);

            if (now >= startDate && now <= endDate) {
                current.push(schedule);
            } else if (startDate > now) {
                upcoming.push(schedule);
            } else {
                past.push(schedule);
            }
        });

        return { current, upcoming, past };
    }

    function handleFormatTimeRange(start: string, end: string) {
        return `${start.substring(0, 5)} - ${end.substring(0, 5)}`;
    }

    function handleRenderScheduleItem(schedule: iSchedule, isActive = false) {
        return (
            <div key={schedule.scheduleId.toString()} className={`${styles.scheduleItem} ${isActive ? styles.active : ''}`}>
                <div className={`${styles.avatar} notSelectable`}>
                    {handleGetNameInitials(schedule.client?.fullName ?? '')}
                </div>

                <div className={styles.info}>
                    <div className={styles.name}>{schedule.client?.fullName}</div>

                    <div className={styles.service}>
                        {schedule.customTitle || 'Agendamento'}{schedule.isRestrictForSpecificUsers ? ` • Agendamento específico para ${schedule?.usersIds?.length} colaborador${schedule?.usersIds?.length > 1 ? 'es' : ''}` : ''}
                    </div>
                </div>

                <div className={styles.time}>
                    {handleFormatDate(schedule.dateStart, DATE_STYLE.DETALHADO_APENAS_REFERENCIA_DIA)}, {handleFormatTimeRange(schedule.timeStart, schedule.timeEnd)}
                </div>
            </div>
        )
    }

    const { current, upcoming, past } = handleCategorizeSchedules();

    return (
        <div className={styles.dailyAgenda}>
            <h2 className={styles.title}>Resumo da agenda</h2>

            {
                current.length > 0 && (
                    <div className={styles.section}>
                        <h3 className={styles.sectionTitle}>Em andamento</h3>
                        {current.map((schedule) => handleRenderScheduleItem(schedule, true))}
                    </div>
                )
            }

            {
                upcoming.length > 0 && (
                    <div className={styles.section}>
                        <h3 className={styles.sectionTitle}>Próximos</h3>
                        {upcoming.map((schedule) => handleRenderScheduleItem(schedule))}
                    </div>
                )
            }

            {
                past.length > 0 && (
                    <div className={styles.section}>
                        <h3 className={styles.sectionTitle}>Anteriores</h3>
                        {past.map((schedule) => handleRenderScheduleItem(schedule))}
                    </div>
                )
            }

            {
                schedules.length === 0 && (
                    <div className={styles.empty}>
                        <p>Nenhum agendamento para hoje</p>
                    </div>
                )
            }
        </div>
    );
}