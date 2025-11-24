'use client';
import { CONSTS_QUOTE, iQuote } from '@/app/api/consts/quote';
import { Fetch } from '@/app/api/fetch';
import ContentLoaderText from '@/app/components/content-loader/text';
import Button from '@/app/components/input/button';
import Dropdown from '@/app/components/input/drop-down';
import InputMask from '@/app/components/input/text';
import ModalGeneric from '@/app/components/modal/generic';
import styles from '@/app/components/modal/generic/index.module.scss';
import Tags from '@/app/components/tags';
import SYSTEM from '@/app/consts/system';
import { handleFormatDateToInputValue } from '@/app/functions/format.date';
import { handleGetOnlyNumbers } from '@/app/functions/format.string';
import { handleClearFormData, handleLoopFormData } from '@/app/functions/set.formState';
import swal from '@/app/functions/swal';
import { Guid } from 'guid-typescript';
import { Dispatch, Fragment, SetStateAction, useCallback, useEffect, useState } from 'react';

interface iProps {
    isModalOpen: boolean;
    setIsModalOpen: Dispatch<SetStateAction<boolean>>;
    type: 'edit' | 'create';
    quote: iQuote | undefined;
    companyId: Guid | undefined;
    setTrigger: Dispatch<SetStateAction<Date>>;
}

export default function EmpresaQuotesModalView({ isModalOpen, setIsModalOpen, type, quote, companyId, setTrigger }: iProps) {

    const [editing, setEditing] = useState<boolean>(false);
    const [saving, setSaving] = useState<boolean>(false);

    const [formData, setFormData] = useState<iQuote>({
        quoteId: SYSTEM.EMPTY_GUID,
        companyId: SYSTEM.EMPTY_GUID,
        clientId: SYSTEM.EMPTY_GUID,
        title: '',
        observation: null,
        validUntil: null,
        quoteStatus: '',
        items: []
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
        setEditing(true);

        if (type === 'create') {
            return;
        }

        if (!isModalOpen || !quote) {
            return;
        }

        setFormData({
            quoteId: quote ? quote.quoteId : SYSTEM.EMPTY_GUID,
            companyId: quote ? quote.companyId : SYSTEM.EMPTY_GUID,
            clientId: quote ? quote.clientId : SYSTEM.EMPTY_GUID,
            title: quote && quote.title ? quote.title : '',
            observation: quote && quote.observation ? quote.observation : null,
            validUntil: quote && quote.validUntil ? handleFormatDateToInputValue(new Date(quote.validUntil)) : SYSTEM.EMPTY_DATE,
            quoteStatus: quote ? quote.quoteStatus : '',
            items: quote && quote.items ? quote.items : []
        });
    }, [isModalOpen, type, quote, setIsModalOpen, handleClose]);

    async function handleSave() {
        if (!formData.fullName || !formData.cpf) {
            swal({ content: SYSTEM.WARN_FILL_OBLIGATORY_FIELDS, icon: 'warning' });
            return;
        }

        setEditing(false);
        setSaving(true);

        const data = handleLoopFormData(formData);
        const input = data.json as iQuote;

        if (input.cpf) {
            input.cpf = handleGetOnlyNumbers(input.cpf);
        }

        if (input.phone) {
            input.phone = handleGetOnlyNumbers(input.phone);
        }

        if (!companyId) {
            swal({ content: 'Erro interno: O ID da empresa está vazio. Tente novamente, e se o erro persistir, contate o suporte.', icon: 'error' });
            return;
        }

        input.companyId = companyId;

        // console.log(input);

        if (type === 'create') {
            const output = await Fetch.post({ url: CONSTS_QUOTE.post, body: input }) as iQuote;

            if (output) {
                swal({
                    content: 'Orçamento registrado com sucesso.',
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

        const output = await Fetch.put({ url: CONSTS_QUOTE.put, body: input }) as iQuote;

        if (output) {
            swal({
                content: 'Orçamento atualizado com sucesso.',
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
                            {type === 'create' ? (formData.title ? `Novo orçamento: ${formData.title}` : 'Registrar novo orçamento') : <ContentLoaderText content={(`Editar orçamento: ${formData?.title}`)} />}
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