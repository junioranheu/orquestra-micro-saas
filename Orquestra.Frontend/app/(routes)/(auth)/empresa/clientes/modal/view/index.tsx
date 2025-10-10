'use client';
import { iClientFormDataModalFilter } from '@/app/(routes)/(auth)/empresa/clientes/modal/filter';
import iClient, { CONSTS_CLIENT } from '@/app/api/consts/client';
import { Fetch } from '@/app/api/fetch';
import ContentLoaderText from '@/app/components/content-loader/text';
import Button from '@/app/components/input/button';
import InputMask from '@/app/components/input/text';
import ModalGeneric from '@/app/components/modal/generic';
import styles from '@/app/components/modal/generic/index.module.scss';
import Tags from '@/app/components/tags';
import SYSTEM from '@/app/consts/system';
import { handleClearFormData, handleLoopFormData } from '@/app/functions/set.formState';
import swal from '@/app/functions/swal';
import { Guid } from 'guid-typescript';
import { Dispatch, Fragment, SetStateAction, useCallback, useEffect, useState } from 'react';

interface iModalFilterParams {
    isModalOpen: boolean;
    setIsModalOpen: Dispatch<SetStateAction<boolean>>;
    type: 'edit' | 'create';
    client: iClient | undefined;
    companyId: Guid | undefined;
}

export default function EmpresaClientesModalView({ isModalOpen, setIsModalOpen, type, client, companyId }: iModalFilterParams) {

    const [editing, setEditing] = useState<boolean>(false);
    const [saving, setSaving] = useState<boolean>(false);

    const [formData, setFormData] = useState<iClientFormDataModalFilter>({
        fullName: null,
        email: null,
        CPF: null,
        address: null,
        dateOfBirth: null,
        notes: null,
        phone: null
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

        if (type === 'create') {
            setEditing(true);
            return;
        }

        if (!isModalOpen || !client) {
            return;
        }
    }, [isModalOpen, type, client, setIsModalOpen, handleClose]);

    async function handleSave() {
        if (!formData.fullName || !formData.CPF) {
            swal({ content: SYSTEM.WARN_FILL_OBLIGATORY_FIELDS, icon: 'warning' });
            return;
        }

        setEditing(false);
        setSaving(true);

        const data = handleLoopFormData(formData);
        const input = data.json as iClient;

        if (input.phone) {
            const cleanedPhone = input.phone.replace(/[()\s-]/g, '');
            input.phone = cleanedPhone;
        }

        if (!companyId) {
            swal({ content: 'Erro interno: O ID da empresa está vazio. Tente novamnete, e se o erro persistir, contate o suporte.', icon: 'error' });
            return;
        }

        input.companyId = companyId;

        // console.log(input);

        if (type === 'create') {
            const company = await Fetch.post({ url: CONSTS_CLIENT.post, body: input }) as iClient;

            if (company) {
                swal({
                    content: 'Cliente registrado com sucesso.',
                    confirmFunction: () => window.location.reload(),
                    icon: 'success'
                });

                handleClose();
                return;
            }

            setEditing(true);
            setSaving(false);
            return;
        }

        if (!client?.clientId) {
            swal({ content: 'Erro interno: O ID do cliente está vazio. Tente novamnete, e se o erro persistir, contate o suporte.', icon: 'error' });
            return;
        }

        input.clientId = client.clientId;
        const company = await Fetch.put({ url: CONSTS_CLIENT.put, body: input }) as iClient;

        if (company) {
            swal({
                content: 'Cliente atualizado com sucesso.',
                confirmFunction: () => window.location.reload(),
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
            style={{ padding: 0, width: '50rem' }}
        >
            <div className={styles.modalCard}>
                <header className={styles.modalHeader}>
                    <div className={styles.modalHeaderLeft}>
                        <h1 className={styles.inputTitle}>
                            {type === 'create' ? (formData.fullName ? `Novo cliente: ${formData.fullName}` : 'Cadastrar novo cliente') : <ContentLoaderText text={(`Editar cliente: ${formData?.fullName ?? client?.fullName}`)} />}
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
                        <InputMask title='Nome' fieldName='fullName' formData={formData} setFormData={setFormData} isObligatory={true} />
                        <InputMask title='CPF' fieldName='CPF' formData={formData} setFormData={setFormData} isObligatory={true} />
                        <InputMask title='E-mail' fieldName='email' formData={formData} setFormData={setFormData} />
                        <InputMask title='Telefone' fieldName='phone' formData={formData} setFormData={setFormData} mask='(00) 00000-0000' />
                        <InputMask title='Endereço' fieldName='address' formData={formData} setFormData={setFormData} />
                        <InputMask type='date' title='Data de aniversário' fieldName='dateOfBirth' formData={formData} setFormData={setFormData} />
                        <InputMask title='Anotações' fieldName='notes' formData={formData} setFormData={setFormData} />
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