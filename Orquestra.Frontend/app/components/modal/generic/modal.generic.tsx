import SYSTEM from '@/app/consts/system';
import { handleDisableScroll } from '@/app/hooks/useDisableScroll';
import useKeyPress from '@/app/hooks/useKeyPress';
import { CSSProperties, Dispatch, ReactNode, SetStateAction, useEffect } from 'react';
import Modal from 'react-modal';
import styles from './modal.generic.module.scss';

interface iParams {
    isOpen: boolean;
    setModalIsOpen: Dispatch<SetStateAction<boolean>>
    onRequestClose?: () => void;
    width?: string | number;
    height?: string | number;
    borderRadius?: string | number;
    showCloseButton?: boolean;
    customStyle?: CSSProperties;
    customClass?: string;
    overlayColor?: number;
    allowCloseOutsideClick?: boolean;
    title?: string;
    customPosition?: iModalCustomPosition;
    children: ReactNode;
}

export interface iModalCustomPosition {
    top?: number;
    left?: number;
}

export default function ModalGeneric({
    isOpen,
    setModalIsOpen,
    onRequestClose,
    width = 'auto',
    height = 'auto',
    borderRadius = 'var(--border-radius)',
    showCloseButton = true,
    customStyle,
    customClass,
    overlayColor = 0.2,
    allowCloseOutsideClick = true,
    title,
    customPosition = {},
    children
}: iParams) {

    useKeyPress('Escape', () => setModalIsOpen(false));

    useEffect(() => {
        if (isOpen) {
            handleDisableScroll();
        }

        return () => {
            handleDisableScroll(false);
        };
    }, [isOpen]);

    function handleClose(isCloseIconClick: boolean = false) {
        if (!allowCloseOutsideClick && !isCloseIconClick) {
            return;
        }

        setModalIsOpen(false);
        onRequestClose && onRequestClose();
    }

    return (
        <Modal
            isOpen={isOpen}
            onRequestClose={() => handleClose()}
            style={{
                content: {
                    minWidth: width,
                    minHeight: height,
                    borderRadius,
                    top: customPosition?.top,
                    left: customPosition?.left,
                    ...customStyle,
                },
                overlay: {
                    backgroundColor: `rgba(0, 0, 0, ${overlayColor})`,
                }
            }}
            className={`${styles.modal} ${customClass} ${SYSTEM.ANIMATE_FAST}`}
            ariaHideApp={false}
        >

            {
                (title || showCloseButton) && (
                    <div
                        className={styles.top}
                        style={{ marginBottom: (title ? '1rem' : '0rem') }}
                    >
                        {
                            title && (
                                <h3 dangerouslySetInnerHTML={{ __html: title }} />
                            )
                        }

                        {
                            showCloseButton && (
                                <button className='close' onClick={() => handleClose(true)}>
                                    ✖
                                </button>
                            )
                        }
                    </div>
                )
            }

            <section>
                {children}
            </section>
        </Modal>
    )
}