import { iEvent } from '@/app/components/calendar/complete';
import ModalGeneric from '@/app/components/modal/generic';
import { Dispatch, Fragment, SetStateAction, useMemo, useState } from 'react';
import styles from './index.module.scss';

interface iProps {
    isOpen: boolean;
    setModalIsOpen: Dispatch<SetStateAction<boolean>>;
    event: iEvent | undefined;
    onSave?: (updated: iEvent) => Promise<void> | void;
}

export default function ModalCalendarView({ isOpen, setModalIsOpen, event, onSave }: iProps) {

    if (!event) {
        return;
    }

    const [editing, setEditing] = useState(false);
    const [saving, setSaving] = useState(false);

    // form state
    const [title, setTitle] = useState(event.title ?? '');
    const [clientName, setClientName] = useState(event.schedule.client?.fullName ?? '');
    const [date, setDate] = useState(formatDateInput(event.start));
    const [startTime, setStartTime] = useState(formatTimeInput(event.start));
    const [endTime, setEndTime] = useState(formatTimeInput(event.end));
    const [duration, setDuration] = useState(String(event.schedule.durationMinutes ?? 60));
    const [paymentType, setPaymentType] = useState(event.schedule.paymentType ?? '');
    const [status, setStatus] = useState(event.schedule.scheduleStatus ?? '');
    const [observations, setObservations] = useState((event.schedule.observations ?? []).join('\n'));

    const startDateObj = useMemo(() => toDateFromInputs(date, startTime), [date, startTime]);
    const endDateObj = useMemo(() => toDateFromInputs(date, endTime), [date, endTime]);

    const canSave = !saving && title.trim().length > 0 && startDateObj < endDateObj;

    function handleClose() {
        setModalIsOpen(false);
    }

    async function handleSave() {
        if (!canSave) return;
        setSaving(true);
        try {
            const updated = {
                ...event,
                title: title.trim(),
                start: startDateObj,
                end: endDateObj,
                schedule: {
                    ...event?.schedule
                }
                // schedule: {
                //     ...event?.schedule,
                //     date: startDateObj,
                //     dateEnd: endDateObj,
                //     durationMinutes: Math.max(1, Number(duration) || 60),
                //     paymentType,
                //     scheduleStatus: status,
                //     // client: { fullName: clientName },
                //     observations: observations.split('\n').map(s => s.trim()).filter(Boolean),
                // },
            } as unknown as iEvent;

            const maybePromise = onSave?.(updated);
            if (maybePromise && typeof (maybePromise as any).then === 'function') await maybePromise;
            setEditing(false);
        } catch (err) {
            console.error('save failed', err);
            // aqui você pode integrar com um swal/toast
        } finally {
            setSaving(false);
        }
    }

    function pad(n: number) {
        return String(n).padStart(2, '0');
    }

    function formatDateInput(d: Date) {
        const dt = new Date(d);
        return `${dt.getFullYear()}-${pad(dt.getMonth() + 1)}-${pad(dt.getDate())}`;
    }

    function formatTimeInput(d: Date) {
        const dt = new Date(d);
        return `${pad(dt.getHours())}:${pad(dt.getMinutes())}`;
    }

    function toDateFromInputs(dateStr: string, timeStr: string) {
        // dateStr: YYYY-MM-DD, timeStr: HH:MM
        return new Date(`${dateStr}T${timeStr}:00`);
    }

    return (
        <ModalGeneric
            isOpen={isOpen}
            setModalIsOpen={setModalIsOpen}
            showCloseButton={true}
            allowCloseOutsideClick={false}
            width='60%'
        >
            <div className={styles.card}>
                <header className={styles.header}>
                    <div className={styles.headerLeft}>
                        <div>
                            <input className={styles.inputTitle} value={title} onChange={e => setTitle(e.target.value)} readOnly={!editing} />
                        </div>
                    </div>

                    <div className={styles.headerRight}>
                        <div className={styles.metaRow}>
                            <label className={styles.label}>Data</label>
                            <input type='date' value={date} onChange={e => setDate(e.target.value)} className={styles.input} readOnly={!editing} />
                        </div>

                        <div className={styles.headerActions}>
                            {
                                !editing ? (
                                    <button className={styles.btn} onClick={() => setEditing(true)}>Editar</button>
                                ) : (
                                    <Fragment>
                                        <button className={`${styles.btn} ${styles.ghost}`} onClick={() => { /* reset to original values */ setEditing(false); setTitle(event.title); setClientName(event.schedule.client?.fullName ?? ''); setDate(formatDateInput(event.start)); setStartTime(formatTimeInput(event.start)); setEndTime(formatTimeInput(event.end)); setDuration(String(event.schedule.durationMinutes ?? 60)); setPaymentType(event.schedule.paymentType ?? ''); setStatus(event.schedule.scheduleStatus ?? ''); setObservations((event.schedule.observations ?? []).join('\n')); }}>Cancelar</button>
                                        <button className={`${styles.btn} ${styles.primary}`} onClick={handleSave} disabled={!canSave || saving}>{saving ? 'Salvando...' : 'Salvar'}</button>
                                    </Fragment>
                                )
                            }
                        </div>
                    </div>
                </header>

                <main className={styles.content}>
                    <div className={styles.grid}>
                        <div className={styles.fieldGroup}>
                            <label className={styles.labelSmall}>Cliente</label>
                            <input className={styles.input} value={clientName} onChange={e => setClientName(e.target.value)} readOnly={!editing} />

                            <label className={styles.labelSmall}>Início</label>
                            <input type='time' value={startTime} onChange={e => setStartTime(e.target.value)} className={styles.input} readOnly={!editing} />

                            <label className={styles.labelSmall}>Fim</label>
                            <input type='time' value={endTime} onChange={e => setEndTime(e.target.value)} className={styles.input} readOnly={!editing} />

                            <label className={styles.labelSmall}>Duração (min)</label>
                            <input type='number' min={1} className={styles.input} value={duration} onChange={e => setDuration(e.target.value)} readOnly={!editing} />
                        </div>

                        <div className={styles.fieldGroup}>
                            <label className={styles.labelSmall}>Tipo de pagamento</label>
                            <select className={styles.input} value={paymentType} onChange={e => setPaymentType(e.target.value)} disabled={!editing}>
                                <option value=''>—</option>
                                <option value='cartao'>Cartão</option>
                                <option value='pix'>PIX</option>
                                <option value='dinheiro'>Dinheiro</option>
                                <option value='boleto'>Boleto</option>
                            </select>

                            <label className={styles.labelSmall}>Status</label>
                            <select className={styles.input} value={status} onChange={e => setStatus(e.target.value)} disabled={!editing}>
                                <option value=''>—</option>
                                <option value='scheduled'>Agendado</option>
                                <option value='confirmed'>Confirmado</option>
                                <option value='cancelled'>Cancelado</option>
                                <option value='done'>Concluído</option>
                            </select>

                            <label className={styles.labelSmall}>Usuários (ids)</label>
                            <input className={styles.input} value={event.schedule.usersIds.join(', ')} readOnly />
                        </div>
                    </div>

                    <label className={styles.labelSmall}>Observações</label>
                    <textarea className={styles.textarea} rows={4} value={observations} onChange={e => setObservations(e.target.value)} readOnly={!editing} />
                </main>

                <footer className={styles.footer}>
                    <div className={styles.buttonsRow}>
                        <button className={styles.ghost} onClick={handleClose}>Fechar</button>
                    </div>
                </footer>
            </div>
        </ModalGeneric>
    )
}