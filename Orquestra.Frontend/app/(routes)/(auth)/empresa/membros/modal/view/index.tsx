'use client';
import { CONSTS_COMPANY_USER } from '@/app/api/consts/company-user';
import { Fetch } from '@/app/api/fetch';
import Button from '@/app/components/input/button';
import InputMask from '@/app/components/input/text';
import ModalGeneric from '@/app/components/modal/generic';
import styles from '@/app/components/modal/generic/index.module.scss';
import Tags from '@/app/components/tags';
import SYSTEM from '@/app/consts/system';
import { handleClearFormData } from '@/app/functions/set.formState';
import swal from '@/app/functions/swal';
import { Guid } from 'guid-typescript';
import { Dispatch, SetStateAction, useCallback, useState } from 'react';

interface iModalFilterParams {
    isModalOpen: boolean;
    setIsModalOpen: Dispatch<SetStateAction<boolean>>;
    companyId: Guid | undefined;
}

interface iFormData {
    inviteEmail: string | null;
}

export default function EmpresaMembrosModalView({ isModalOpen, setIsModalOpen, companyId }: iModalFilterParams) {

    const [saving, setSaving] = useState<boolean>(false);

    const [formData, setFormData] = useState<iFormData>({
        inviteEmail: ''
    });

    const handleClose = useCallback(() => {
        setSaving(false);
        setIsModalOpen(false);
        handleClearFormData(setFormData);
    }, [setIsModalOpen]);

    async function handleSave() {
        if (!formData.inviteEmail) {
            swal({ content: SYSTEM.WARN_FILL_OBLIGATORY_FIELDS, icon: 'warning' });
            return;
        }

        setSaving(true);

        if (!companyId) {
            swal({ content: 'Erro interno: O ID da empresa está vazio. Tente novamnete, e se o erro persistir, contate o suporte.', icon: 'error' });
            return;
        }

        const input = { companyId: companyId, email: formData.inviteEmail };
        const output = await Fetch.post({ url: CONSTS_COMPANY_USER.inviteUser, body: input });

        if (output) {
            swal({
                content: `Um convite foi enviado para o e-mail ${formData.inviteEmail}. Esperamos que aceitem o convite o mais rápido possível para que o convidado se torne um membro da sua empresa no ${SYSTEM.NAME}.`,
                icon: 'success'
            });

            handleClose();
            return;
        }

        setSaving(false);
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
            style={{ padding: 0, width: '50rem' }}
        >
            <div className={styles.modalCard}>
                <header className={styles.modalHeader}>
                    <div className={styles.modalHeaderLeft}>
                        <h1 className={styles.inputTitle}>
                            Convidar novo membro
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
                        <InputMask title='E-mail do novo membro da equipe a ser convidado' fieldName='inviteEmail' formData={formData} setFormData={setFormData} isObligatory={true} />
                    </div>
                </main>

                <footer className={styles.modalFooter}>
                    <div className={styles.buttonsRow}>
                        <Button label='Fechar' handleFunction={() => handleClose()} isStyleSimple={true} />
                    </div>

                    <div className={styles.buttonsRow}>
                        <Button label={saving ? 'Convidando...' : 'Enviar convite'} handleFunction={() => handleSave()} isDisabled={saving} />
                    </div>
                </footer>
            </div>
        </ModalGeneric>
    )
}