import useKeyPress from '@/app/hooks/useKeyPress';
import Tippy from '@tippyjs/react';
import { CSSProperties, Dispatch, ReactNode, SetStateAction } from 'react';
import Modal from 'react-modal';
import styles from './index.module.scss';

interface iProps {
    isOpen: boolean;
    setIsModalOpen: Dispatch<SetStateAction<boolean>>
    onRequestClose?: () => void;
    width?: string | number;
    height?: string | number;
    borderRadius?: string | number;
    showCloseButton?: boolean;
    className?: string;
    style?: CSSProperties;
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
    setIsModalOpen,
    onRequestClose,
    width = 'auto',
    height = 'auto',
    borderRadius = 'var(--border-radius)',
    showCloseButton = true,
    className,
    style = {},
    overlayColor = 0.2,
    allowCloseOutsideClick = true,
    title,
    customPosition = {},
    children
}: iProps) {

    useKeyPress('Escape', () => setIsModalOpen(false));

    function handleClose(isCloseIconClick: boolean = false) {
        if (!allowCloseOutsideClick && !isCloseIconClick) {
            return;
        }

        setIsModalOpen(false);

        if (onRequestClose) {
            onRequestClose();
        }
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
                    ...style,
                },
                overlay: {
                    backgroundColor: `rgba(0, 0, 0, ${overlayColor})`,
                }
            }}
            className={`${styles.modal} ${className}`}
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
                                <Tippy content='Fechar'>
                                    <button className='close' onClick={() => handleClose(true)}>
                                        ✖
                                    </button>
                                </Tippy>
                            )
                        }
                    </div>
                )
            }

            <section id='modal-settings'>
                {children}
            </section>
        </Modal>
    )
}