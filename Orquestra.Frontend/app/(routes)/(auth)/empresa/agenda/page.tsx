'use client';
import CalendarComplete, { iEvent } from '@/app/components/calendar/complete';
import Mascot from '@/app/components/mascot';
import ROUTES from '@/app/consts/routes';
import SYSTEM from '@/app/consts/system';
import useApiGetMe from '@/app/hooks/api/useApiGetMe';
import useDisableScroll from '@/app/hooks/useDisableScroll';
import useTitle from '@/app/hooks/useTitle';
import Link from 'next/link';
import { Fragment, useEffect, useRef, useState } from 'react';
import 'react-big-calendar/lib/css/react-big-calendar.css';

export default function EmpresaAgendamentos() {

    useTitle('Agenda');
    useDisableScroll();

    const me = useApiGetMe({});
    const [events, setEvents] = useState<iEvent[]>([]);

    const sectionRef = useRef<HTMLElement>(null);
    const [availableHeight, setAvailableHeight] = useState<number>(0);

    useEffect(() => {
        function updateSize(threshold: number) {
            if (sectionRef.current) {
                const rect = sectionRef.current.getBoundingClientRect();
                const height = window.innerHeight - rect.top - threshold; // Até o final da tela;
                setAvailableHeight(height);
            }
        }

        updateSize(25);
        window.addEventListener('resize', () => updateSize(10));
        return () => window.removeEventListener('resize', () => updateSize(10));
    }, []);

    return (
        <Fragment>
            <section className={SYSTEM.ANIMATE} ref={sectionRef}>
                {
                    availableHeight && me && me?.currentMainCompany ? (
                        <CalendarComplete
                            events={events}
                            customElementHeight={(availableHeight)}
                            companyId={me?.currentMainCompany?.companyId}
                            setEvents={setEvents}
                        />
                    ) : <Fragment></Fragment>
                }
            </section>

            <Mascot
                isCentralized={false}
                tippyContent={
                    <div>
                        Oi! Tudo em dia na sua agenda?<br /><br />
                        Aliás, caso queira me dispensar por um tempo, é só ajustar isso na aba de personalização, nas <Link href={ROUTES.USUARIO_CONFIGURACOES}>configurações</Link> do {SYSTEM.NAME}. 😅
                    </div>
                }
                tippyPlacement='right'
                flip={true}
                absolutePosition='bottom-left'
            />
        </Fragment>
    )
}