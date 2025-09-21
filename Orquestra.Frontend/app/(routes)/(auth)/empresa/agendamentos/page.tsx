'use client';
import CalendarComplete, { iEvent } from '@/app/components/calendar/complete';
import useDisableScroll from '@/app/hooks/useDisableScroll';
import useTitle from '@/app/hooks/useTitle';
import { Guid } from 'guid-typescript';
import { useEffect, useState } from 'react';
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
                start: new Date(2025, 8, 21, 9, 30),
                end: new Date(2025, 8, 21, 10, 30)
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
            }
        ] as iEvent[];

        setEvents(events);
    }, []);

    return (
        <section className={styles.main}>
            <CalendarComplete events={events} />
        </section>
    )
}