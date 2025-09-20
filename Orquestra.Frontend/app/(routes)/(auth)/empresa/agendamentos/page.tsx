'use client';
import CalendarComplete from '@/app/components/calendar/complete';
import useTitle from '@/app/hooks/useTitle';
import 'react-big-calendar/lib/css/react-big-calendar.css';
import styles from './page.module.scss';

export default function EmpresaAgendamentos() {

    useTitle('Agendamentos');

    return (
        <section className={styles.main}>
            <CalendarComplete />
        </section>
    )
}