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
                resetBorderRadiusRight={true}
                removeAllBorder={true}
            />

            <div className={styles.flex}>
                <div className={styles.panel}>
                    <div className={styles.panelInner}>
                        <div className={styles.panelHeader}>
                            <span>Comece a usar o {SYSTEM.NAME}</span>
                        </div>

                        <div className={styles.steps}>
                            {
                                hasAccessToSchedule ? (
                                    <CardSimple
                                        img={SvgOne}
                                        isImgInsideOfCard={false}
                                        title='Tudo certo!'
                                        description='Sua empresa já possui o módulo de Agendamento. Comece a gerenciar seus compromissos agora mesmo.'
                                    />
                                ) : (
                                    <CardSimple
                                        img={SvgOne}
                                        isImgInsideOfCard={false}
                                        title='Não perca mais tempo!'
                                        description='No momento você não está vinculado a nenhuma empresa, ou sua empresa ainda não ativou o módulo de Agendamento. Confira sua situação clicando no botão abaixo:'
                                        buttonLabel='Gerenciar situação da empresa'
                                        buttonFunction={() => router.push(ROUTES.EMPRESA_GERENCIAR)}
                                    />
                                )
                            }

                            <CardSimple
                                img={SvgTwo}
                                isImgInsideOfCard={false}
                                title='Simplifique a gestão da sua empresa'
                                description='Gestão de horários simples, rápida e sem dor de cabeça. Seu negócio afinado como uma orquestra.'
                                buttonLabel='Agendar compromissos'
                                buttonFunction={() => router.push(ROUTES.EMPRESA_AGENDAMENTOS)}
                                buttonDisabled={!hasAccessToSchedule}
                            />
                        </div>
                    </div>
                </div>
            </div>
        </section>
    )
} 