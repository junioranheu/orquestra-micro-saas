import { iEvent } from '@/app/components/calendar/complete';
import ModalGeneric from '@/app/components/modal/generic';
import { DATE_STYLE, handleFormatDate } from '@/app/functions/format.date';
import { Dispatch, SetStateAction } from 'react';
import styles from './index.module.scss';

interface iProps {
    isOpen: boolean;
    setModalIsOpen: Dispatch<SetStateAction<boolean>>;
    event: iEvent | undefined;
}

export default function ModalCalendarView({ isOpen, setModalIsOpen, event }: iProps) {

    if (!event) {
        return;
    }

    return (
        <ModalGeneric
            isOpen={isOpen}
            setModalIsOpen={setModalIsOpen}
            showCloseButton={true}
            allowCloseOutsideClick={false}
        >
            <div className={styles.main}>
                <div className={styles.header}>
                    <div>
                        <span>{event.schedule.client?.fullName}</span>
                    </div>

                    <div>
                        <span>{event.start.toLocaleDateString('pt-BR')}</span>
                        <span>{handleFormatDate(event.start, DATE_STYLE.DETALHADO)}</span>
                    </div>
                </div>

                <div className={styles.separator} />

                <h1>xd</h1>
            </div >
        </ModalGeneric >
    )
}