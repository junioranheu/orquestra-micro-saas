'use client';
import { iMe } from '@/app/api/consts/auth';
import SvgOne from '@/app/assets/svg/one.svg';
import SvgTwo from '@/app/assets/svg/two.svg';
import CalendarSimple from '@/app/components/calendar/simple';
import CardSimple from '@/app/components/card/simple';
import { MODULES } from '@/app/consts/modules';
import ROUTES from '@/app/consts/routes';
import SYSTEM from '@/app/consts/system';
import { handleCheckShowElement } from '@/app/functions/check.permission';
import useWindowSize from '@/app/hooks/useWindowSize';
import { useRouter } from 'next/navigation';
import { useEffect, useState } from 'react';
import styles from './index.module.scss';

interface iProps {
    me: iMe | undefined;
}

export default function CardCalendar({ me }: iProps) {

    const router = useRouter();
    const windowSize = useWindowSize();
    const [hasAccessToSchedule, setHasAccessToSchedule] = useState<boolean>(false);

    useEffect(() => {
        const hasAccess = handleCheckShowElement({ me, rolesRequired: [MODULES.Scheduling] });
        setHasAccessToSchedule(hasAccess);
    }, [me]);

    return (
        <section className={styles.wrapper}>
            <CalendarSimple
                isReadOnly={!hasAccessToSchedule}
                disablePastDays={true}
                resetBorderRadiusRight={!windowSize.width ? true : windowSize.width > 1281}
                removeBorderRight={!windowSize.width ? true : windowSize.width > 1281}
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
                                        isImgInsideOfCard={!windowSize.width ? false : windowSize.width < 1281}
                                        title='Tudo certo!'
                                        description={`A empresa <b>${me?.currentMainCompany?.name}</b> já possui o módulo de agendamentos.<br/>Comece a gerenciar seus compromissos agora mesmo clicando no botão do card ao lado.`}
                                        className={SYSTEM.ANIMATE_DELAY_0_5s}
                                    />
                                ) : (
                                    <CardSimple
                                        img={SvgOne}
                                        isImgInsideOfCard={!windowSize.width ? false : windowSize.width < 1281}
                                        title='Não perca mais tempo!'
                                        description='No momento você não está vinculado a nenhuma empresa, ou sua empresa ainda não ativou o módulo de Agendamento.<br/>Confira sua situação abaixo:'
                                        buttonLabel='Gerenciar situação da empresa'
                                        buttonFunction={() => router.push(ROUTES.EMPRESA_USO_E_PLANO)}
                                        className={SYSTEM.ANIMATE_DELAY_0_5s} />
                                )
                            }

                            <CardSimple
                                img={SvgTwo}
                                isImgInsideOfCard={windowSize.width < 1281}
                                title='Simplifique a gestão da sua empresa'
                                description='Gestão de horários simples, rápida e sem dor de cabeça.<br/>Seu negócio afinado como uma orquestra.'
                                buttonLabel='Agendar compromissos agora mesmo'
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