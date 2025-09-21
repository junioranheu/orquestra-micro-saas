'use client';
import CalendarComplete, { iEvent } from '@/app/components/calendar/complete';
import SYSTEM from '@/app/consts/system';
import useApiGetMe from '@/app/hooks/api/useApiGetMe';
import useDisableScroll from '@/app/hooks/useDisableScroll';
import useTitle from '@/app/hooks/useTitle';
import { useEffect, useRef, useState } from 'react';
import 'react-big-calendar/lib/css/react-big-calendar.css';
import styles from './page.module.scss';

export default function EmpresaAgendamentos() {

    useTitle('Agendamentos');
    useDisableScroll();

    const me = useApiGetMe();
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
        <section className={`${styles.main} ${SYSTEM.ANIMATE}`} ref={sectionRef}>
            {
                availableHeight && me && me?.currentMainCompany && (
                    <CalendarComplete
                        events={events}
                        customElementHeight={(availableHeight)}
                        companyId={me?.currentMainCompany?.companyId}
                        setEvents={setEvents}
                    />
                )
            }
        </section>
    )
}