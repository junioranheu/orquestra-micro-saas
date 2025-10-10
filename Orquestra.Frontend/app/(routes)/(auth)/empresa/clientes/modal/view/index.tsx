'use client';
import { iClientFormDataModalFilter } from '@/app/(routes)/(auth)/empresa/clientes/modal/filter';
import iClient from '@/app/api/consts/client';
import ContentLoaderText from '@/app/components/content-loader/text';
import Button from '@/app/components/input/button';
import InputMask from '@/app/components/input/text';
import ModalGeneric from '@/app/components/modal/generic';
import styles from '@/app/components/modal/generic/index.module.scss';
import Tags from '@/app/components/tags';
import SYSTEM from '@/app/consts/system';
import { handleClearFormData, handleLoopFormData } from '@/app/functions/set.formState';
import swal from '@/app/functions/swal';
import { Dispatch, Fragment, SetStateAction, useCallback, useEffect, useState } from 'react';

interface iModalFilterParams {
    isModalOpen: boolean;
    setIsModalOpen: Dispatch<SetStateAction<boolean>>;
    type: 'edit' | 'create';
    client: iClient | undefined;
}

export default function EmpresaClientesModalView({ isModalOpen, setIsModalOpen, type, client }: iModalFilterParams) {

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
        if (!formData.name || !formData.email || !formData.phone || !formData.companyType) {
            swal({ content: SYSTEM.WARN_FILL_OBLIGATORY_FIELDS, icon: 'warning' });
            return;
        }

        setEditing(false);
        setSaving(true);

        const data = handleLoopFormData(formData);
        const input = data.json as iCompanyOutput;
        // console.log(input);

        const formDataInput = new FormData();
        formDataInput.append('CompanyId', input.companyId ? input.companyId.toString() : Guid.create().toString());
        formDataInput.append('Name', input.name);
        formDataInput.append('Email', input.email);

        if (input.phone) {
            const cleanedPhone = input.phone.replace(/[()\s-]/g, '');
            formDataInput.append('Phone', cleanedPhone);
        }

        formDataInput.append('CompanyType', input.companyType);
        if (input.address) formDataInput.append('Address', input.address);
        if (input.city) formDataInput.append('City', input.city);
        if (input.state) formDataInput.append('State', input.state);

        if (input.zipCode) {
            const cleanedZipCode = input.zipCode.replace(/[()\s-]/g, '');
            formDataInput.append('ZipCode', cleanedZipCode);
        }

        if (input.country) formDataInput.append('Country', input.country);
        if (input.color) formDataInput.append('Color', input.color);
        if (input.logoFormFile && input.logoFormFile instanceof File) formDataInput.append('LogoFormFile', input.logoFormFile as Blob, input.logoFormFile.name);
        formDataInput.append('Status', input.status?.toString() ?? 'false');

        // console.log('formDataInput', formDataInput);

        if (type === 'create') {
            const company = await Fetch.post({ url: CONSTS_COMPANY.post, body: formDataInput, isFormData: true }) as iCompanyOutput;

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

        const company = await Fetch.put({ url: CONSTS_COMPANY.put, body: formDataInput, isFormData: true }) as iCompanyOutput;

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
                            {type === 'create' ? (formData.fullName ?? 'Cadastrar novo cliente') : <ContentLoaderText text={(`Editar cliente: ${formData?.fullName ?? client?.fullName}`)} />}
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
                        <InputMask title='Nome' fieldName='fullName' formData={formData} setFormData={setFormData} />
                        <InputMask title='CPF' fieldName='CPF' formData={formData} setFormData={setFormData} />
                        <InputMask title='E-mail' fieldName='email' formData={formData} setFormData={setFormData} />
                        <InputMask title='Telefone' fieldName='phone' formData={formData} setFormData={setFormData} />
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