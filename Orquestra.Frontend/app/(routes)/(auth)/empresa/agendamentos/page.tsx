'use client';
import CalendarComplete, { iEvent } from '@/app/components/calendar/complete';
import SYSTEM from '@/app/consts/system';
import useDisableScroll from '@/app/hooks/useDisableScroll';
import useTitle from '@/app/hooks/useTitle';
import { Guid } from 'guid-typescript';
import { useEffect, useRef, useState } from 'react';
import 'react-big-calendar/lib/css/react-big-calendar.css';
import styles from './page.module.scss';

export default function EmpresaAgendamentos() {

    useTitle('Agendamentos');
    useDisableScroll();

    const [events, setEvents] = useState<iEvent[]>([]);

    useEffect(() => {
        const events = [
            {
                scheduleId: Guid.create(),
                title: 'Pita',
                start: new Date(2025, 8, 19, 9, 30),
                end: new Date(2025, 8, 19, 10, 30)
            },
            {
                scheduleId: Guid.create(),
                title: 'Mariana Scalzaretto',
                start: new Date(2025, 8, 21, 8, 30),
                end: new Date(2025, 8, 21, 10, 5)
            },
            {
                scheduleId: Guid.create(),
                title: 'Bau bau',
                start: new Date(2025, 8, 21, 12, 30),
                end: new Date(2025, 8, 21, 13, 30)
            },
            {
                scheduleId: Guid.create(),
                title: 'Bau bau',
                start: new Date(2025, 8, 21, 12, 30),
                end: new Date(2025, 8, 21, 13, 30)
            },
            {
                scheduleId: Guid.create(),
                title: 'Bau bau',
                start: new Date(2025, 8, 21, 12, 30),
                end: new Date(2025, 8, 21, 13, 30)
            },
            {
                scheduleId: Guid.create(),
                title: 'Bau bau',
                start: new Date(2025, 8, 21, 12, 30),
                end: new Date(2025, 8, 21, 13, 30)
            },
            {
                scheduleId: Guid.create(),
                title: 'Bau bau',
                start: new Date(2025, 8, 21, 12, 30),
                end: new Date(2025, 8, 21, 13, 30)
            },
            {
                scheduleId: Guid.create(),
                title: 'Bau bau',
                start: new Date(2025, 8, 21, 12, 30),
                end: new Date(2025, 8, 21, 13, 30)
            },
            {
                scheduleId: Guid.create(),
                title: 'Bau bau',
                start: new Date(2025, 8, 21, 12, 30),
                end: new Date(2025, 8, 21, 13, 30)
            },
            {
                scheduleId: Guid.create(),
                title: 'Bau bau',
                start: new Date(2025, 8, 21, 12, 30),
                end: new Date(2025, 8, 21, 13, 30)
            },
            {
                scheduleId: Guid.create(),
                title: 'Bau bau',
                start: new Date(2025, 8, 21, 15, 30),
                end: new Date(2025, 8, 21, 16, 30)
            },
            {
                scheduleId: Guid.create(),
                title: 'Reunião com o Lule',
                start: new Date(2025, 8, 22, 14, 0),
                end: new Date(2025, 8, 22, 15, 0)
            },
            {
                scheduleId: Guid.create(),
                title: 'Feriado',
                start: new Date(2025, 8, 23),
                end: new Date(2025, 8, 23),
                allDay: true
            },
            {
                scheduleId: Guid.create(),
                title: 'Reunião com o Lulers',
                start: new Date(2025, 8, 20, 21, 0),
                end: new Date(2025, 8, 20, 22, 0)
            },
            {
                scheduleId: Guid.create(),
                title: 'Reunião com o Lulers',
                start: new Date(2025, 8, 20, 22, 0),
                end: new Date(2025, 8, 20, 23, 0)
            }
        ] as iEvent[];

        setEvents(events);
    }, []);

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
                availableHeight && (
                    <CalendarComplete events={events} customElementHeight={(availableHeight)} />
                )
            }
        </section>
    )
}