import { format, getDay, parse, startOfWeek } from 'date-fns';
import { ptBR } from 'date-fns/locale';
import { useCallback, useState } from 'react';
import { Calendar, dateFnsLocalizer, View } from 'react-big-calendar';
import styles from './index.module.scss';

export default function CalendarComplete() {

    const locales = { 'pt-BR': ptBR };
    const localizer = dateFnsLocalizer({
        format,
        parse,
        startOfWeek: (date: Date) => startOfWeek(date, { weekStartsOn: 1 }), // Segunda;
        getDay,
        locales
    });

    const messages = {
        date: 'Data',
        time: 'Hora',
        event: 'Evento',
        allDay: 'Dia inteiro',
        week: 'Semana',
        work_week: 'Semana útil',
        day: 'Dia',
        month: 'Mês',
        previous: 'Anterior',
        next: 'Próximo',
        yesterday: 'Ontem',
        tomorrow: 'Amanhã',
        today: 'Hoje',
        agenda: 'Agenda',
        noEventsInRange: 'Nenhum evento neste período.',
        showMore: (total: number) => `+ ver mais (${total})`,
    }

    const myEventsList = [
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
    ];

    const [date, setDate] = useState<Date>(new Date());
    const [view, setView] = useState<View>('month');

    const onNavigate = useCallback((newDate: Date) => {
        setDate(newDate);
    }, []);

    const onViewChange = useCallback((newView: View) => {
        setView(newView);
    }, []);

    return (
        <div className={styles.calendar}>
            <Calendar
                localizer={localizer}
                events={myEventsList}
                startAccessor='start'
                endAccessor='end'
                date={date}
                view={view}
                onNavigate={setDate}
                onView={setView}
                views={['month', 'week', 'day', 'agenda']}
                style={{ height: '100%' }}
                culture='pt-BR'
                messages={messages}
            />
        </div>
    )
}