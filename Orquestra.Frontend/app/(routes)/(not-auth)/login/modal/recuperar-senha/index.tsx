'use client';
import { CONSTS_AUTH } from '@/app/api/consts/auth';
import { Fetch } from '@/app/api/fetch';
import Button from '@/app/components/input/button';
import InputMask from '@/app/components/input/text';
import ModalGeneric from '@/app/components/modal/generic';
import styles from '@/app/components/modal/generic/index.module.scss';
import Tags from '@/app/components/tags';
import SYSTEM from '@/app/consts/system';
import { handleClearFormData } from '@/app/functions/set.formState';
import swal from '@/app/functions/swal';
import { Dispatch, SetStateAction, useCallback, useState } from 'react';

interface iProps {
    isModalOpen: boolean;
    setIsModalOpen: Dispatch<SetStateAction<boolean>>;
}

interface iFormData {
    recoverEmail: string | null;
}

export default function LoginModalRecuperarSenha({ isModalOpen, setIsModalOpen }: iProps) {

    const [sending, setSending] = useState<boolean>(false);

    const [formData, setFormData] = useState<iFormData>({
        recoverEmail: ''
    });

    const handleClose = useCallback(() => {
        setSending(false);
        setIsModalOpen(false);
        handleClearFormData(setFormData);
    }, [setIsModalOpen]);

    async function handleSave() {
        if (!formData.recoverEmail) {
            swal({ content: SYSTEM.WARN_FILL_OBLIGATORY_FIELDS, icon: 'warning' });
            return;
        }

        setSending(true);

        const output = await Fetch.post({ url: `${CONSTS_AUTH.sendRecoverPassword}/${formData.recoverEmail}` });

        if (output) {
            swal({
                content: `Um e-mail de recuperação de senha foi enviado para <b>${formData.recoverEmail}</b>. Caso necessário, cheque sua caixa de spam.
            <br/><br/><b>Atenção a um ponto importantíssimo</b>: ao redefinir a senha, ela será temporariamente definida como seu e-mail de cadastro. Por exemplo, se seu e-mail é joaozinho@gmail.com, sua nova senha será joaozinho@gmail.com.
            <br/><br/>Faça login rapidamente e troque a senha manualmente nas configurações para manter sua conta mais segura.`,
                icon: 'success',
                mustConfirm: true,
                checkboxLabel: 'Li e estou de acordo'
            });

            handleClose();
            return;
        }

        setSending(false);
        return;
    }

    if (!isModalOpen) {
        return;
    }

    return (
        <ModalGeneric
            isOpen={isModalOpen}
            setIsModalOpen={setIsModalOpen}
            onRequestClose={handleClose}
            showCloseButton={false}
            allowCloseOutsideClick={false}
            overlayColor={0.35}
            style={{ padding: 0, width: '50rem', maxWidth: '90%' }}
        >
            <div className={styles.modalCard}>
                <header className={styles.modalHeader}>
                    <div className={styles.modalHeaderLeft}>
                        <h1 className={styles.inputTitle}>
                            Recuperar senha
                        </h1>
                    </div>

                    <div className={styles.modalHeaderRight}>
                        <div className={styles.metaRow}>
                            <Tags
                                tags={[
                                    { label: '✖', color: 'transparent', handleFunction: () => handleClose(), title: 'Fechar' }
                                ]}
                            />
                        </div>
                    </div>
                </header>

                <main className={styles.modalContent}>
                    <div className='modal-layout-flex'>
                        <InputMask title='E-mail' fieldName='recoverEmail' formData={formData} setFormData={setFormData} isObligatory={true} />
                    </div>
                </main>

                <footer className={styles.modalFooter}>
                    <div className={styles.buttonsRow}>
                        <Button label='Fechar' handleFunction={() => handleClose()} isStyleSimple={true} />
                    </div>

                    <div className={styles.buttonsRow}>
                        <Button label={sending ? 'Enviando e-mail de recuperação...' : 'Enviar e-mail de recuperação de senha'} handleFunction={() => handleSave()} isDisabled={sending} />
                    </div>
                </footer>
            </div>
        </ModalGeneric>
    )
}