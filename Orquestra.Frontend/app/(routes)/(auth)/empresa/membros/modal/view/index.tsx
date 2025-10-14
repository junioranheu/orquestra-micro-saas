'use client';
import { iCompanyUserFormDataModalFilter } from '@/app/(routes)/(auth)/empresa/membros/modal/filter';
import { iUser } from '@/app/api/consts/user';
import ContentLoaderText from '@/app/components/content-loader/text';
import Button from '@/app/components/input/button';
import ModalGeneric from '@/app/components/modal/generic';
import styles from '@/app/components/modal/generic/index.module.scss';
import Tags from '@/app/components/tags';
import { handleClearFormData } from '@/app/functions/set.formState';
import { Guid } from 'guid-typescript';
import { Dispatch, Fragment, SetStateAction, useCallback, useEffect, useState } from 'react';

interface iModalFilterParams {
    isModalOpen: boolean;
    setIsModalOpen: Dispatch<SetStateAction<boolean>>;
    type: 'edit' | 'create';
    member: iUser | undefined;
    companyId: Guid | undefined;
}

export default function EmpresaMembrosModalView({ isModalOpen, setIsModalOpen, type, member, companyId }: iModalFilterParams) {

    const [editing, setEditing] = useState<boolean>(false);
    const [saving, setSaving] = useState<boolean>(false);

    const [formData, setFormData] = useState<iCompanyUserFormDataModalFilter>({
        companyUserRole: null,
        modules: [],
        fullName: null,
        email: null,
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

        if (!isModalOpen || !member) {
            return;
        }

        // setFormData({
        //     fullName: client ? client.fullName : null,
        //     email: client ? client.email : null,
        //     cpf: client ? handleFormatCPF(client.cpf) : null,
        //     address: client && client.address ? client.address : null,
        //     addressNumber: client && client.addressNumber ? client.addressNumber : null,
        //     city: client && client.city ? client.city : null,
        //     state: client && client.state ? client.state : null,
        //     zipCode: client && client.zipCode ? client.zipCode : null,
        //     country: client && client.country ? client.country : null,
        //     dateOfBirth: client && client?.dateOfBirth ? handleFormatDateToInputValue(new Date(client?.dateOfBirth)) : null,
        //     notes: client && client?.notes ? client.notes : null,
        //     phone: client && client?.phone ? client.phone : null
        // });
    }, [isModalOpen, type, member, setIsModalOpen, handleClose]);

    // const setCountryOption = handleSetDropdownOption(formData, setFormData, handleGetPropName(formData, x => x.country ?? '')[1]) as Dispatch<SetStateAction<iDropdownOption[]>>;

    async function handleSave() {
        // if (!formData.fullName || !formData.cpf) {
        //     swal({ content: SYSTEM.WARN_FILL_OBLIGATORY_FIELDS, icon: 'warning' });
        //     return;
        // }

        // setEditing(false);
        // setSaving(true);

        // const data = handleLoopFormData(formData);
        // const input = data.json as iClient;

        // if (input.cpf) {
        //     input.cpf = handleGetOnlyNumbers(input.cpf);
        // }

        // if (input.phone) {
        //     input.phone = handleGetOnlyNumbers(input.phone);
        // }

        // if (!companyId) {
        //     swal({ content: 'Erro interno: O ID da empresa está vazio. Tente novamnete, e se o erro persistir, contate o suporte.', icon: 'error' });
        //     return;
        // }

        // input.companyId = companyId;

        // // console.log(input);

        // if (type === 'create') {
        //     const output = await Fetch.post({ url: CONSTS_CLIENT.post, body: input }) as iClient;

        //     if (output) {
        //         swal({
        //             content: 'Cliente registrado com sucesso.',
        //             confirmFunction: () => window.location.reload(),
        //             icon: 'success'
        //         });

        //         handleClose();
        //         return;
        //     }

        //     setEditing(true);
        //     setSaving(false);
        //     return;
        // }

        // if (!client?.clientId) {
        //     swal({ content: 'Erro interno: O ID do cliente está vazio. Tente novamnete, e se o erro persistir, contate o suporte.', icon: 'error' });
        //     return;
        // }

        // input.clientId = client.clientId;
        // const output = await Fetch.put({ url: CONSTS_CLIENT.put, body: input }) as iClient;

        // if (output) {
        //     swal({
        //         content: 'Cliente atualizado com sucesso.',
        //         confirmFunction: () => window.location.reload(),
        //         icon: 'success'
        //     });

        //     handleClose();
        //     return;
        // }

        // setEditing(true);
        // setSaving(false);
        // return;
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
                            {type === 'create' ? (formData.fullName ? `Novo cliente: ${formData.fullName}` : 'Cadastrar novo cliente') : <ContentLoaderText text={(`Editar cliente: ${formData?.fullName ?? member?.fullName}`)} />}
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
                        {/* <InputMask title='Nome' fieldName='fullName' formData={formData} setFormData={setFormData} isDisabled={!editing} isObligatory={true} />
                        <InputMask title='CPF' fieldName='cpf' formData={formData} setFormData={setFormData} isDisabled={!editing} mask='00000000-00' isObligatory={true} />
                        <InputMask title='E-mail' fieldName='email' formData={formData} setFormData={setFormData} isDisabled={!editing} />
                        <InputMask title='Telefone' fieldName='phone' formData={formData} setFormData={setFormData} isDisabled={!editing} mask='(00) 00000-0000' />
                        <InputMask title='CEP' fieldName='zipCode' formData={formData} setFormData={setFormData} isDisabled={!editing} mask='00000-000' handleOnChange={(e) => handleGetCEP(e)} />
                        <InputMask title='Rua' fieldName='address' formData={formData} setFormData={setFormData} isDisabled={!editing} />
                        <InputMask title='Número do endereço' fieldName='addressNumber' formData={formData} setFormData={setFormData} isDisabled={!editing} />
                        <InputMask title='Cidade' fieldName='city' formData={formData} setFormData={setFormData} isDisabled={!editing} />
                        <InputMask title='Estado' fieldName='state' formData={formData} setFormData={setFormData} isDisabled={!editing} />
                        <Dropdown title='País' options={(countries ?? []).map(country => ({ value: country, label: country }))} selectedOption={(countries ?? []).map(country => ({ value: country, label: country })).find(x => x.value === formData.country)} setSelectedOption={setCountryOption} isDisabled={!editing} />
                        <InputMask type='date' title='Data de aniversário' fieldName='dateOfBirth' formData={formData} setFormData={setFormData} isDisabled={!editing} />
                        <InputMask title='Anotações' fieldName='notes' formData={formData} setFormData={setFormData} isDisabled={!editing} /> */}
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