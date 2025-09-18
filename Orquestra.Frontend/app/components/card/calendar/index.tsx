'use client';
import SvgOne from '@/app/assets/svg/one.svg';
import SvgTwo from '@/app/assets/svg/two.svg';
import CardSimple from '@/app/components/card/simple';
import SYSTEM from '@/app/consts/system';
import { useMemo, useState } from 'react';
import styles from './index.module.scss';

export default function CardCalendar() {

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

    return (
        <section className={styles.wrapper}>
            <aside className={styles.calendar}>
                <div className={styles.calendarHeader}>
                    <button aria-label="Mês anterior" onClick={handlePrevMonth} className={styles.iconBtn}>{'‹'}</button>
                    <div className={styles.title}>{handleGetMonthNames()[viewDate.getMonth()]} {viewDate.getFullYear()}</div>
                    <button aria-label="Próximo mês" onClick={handleNextMonth} className={styles.iconBtn}>{'›'}</button>
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
                            const cls = [
                                styles.day,
                                cell.inCurrent ? '' : styles.outside,
                                handleIsSameDay(cell.date, selectedDate) ? styles.selected : ''
                            ].join(' ').trim();

                            return (
                                <button
                                    key={i}
                                    className={cls}
                                    onClick={function () { setSelectedDate(cell.date); }}
                                    aria-pressed={handleIsSameDay(cell.date, selectedDate)}
                                    title={`${cell.date.toLocaleDateString()}`}
                                >
                                    <span className={styles.dayNumber}>{cell.date.getDate()}</span>
                                </button>
                            )
                        })
                    }
                </div>
            </aside>

            <main className={styles.panel}>
                <div className={styles.panelInner}>
                    <div className={styles.panelHeader}>
                        <h2>Comece a usar o {SYSTEM.NAME}</h2>
                    </div>

                    <div className={styles.steps}>
                        <CardSimple
                            img={SvgOne}
                            title='Adicione seus primeiros contatos'
                            description='Você precisa de contatos para criar uma campanha. Crie seu banco de dados de contatos ou adicione os destinatários da sua primeira campanha.'
                            buttonLabel='Importar seus contatos'
                            buttonFunction={() => alert('xd')}
                        />

                        <CardSimple
                            img={SvgTwo}
                            title='Crie sua primeira campanha'
                            description='É hora de ser criativo e criar uma campanha. Precisa de inspiração? Escolha um modelo e use nosso assistente de redação com IA.'
                            buttonLabel='Crie sua primeira campanha'
                            buttonFunction={() => alert('xd')}
                        />
                    </div>
                </div>
            </main>
        </section>
    )
}