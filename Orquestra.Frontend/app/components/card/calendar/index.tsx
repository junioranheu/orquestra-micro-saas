'use client';
import SvgOne from '@/app/assets/svg/one.svg';
import SvgTwo from '@/app/assets/svg/two.svg';
import CalendarSimple from '@/app/components/calendar/simple';
import CardSimple from '@/app/components/card/simple';
import { MODULES } from '@/app/consts/modules';
import ROUTES from '@/app/consts/routes';
import SYSTEM from '@/app/consts/system';
import { handleCheckShowElement } from '@/app/functions/check.permission';
import useApiGetMe from '@/app/hooks/api/useApiGetMe';
import { useRouter } from 'next/navigation';
import { useEffect, useState } from 'react';
import styles from './index.module.scss';

export default function CardCalendar() {

    const me = useApiGetMe();
    const router = useRouter();
    const [hasAccessToSchedule, setHasAccessToSchedule] = useState<boolean>(false);

    useEffect(() => {
        const hasAccess = handleCheckShowElement(me, [MODULES.Scheduling]);
        setHasAccessToSchedule(hasAccess);
    }, [me]);

    return (
        <section className={styles.wrapper}>
            <CalendarSimple
                isReadOnly={!hasAccessToSchedule}
                disablePastDays={true}
            />

            <div className={styles.panel}>
                <div className={styles.panelInner}>
                    <div className={styles.panelHeader}>
                        <h2>Comece a usar o {SYSTEM.NAME}</h2>
                    </div>

                    <div className={styles.steps}>
                        {
                            hasAccessToSchedule ? (
                                <CardSimple
                                    img={SvgOne}
                                    title='Tudo certo!'
                                    description='Sua empresa já possui o módulo de Agendamento. Comece a gerenciar seus compromissos agora mesmo.'
                                />
                            ) : (
                                <CardSimple
                                    img={SvgOne}
                                    title='Não perca mais tempo!'
                                    description='No momento você não está vinculado a nenhuma empresa, ou sua empresa ainda não ativou o módulo de Agendamento. Confira sua situação clicando no botão abaixo:'
                                    buttonLabel='Gerenciar situação da empresa'
                                    buttonFunction={() => router.push(ROUTES.EMPRESA_GERENCIAR)}
                                />
                            )
                        }

                        <CardSimple
                            img={SvgTwo}
                            title='Simplifique sua vida e a gestão da sua empresa'
                            description='Gestão de horários simples, rápida e sem dor de cabeça. Agende agora seus compromissos em segundos. Seu negócio afinado como uma orquestra.'
                            buttonLabel='Agendar compromissos'
                            buttonFunction={() => router.push(ROUTES.EMPRESA_AGENDAMENTOS)}
                            buttonDisabled={!hasAccessToSchedule}
                        />
                    </div>
                </div>
            </div>
        </section>
    )
}