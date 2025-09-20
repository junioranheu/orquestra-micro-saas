'use client';
import CalendarComplete, { iEvent } from '@/app/components/calendar/complete';
import useTitle from '@/app/hooks/useTitle';
import 'react-big-calendar/lib/css/react-big-calendar.css';
import styles from './page.module.scss';

export default function EmpresaAgendamentos() {

    useTitle('Agendamentos');

    const events = [
        {
            title: 'Consulta com dentista',
            start: new Date(2025, 8, 21, 9, 30), // 21/09/2025 09:30
            end: new Date(2025, 8, 21, 10, 30),
        },
        {
            title: 'Reunião com cliente',
            start: new Date(2025, 8, 22, 14, 0), // 22/09/2025 14:00
            end: new Date(2025, 8, 22, 15, 0),
        },
        {
            title: 'Feriado',
            start: new Date(2025, 8, 23), // começa dia inteiro
            end: new Date(2025, 8, 23),   // termina mesmo dia
            allDay: true,
        }
    ] as iEvent[];

    return (
        <section className={styles.main}>
            <CalendarComplete events={events} />
        </section>
    )
}