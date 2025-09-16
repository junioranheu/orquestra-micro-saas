import ModalGeneric, { iModalCustomPosition } from '@/app/components/modal/generic';
import { Dispatch, SetStateAction } from 'react';
import styles from './index.module.scss';

interface iProps {
    isOpen: boolean;
    setModalIsOpen: Dispatch<SetStateAction<boolean>>;
    customPosition: iModalCustomPosition;
}

export default function ModalSettings({ isOpen, setModalIsOpen, customPosition }: iProps) {
    return (
        <ModalGeneric
            isOpen={isOpen}
            setModalIsOpen={setModalIsOpen}
            customPosition={customPosition}
            showCloseButton={false}
            overlayColor={0}
        >
            <div className={styles.main}>
                aea
            </div>
        </ModalGeneric>
    )
}