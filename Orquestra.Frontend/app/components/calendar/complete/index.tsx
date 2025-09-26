'use client';
import iClient, { CONSTS_CLIENT } from '@/app/api/consts/client';
import { CONSTS_COMPANY_USER } from '@/app/api/consts/company-user';
import iSchedule, { CONSTS_SCHEDULE } from '@/app/api/consts/schedule';
import { iUser } from '@/app/api/consts/user';
import { Fetch } from '@/app/api/fetch';
import { DATE_STYLE, handleFormatDate, handleParseDateFromString } from '@/app/functions/format.date';
import { handleCapitalizeFirstLetter } from '@/app/functions/get.formatUserName';
import swal from '@/app/functions/swal';
import { useIsRequestLoading } from '@/app/hooks/contexts/useGlobalContext';
import useApiRequestToSetterOnUrlChange from '@/app/hooks/useApiRequestToSetterOnUrlChange';
import { format, getDay, parse, startOfWeek } from 'date-fns';
import { ptBR } from 'date-fns/locale';
import { Guid } from 'guid-typescript';
import { usePathname, useRouter, useSearchParams } from 'next/navigation';
import { Dispatch, Fragment, SetStateAction, useCallback, useEffect, useState } from 'react';
import { Calendar, dateFnsLocalizer, Event as RBCEvent, SlotInfo, View } from 'react-big-calendar';
import styles from './index.module.scss';
import ModalCalendarView from './modal/view';

export interface iEvent extends RBCEvent {
    schedule: iSchedule;
    title: string;
    start: Date;
    end: Date;
    allDay?: boolean;
}

interface iProps {
    events: iEvent[];
    customElementHeight?: number;
    companyId: Guid;
    setEvents: Dispatch<SetStateAction<iEvent[]>>;
}

export default function CalendarComplete({ events, customElementHeight, companyId, setEvents }: iProps) {

    const [, setIsRequestLoading] = useIsRequestLoading();

    const router = useRouter();
    const pathname = usePathname();
    const searchParams = useSearchParams();
    const queryData = searchParams.get('data');

    const [eventClicked, setEventClicked] = useState<iEvent | undefined>(undefined);
    const [isModalOpen, setIsModalOpen] = useState<boolean>(false);
    const [typeModal, setTypeModal] = useState<('edit' | 'create')>();

    const locales = { 'pt-BR': ptBR };
    const localizer = dateFnsLocalizer({
        format,
        parse,
        startOfWeek: (date: Date) => startOfWeek(date, { weekStartsOn: 1 }), // Segunda-feira;
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

    const formats = {
        monthHeaderFormat: (date: Date) => handleCapitalizeFirstLetter(format(date, `MMMM 'de' yyyy`, { locale: ptBR })),
        dayRangeHeaderFormat: ({ start, end }: { start: Date, end: Date }) => `${format(start, 'dd', { locale: ptBR })} - ${format(end, 'dd', { locale: ptBR })} de ${format(start, 'MMMM', { locale: ptBR })}`,
        dayHeaderFormat: (date: Date) => {
            const dayName = format(date, 'EEEE', { locale: ptBR });
            const dayNumber = format(date, 'dd', { locale: ptBR });
            const monthName = format(date, 'MMMM', { locale: ptBR });

            return handleCapitalizeFirstLetter(`${dayName}, ${dayNumber} de ${monthName}`);
        },
        agendaDateFormat: (date: Date) => handleCapitalizeFirstLetter(format(date, `EEE, dd 'de' MMM.`, { locale: ptBR })),
        agendaTimeFormat: (date: Date) => format(date, 'HH:mm', { locale: ptBR }),
        agendaTimeRangeFormat: ({ start, end }: { start: Date; end: Date }) => `${format(start, 'HH:mm', { locale: ptBR })} - ${format(end, 'HH:mm', { locale: ptBR, })}`
    };

    const [date, setDate] = useState<Date>(new Date());
    const [view, setView] = useState<View>('month');
    const [availableViews, setAvailableViews] = useState<Array<View>>(['month', 'week', 'day', 'agenda']);

    // Responsividade;
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

    // Buscar toda vez no back-end ao alterar a opção de mês e/ou ano;
    const handleGetSchedules = useCallback(
        async (companyId: Guid, year: number = 0, month: number = 0, view: View) => {
            const items = (await Fetch.get({
                url: `${CONSTS_SCHEDULE.getAllByCompanyId}?companyId=${companyId}&year=${year}&month=${month}`,
                setIsRequestLoading: setIsRequestLoading,
            })) as iSchedule[];

            const events = handleMapSchedulesToEvents(items, view) as iEvent[];
            setEvents(events);
        },
        [setIsRequestLoading, setEvents]
    );

    useEffect(() => {
        if (!date || !companyId) {
            return;
        }

        const year = date.getFullYear();
        const month = date.getMonth() + 1; // 0-based;

        handleGetSchedules(companyId, year, month, view);
    }, [date, companyId, view, setEvents, setIsRequestLoading, handleGetSchedules]);

    // Clientes;
    const [clients, setClients] = useState<iClient[]>();
    useApiRequestToSetterOnUrlChange<iClient[]>({ apiUrlRequest: `${CONSTS_CLIENT.getAllByCompanyId}?companyId=${companyId}`, setter: setClients });

    // CompanyUser;
    const [companyUsers, setCompanyUsers] = useState<iUser[]>();
    useApiRequestToSetterOnUrlChange<iUser[]>({ apiUrlRequest: `${CONSTS_COMPANY_USER.getAllByCompanyId}?companyId=${companyId}`, setter: setCompanyUsers });

    // Buscar a query data na URL; caso exista: abra o modal de novo evento na data;
    useEffect(() => {
        if (!queryData || !clients || !companyUsers) {
            return;
        }

        const queryDataNormalizedToDate = handleParseDateFromString(queryData) as Date;
        // console.log(queryData, queryDataNormalizedToDate);

        // remove a query "?data" da URL sem recarregar a página;
        router.replace(pathname);

        const event = {
            start: queryDataNormalizedToDate,
            end: queryDataNormalizedToDate
        } as SlotInfo;

        handleAddNewEvent(event);
    }, [queryData, pathname, router, clients, companyUsers]);

    // Adicionar novo evento;
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
            schedule: {},
            title: 'Novo evento',
            start: event.start,
            end: event.end,
            allDay: false
        } as iEvent;

        setTypeModal('create');
        setEventClicked(newEvent);
        setIsModalOpen(true);
    }

    // Visualizar evento;
    function handleCheckEvent(event: iEvent) {
        setTypeModal('edit');
        setEventClicked(event);
        setIsModalOpen(true);
    }

    return (
        <Fragment>
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
                    style={{ height: customElementHeight || '85vh' }}
                    culture='pt-BR'
                    messages={messages}
                    formats={formats}
                    selectable={true}
                    onSelectSlot={(e) => handleAddNewEvent(e)}
                    onSelectEvent={(e) => handleCheckEvent(e)}
                    dayPropGetter={(date) => { // Quadrados;
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
                    eventPropGetter={(event) => { // Eventos;
                        const today = new Date();
                        const eventEnd = new Date(event.end);

                        // Se o evento terminou antes de hoje → cor diferenciada;
                        if (eventEnd < today) {
                            return {
                                className: 'rbc-event-past'
                            };
                        }

                        return {};
                    }}
                />
            </div>

            <ModalCalendarView
                isOpen={isModalOpen}
                setModalIsOpen={setIsModalOpen}
                type={typeModal}
                event={eventClicked}
                companyId={companyId}
                companyUsers={companyUsers}
                clients={clients}
                handleGetSchedules={() => handleGetSchedules(companyId, date.getFullYear(), date.getMonth() + 1, view)}
            />
        </Fragment>
    )
}

export function handleMapSchedulesToEvents(schedules: iSchedule[], view: View): iEvent[] {
    return schedules.map((schedule) => {
        // console.log('handleMapSchedulesToEvents', schedule);
        const start = new Date(schedule.dateStart);
        const end = schedule.dateEnd ? new Date(schedule.dateEnd) : start;

        let title = schedule.customTitle ? schedule.customTitle : '';

        if (!title) {
            const client = schedule.client as iClient;
            const clientName = client && client?.fullName ? client.fullName : '';
            title = clientName;
        }

        if (view === 'month') {
            const startNormalized = handleFormatDate(start, DATE_STYLE.HORA_MINUTO);
            // const endNormalized = handleFormatDate(end, DATE_STYLE.HORA_MINUTO);
            // title = `${startNormalized} - ${endNormalized} - ${title}`;

            title = `${startNormalized} - ${title}`;
        }

        const isAllDay = start.getHours() === 0 && start.getMinutes() === 0 && start.getSeconds() === 0 && end.getHours() === 23 && end.getMinutes() === 59;

        return {
            schedule,
            title: title,
            start: start,
            end: end,
            allDay: isAllDay
        };
    })
}