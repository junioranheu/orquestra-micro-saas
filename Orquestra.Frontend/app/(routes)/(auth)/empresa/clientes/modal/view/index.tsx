'use client';
import { iClientFormDataModalFilter } from '@/app/(routes)/(auth)/empresa/clientes/modal/filter';
import iClient, { CONSTS_CLIENT } from '@/app/api/consts/client';
import { CONSTS_UTILITY } from '@/app/api/consts/utility';
import { Fetch } from '@/app/api/fetch';
import ContentLoaderText from '@/app/components/content-loader/text';
import Button from '@/app/components/input/button';
import Dropdown, { iDropdownOption } from '@/app/components/input/drop-down';
import InputMask from '@/app/components/input/text';
import ModalGeneric from '@/app/components/modal/generic';
import styles from '@/app/components/modal/generic/index.module.scss';
import Tags from '@/app/components/tags';
import SYSTEM from '@/app/consts/system';
import { handleFetchCEP } from '@/app/functions/fetch.CEP';
import { handleFormatDateToInputValue } from '@/app/functions/format.date';
import { handleFormatCPF, handleGetOnlyNumbers } from '@/app/functions/format.string';
import handleGetPropName from '@/app/functions/get.propName';
import { handleClearFormData, handleLoopFormData, handleSetDropdownOption } from '@/app/functions/set.formState';
import swal from '@/app/functions/swal';
import useApiRequestToSetterOnUrlChange from '@/app/hooks/api/useApiRequestToSetterOnUrlChange';
import { Guid } from 'guid-typescript';
import { Dispatch, Fragment, KeyboardEvent, SetStateAction, useCallback, useEffect, useState } from 'react';

interface iProps {
    isModalOpen: boolean;
    setIsModalOpen: Dispatch<SetStateAction<boolean>>;
    type: 'edit' | 'create';
    client: iClient | undefined;
    companyId: Guid | undefined;
    setTrigger: Dispatch<SetStateAction<Date>>;
}

export default function EmpresaClientesModalView({ isModalOpen, setIsModalOpen, type, client, companyId, setTrigger }: iProps) {

    const [editing, setEditing] = useState<boolean>(false);
    const [saving, setSaving] = useState<boolean>(false);

    const [formData, setFormData] = useState<iClientFormDataModalFilter>({
        fullName: null,
        email: null,
        cpf: null,
        address: '',
        addressNumber: '',
        city: '',
        state: '',
        zipCode: '',
        country: '',
        dateOfBirth: null,
        notes: null,
        phone: null
    });

    const [countries, setCountries] = useState<string[] | undefined>([]);
    useApiRequestToSetterOnUrlChange<string[]>({ apiUrlRequest: CONSTS_UTILITY.getCountry, setter: setCountries });

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

        setFormData({
            fullName: client ? client.fullName : null,
            email: client ? client.email : null,
            cpf: client ? handleFormatCPF(client.cpf) : null,
            address: client && client.address ? client.address : null,
            addressNumber: client && client.addressNumber ? client.addressNumber : null,
            city: client && client.city ? client.city : null,
            state: client && client.state ? client.state : null,
            zipCode: client && client.zipCode ? client.zipCode : null,
            country: client && client.country ? client.country : null,
            dateOfBirth: client && client?.dateOfBirth ? handleFormatDateToInputValue(new Date(client?.dateOfBirth)) : null,
            notes: client && client?.notes ? client.notes : null,
            phone: client && client?.phone ? client.phone : null
        });
    }, [isModalOpen, type, client, setIsModalOpen, handleClose]);

    const setCountryOption = handleSetDropdownOption(formData, setFormData, handleGetPropName(formData, x => x.country ?? '')[1]) as Dispatch<SetStateAction<iDropdownOption[]>>;

    async function handleSave() {
        if (!formData.fullName || !formData.cpf) {
            swal({ content: SYSTEM.WARN_FILL_OBLIGATORY_FIELDS, icon: 'warning' });
            return;
        }

        setEditing(false);
        setSaving(true);

        const data = handleLoopFormData(formData);
        const input = data.json as iClient;

        if (input.cpf) {
            input.cpf = handleGetOnlyNumbers(input.cpf);
        }

        if (input.phone) {
            input.phone = handleGetOnlyNumbers(input.phone);
        }

        if (!companyId) {
            swal({ content: 'Erro interno: O ID da empresa está vazio. Tente novamnete, e se o erro persistir, contate o suporte.', icon: 'error' });
            return;
        }

        input.companyId = companyId;

        // console.log(input);

        if (type === 'create') {
            const output = await Fetch.post({ url: CONSTS_CLIENT.post, body: input }) as iClient;

            if (output) {
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
        const output = await Fetch.put({ url: CONSTS_CLIENT.put, body: input }) as iClient;

        if (output) {
            swal({
                content: 'Cliente atualizado com sucesso.',
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

    async function handleGetCEP(e: KeyboardEvent<HTMLInputElement>) {
        setFormData(prev => ({ ...prev, address: '', addressNumber: '', city: '', state: '', country: '' }));

        const cep = e.currentTarget.value;
        const cepRegex = /^\d{5}-\d{3}$/;

        if (!cepRegex.test(cep)) {
            return;
        }

        const data = await handleFetchCEP(cep);

        if (data) {
            setFormData(prev => ({ ...prev, ...data }));
        }
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
                        <InputMask title='Nome' fieldName='fullName' formData={formData} setFormData={setFormData} isDisabled={!editing} isObligatory={true} />
                        <InputMask title='CPF' fieldName='cpf' formData={formData} setFormData={setFormData} isDisabled={!editing} mask='000000000-00' isObligatory={true} />
                        <InputMask title='E-mail' fieldName='email' formData={formData} setFormData={setFormData} isDisabled={!editing} />
                        <InputMask title='Telefone' fieldName='phone' formData={formData} setFormData={setFormData} isDisabled={!editing} mask='(00) 00000-0000' />
                        <InputMask title='CEP' fieldName='zipCode' formData={formData} setFormData={setFormData} isDisabled={!editing} mask='00000-000' handleOnChange={(e) => handleGetCEP(e)} />
                        <InputMask title='Rua' fieldName='address' formData={formData} setFormData={setFormData} isDisabled={!editing} />
                        <InputMask title='Número do endereço' fieldName='addressNumber' formData={formData} setFormData={setFormData} isDisabled={!editing} />
                        <InputMask title='Cidade' fieldName='city' formData={formData} setFormData={setFormData} isDisabled={!editing} />
                        <InputMask title='Estado' fieldName='state' formData={formData} setFormData={setFormData} isDisabled={!editing} />
                        <Dropdown title='País' options={(countries ?? []).map(country => ({ value: country, label: country }))} selectedOption={(countries ?? []).map(country => ({ value: country, label: country })).find(x => x.value === formData.country)} setSelectedOption={setCountryOption} isDisabled={!editing} />
                        <InputMask type='date' title='Data de aniversário' fieldName='dateOfBirth' formData={formData} setFormData={setFormData} isDisabled={!editing} />
                        <InputMask title='Anotações' fieldName='notes' formData={formData} setFormData={setFormData} isDisabled={!editing} />
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
                                            <Button label='Habilitar edição' handleFunction={() => setEditing(true)} />
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