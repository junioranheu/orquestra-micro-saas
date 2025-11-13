'use client';
import iClientFollowUp, { CONSTS_CLIENT_FOLLOW_UP } from '@/app/api/consts/client-follow-up';
import { Fetch } from '@/app/api/fetch';
import Button from '@/app/components/input/button';
import InputImage from '@/app/components/input/image';
import InputMask from '@/app/components/input/text';
import ModalGeneric from '@/app/components/modal/generic';
import styles from '@/app/components/modal/generic/index.module.scss';
import Tags from '@/app/components/tags';
import SYSTEM from '@/app/consts/system';
import { handleConvertBase64ListToFiles } from '@/app/functions/convert.base64ToFile';
import { handleClearFormData, handleLoopFormData } from '@/app/functions/set.formState';
import swal from '@/app/functions/swal';
import { Guid } from 'guid-typescript';
import { Dispatch, Fragment, SetStateAction, useCallback, useEffect, useState } from 'react';

interface iProps {
    isModalOpen: boolean;
    setIsModalOpen: Dispatch<SetStateAction<boolean>>;
    type: 'edit' | 'create';
    clientId: Guid | undefined;
    followUpClicked: iClientFollowUp | undefined;
    setTrigger: Dispatch<SetStateAction<Date>>;
}

export default function EmpresaClientesModalFollowUp({ isModalOpen, setIsModalOpen, type, clientId, followUpClicked, setTrigger }: iProps) {

    const [editing, setEditing] = useState<boolean>(false);
    const [saving, setSaving] = useState<boolean>(false);

    const [formData, setFormData] = useState<iClientFollowUp>({
        clientId: SYSTEM.EMPTY_GUID,
        observation: '',
        clientFollowUpStatus: '',
        imagesFormFile: [],
        imagesBase64: []
    });

    const handleClose = useCallback(() => {
        setSaving(false);
        setEditing(false);
        setIsModalOpen(false);
        handleClearFormData(setFormData);
    }, [setIsModalOpen]);

    useEffect(() => {
        handleClearFormData(setFormData);
        setSaving(false);
        setEditing(false);

        if (!isModalOpen || !clientId) {
            return;
        }

        if (type === 'create') {
            setEditing(true);
        }

        if (type === 'edit') {
            if (!followUpClicked) {
                return;
            }
        }

        setFormData({
            clientId: clientId,
            observation: followUpClicked ? followUpClicked.observation : '',
            clientFollowUpStatus: followUpClicked ? followUpClicked.clientFollowUpStatus : '',
            imagesBase64: followUpClicked?.imagesBase64 ?? [],
            imagesFormFile: followUpClicked?.imagesBase64 ?
                handleConvertBase64ListToFiles(
                    followUpClicked.imagesBase64.map(img => ({
                        base64: img,
                        filename: 'img',
                    }))
                ) : [],
        });
    }, [isModalOpen, type, clientId, followUpClicked, setIsModalOpen, handleClose]);

    async function handleSave() {
        if (!formData.clientId || !formData.clientFollowUpStatus) {
            swal({ content: SYSTEM.WARN_FILL_OBLIGATORY_FIELDS, icon: 'warning' });
            return;
        }

        if (!formData.observation && !formData.imagesFormFile) {
            swal({ content: 'Adicione pelo menos a observação ou um anexo.', icon: 'warning' });
            return;
        }

        setEditing(false);
        setSaving(true);

        const data = handleLoopFormData(formData);
        const input = data.json as iClientFollowUp;
        // console.log(input);

        if (type === 'create') {
            const output = await Fetch.post({ url: CONSTS_CLIENT_FOLLOW_UP.post, body: input });

            if (output) {
                swal({
                    content: 'Acompanhamento registrado com sucesso.',
                    confirmFunction: () => setTrigger(new Date()),
                    icon: 'success'
                });

                handleClose();
                return;
            }

            setEditing(true);
            setSaving(false);
            return;
        }

        const output = await Fetch.put({ url: CONSTS_CLIENT_FOLLOW_UP.put, body: input });

        if (output) {
            swal({
                content: 'Acompanhamento atualizado com sucesso.',
                confirmFunction: () => setTrigger(new Date()),
                icon: 'success'
            });

            handleClose();
            return;
        }

        setEditing(true);
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
            style={{ padding: 0, width: '50rem', maxWidth: '90%' }}
        >
            <div className={styles.modalCard}>
                <header className={styles.modalHeader}>
                    <div className={styles.modalHeaderLeft}>
                        <h1 className={styles.inputTitle}>
                            {type === 'create' ? 'Cadastrar novo acompanhamento' : 'Editar acompanhamento'}
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
                    <div className='modal-layout-grid'>
                        <InputMask title='Observações' fieldName='observation' formData={formData} setFormData={setFormData} isDisabled={!editing} />
                        <InputMask title='Status' fieldName='clientFollowUpStatus' formData={formData} setFormData={setFormData} isDisabled={!editing} />
                        <InputImage title='Logo' fieldName='imagesFormFile' formData={formData} setFormData={setFormData} isDisabled={!editing} placeholder='Selecionar anexos' multiple={true} />
                    </div>
                </main>

                <footer className={styles.modalFooter}>
                    <div className={styles.buttonsRow}>
                        <Button label='Fechar' handleFunction={() => handleClose()} isStyleSimple={true} />
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
                                            <Button label='Cancelar edição' handleFunction={() => setEditing(false)} isStyleSimple={true} />
                                            <Button label={saving ? 'Salvando...' : 'Salvar'} handleFunction={() => handleSave()} isDisabled={saving} />
                                        </Fragment>
                                    )
                                }
                            </div>
                        )
                    }
                </footer>
            </div>
        </ModalGeneric>
    )
}