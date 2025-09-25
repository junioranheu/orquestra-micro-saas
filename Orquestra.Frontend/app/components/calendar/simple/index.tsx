'use client';
import ROUTES from '@/app/consts/routes';
import { DATE_STYLE, handleFormatDate } from '@/app/functions/format.date';
import { handleCapitalizeFirstLetter } from '@/app/functions/get.formatUserName';
import swal from '@/app/functions/swal';
import { useRouter } from 'next/navigation';
import { useEffect, useMemo, useRef, useState } from 'react';
import styles from './index.module.scss';

interface iProps {
    isReadOnly: boolean;
    disablePastDays: boolean;
    resetBorderRadiusRight?: boolean;
    removeBorderLeft?: boolean;
    removeBorderRight?: boolean;
}

export default function CalendarSimple({ isReadOnly, disablePastDays, resetBorderRadiusRight = false, removeBorderLeft = false, removeBorderRight = false }: iProps) {

    const router = useRouter();

    const today = new Date();
    const [viewDate, setViewDate] = useState<Date>(new Date(today.getFullYear(), today.getMonth(), 1));
    const [selectedDate, setSelectedDate] = useState<Date | null>(today);

    function handleGetMonthNames(): string[] {
        return ['janeiro', 'fevereiro', 'março', 'abril', 'maio', 'junho', 'julho', 'agosto', 'setembro', 'outubro', 'novembro', 'dezembro'];
    }

    function handlePrevMonth(): void {
        setViewDate(prev => new Date(prev.getFullYear(), prev.getMonth() - 1, 1));
    }

    function handleNextMonth(): void {
        setViewDate(prev => new Date(prev.getFullYear(), prev.getMonth() + 1, 1));
    }

    // Monday-first index (0 = Monday);
    function handleGetFirstWeekdayIndex(date: Date): number {
        const js = new Date(date.getFullYear(), date.getMonth(), 1).getDay(); // 0 = Sun;
        return (js + 6) % 7;
    }

    const grid = useMemo(() => {
        const year = viewDate.getFullYear();
        const month = viewDate.getMonth();
        const firstIndex = handleGetFirstWeekdayIndex(viewDate);
        const daysInCurrent = new Date(year, month + 1, 0).getDate();
        const daysInPrev = new Date(year, month, 0).getDate();
        const cells: { date: Date; inCurrent: boolean }[] = [];

        // Prev tail;
        for (let i = firstIndex - 1; i >= 0; i--) {
            const d = daysInPrev - i;
            cells.push({ date: new Date(year, month - 1, d), inCurrent: false });
        }

        // Current;
        for (let d = 1; d <= daysInCurrent; d++) {
            cells.push({ date: new Date(year, month, d), inCurrent: true });
        }

        // Next head;
        let next = 1;
        while (cells.length < 42) {
            cells.push({ date: new Date(year, month + 1, next++), inCurrent: false });
        }

        return cells;
    }, [viewDate]);

    function handleIsSameDay(a: Date, b: Date | null): boolean {
        if (!b) {
            return false;
        }

        return a.getFullYear() === b.getFullYear() && a.getMonth() === b.getMonth() && a.getDate() === b.getDate();
    }

    function handleCheckIsBeforeDay(a: Date, b: Date): boolean {
        return a.getFullYear() < b.getFullYear() || (a.getFullYear() === b.getFullYear() && a.getMonth() < b.getMonth()) || (a.getFullYear() === b.getFullYear() && a.getMonth() === b.getMonth() && a.getDate() < b.getDate());
    }

    const firstRun = useRef<boolean>(true);

    useEffect(() => {
        if (firstRun.current) {
            firstRun.current = false;
            return;
        }

        if (!selectedDate) {
            return;
        }

        const isoDate = selectedDate?.toISOString().split('T')[0];

        swal({
            content: `Você quer agendar um novo compromisso para a ${handleFormatDate(selectedDate ?? undefined, DATE_STYLE.DIA_DA_SEMANA_E_DIA_DO_MES)}?`,
            cancelBtnText: 'Voltar',
            cancelFunction: () => setSelectedDate(null),
            confirmBtnText: 'Agendar',
            confirmFunction: () => router.push(`${ROUTES.EMPRESA_AGENDAMENTOS}?data=${isoDate}`),
            icon: 'question'
        });
    }, [selectedDate, router]);

    return (
        <div
            className={styles.calendar}
            style={{
                ...(resetBorderRadiusRight && {
                    borderTopRightRadius: 0,
                    borderBottomRightRadius: 0,
                    borderRight: 0
                }),
                ...(removeBorderLeft && {
                    borderLeft: 0
                }),
                ...(removeBorderRight && {
                    borderRight: 0
                })
            }}
        >
            <div className={styles.calendarHeader}>
                <button aria-label="Mês anterior" onClick={isReadOnly ? undefined : handlePrevMonth} className={`${styles.iconBtn} ${(isReadOnly && 'notAllowed')}`}>{'‹'}</button>
                <div className={styles.title}>{handleCapitalizeFirstLetter(handleGetMonthNames()[viewDate.getMonth()])} de {viewDate.getFullYear()}</div>
                <button aria-label="Próximo mês" onClick={isReadOnly ? undefined : handleNextMonth} className={`${styles.iconBtn} ${(isReadOnly && 'notAllowed')}`}>{'›'}</button>
            </div>

            <div className={styles.weekdays}>
                {
                    ['seg', 'ter', 'qua', 'qui', 'sex', 'sáb', 'dom'].map(function (w) {
                        return <div key={w} className={styles.weekday}>{w}</div>;
                    })
                }
            </div>

            <div className={styles.days}>
                {
                    grid.map(function (cell, i) {
                        const isPastDay = disablePastDays && handleCheckIsBeforeDay(cell.date, today);

                        const cls = [
                            styles.day,
                            cell.inCurrent ? '' : styles.outside,
                            handleIsSameDay(cell.date, selectedDate) ? styles.selected : '',
                            (isReadOnly || isPastDay) ? 'notAllowed' : ''
                        ].join(' ').trim();

                        return (
                            <button
                                key={i}
                                className={`${cls} ${(isReadOnly && 'notAllowed')}`}
                                onClick={isReadOnly || isPastDay ? undefined : () => setSelectedDate(cell.date)}
                                aria-pressed={handleIsSameDay(cell.date, selectedDate)}
                                // title={`${cell.date.toLocaleDateString()}`}
                                disabled={isReadOnly || isPastDay}
                            >
                                <span className={styles.dayNumber}>{cell.date.getDate()}</span>
                            </button>
                        )
                    })
                }
            </div>
        </div>
    )
} 