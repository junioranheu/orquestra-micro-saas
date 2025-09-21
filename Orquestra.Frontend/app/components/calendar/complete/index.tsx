import swal from '@/app/functions/swal';
import { format, getDay, parse, startOfWeek } from 'date-fns';
import { ptBR } from 'date-fns/locale';
import { Guid } from 'guid-typescript';
import { useEffect, useState } from 'react';
import { Calendar, dateFnsLocalizer, Event as RBCEvent, SlotInfo, View } from 'react-big-calendar';
import styles from './index.module.scss';

export interface iEvent extends RBCEvent {
    scheduleId: Guid;
    title: string;
    start: Date;
    end: Date;
    allDay?: boolean;
}

interface iProps {
    events: iEvent[];
}

export default function CalendarComplete({ events }: iProps) {

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

    const [date, setDate] = useState<Date>(new Date());
    const [view, setView] = useState<View>('month');
    const [availableViews, setAvailableViews] = useState<Array<View>>(['month', 'week', 'day', 'agenda']);

    useEffect(() => {
        function handleResize() {
            if (window.innerWidth < 801) {
                setView('day'); // Força a view pra day;
                setAvailableViews(['day', 'agenda']); // Remove month e week;
            } else {
                setAvailableViews(['month', 'week', 'day', 'agenda']); // Mostra todas;
            }
        };

        handleResize();
        window.addEventListener('resize', handleResize);
        return () => window.removeEventListener('resize', handleResize);
    }, []);


    function handleAddNewEvent(event: SlotInfo) {
        const today = new Date();
        today.setHours(0, 0, 0, 0);
        const slotDate = new Date(event.start);
        slotDate.setHours(0, 0, 0, 0);

        if (slotDate < today) {
            swal({ content: 'Não é possível agendar um compromisso numa data passada.', icon: 'error' });
            return;
        }

        const newEvent = {
            scheduleId: Guid.create(),
            title: 'Novo Evento',
            start: event.start,
            end: event.end,
            allDay: true,
        } as iEvent;

        console.log('Novo evento:', event.start.toLocaleDateString('pt-BR'), event);
        console.log('newEvent:', newEvent);
    }

    function handleCheckEvent(event: iEvent) {
        console.log('Clicou no evento:', event);
    }

    return (
        <div className={styles.calendar}>
            <Calendar
                localizer={localizer}
                events={events}
                startAccessor='start'
                endAccessor='end'
                date={date}
                view={view}
                onNavigate={setDate}
                onView={setView}
                views={availableViews}
                style={{ height: '100%' }}
                culture='pt-BR'
                messages={messages}
                selectable={true}
                onSelectSlot={(e) => handleAddNewEvent(e)}
                onSelectEvent={(e) => handleCheckEvent(e)}
                dayPropGetter={(date) => {
                    const today = new Date();
                    today.setHours(0, 0, 0, 0);

                    const day = new Date(date);
                    day.setHours(0, 0, 0, 0);

                    if (day < today) {
                        return {
                            style: {
                                cursor: 'not-allowed',
                                backgroundColor: '#f5f5f5'
                            }
                        }
                    }

                    return {};
                }}
                eventPropGetter={(event) => {
                    const today = new Date();
                    today.setHours(0, 0, 0, 0);

                    const eventEnd = new Date(event.end);
                    eventEnd.setHours(0, 0, 0, 0);

                    // Se o evento terminou antes de hoje → cor diferenciada;
                    if (eventEnd < today) {
                        return {
                            style: {
                                backgroundColor: 'var(--gray)'
                            },
                        };
                    }

                    return {};
                }}
            />
        </div>
    )
}