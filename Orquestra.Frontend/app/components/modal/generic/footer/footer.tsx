import Button from '@/app/components/input/button';
import styles from '@/app/components/modal/generic/index.module.scss';
import { Fragment, SetStateAction } from 'react';

interface iProps {
    type: 'edit' | 'create';
    saving: boolean;
    editing: boolean;
    handleClose: () => void;
    handleSave: () => Promise<void>;
    setEditing: (value: SetStateAction<boolean>) => void;
}

export default function ModalGenericFooter({ type, saving, editing, handleClose, handleSave, setEditing }: iProps) {
    return (
        <footer className={styles.modalFooter}>
            <div className={styles.buttonsRow}>
                <Button label='Fechar' handleFunction={() => handleClose()} styleType='transparent' />
            </div>

            {
                type === 'create' ? (
                    <div className={styles.buttonsRow}>
                        <Button label={saving ? 'Salvando...' : 'Salvar'} handleFunction={() => handleSave()} isDisabled={saving} />
                    </div>
                ) : (
                    <div className={styles.buttonsRow}>
                        {
                            !editing ? (
                                <Fragment>
                                    <Button label='Editar' handleFunction={() => setEditing(true)} />
                                </Fragment>
                            ) : (
                                <Fragment>
                                    <Button label='Cancelar edição' handleFunction={() => setEditing(false)} styleType='transparent' />
                                    <Button label={saving ? 'Salvando...' : 'Salvar'} handleFunction={() => handleSave()} isDisabled={saving} />
                                </Fragment>
                            )
                        }
                    </div>
                )
            }
        </footer>
    )
}