'use client';
import EmpresaQuotesItemsEditor from '@/app/(routes)/(auth)/empresa/orcamento/quote-item';
import { iClient } from '@/app/api/consts/client';
import { CONSTS_QUOTE, iQuote } from '@/app/api/consts/quote';
import { Fetch } from '@/app/api/fetch';
import ContentLoaderText from '@/app/components/content-loader/text';
import Button from '@/app/components/input/button';
import Dropdown, { iDropdownOption } from '@/app/components/input/drop-down';
import InputMask from '@/app/components/input/text';
import ModalGeneric from '@/app/components/modal/generic';
import styles from '@/app/components/modal/generic/index.module.scss';
import Tags from '@/app/components/tags';
import SYSTEM from '@/app/consts/system';
import { handleFormatDateToInputValue } from '@/app/functions/format.date';
import handleGetPropName from '@/app/functions/get.propName';
import { handleNormalizeGuidField } from '@/app/functions/normalize.guid';
import { handleClearFormData, handleLoopFormData, handleSetDropdownOption } from '@/app/functions/set.formState';
import swal from '@/app/functions/swal';
import { handleTransformArrayToDropdownOptionsGuid } from '@/app/functions/transform.arrayToDropdownOptions';
import useApiGetEnum from '@/app/hooks/api/useApiGetEnum';
import { useIsModalGrid } from '@/app/hooks/contexts/useGlobalContext';
import { Guid } from 'guid-typescript';
import { Dispatch, Fragment, SetStateAction, useCallback, useEffect, useState } from 'react';

interface iProps {
    isModalOpen: boolean;
    setIsModalOpen: Dispatch<SetStateAction<boolean>>;
    type: 'edit' | 'create';
    clients: iClient[];
    quote: iQuote | undefined;
    companyId: Guid | undefined;
    setTrigger: Dispatch<SetStateAction<Date>>;
}

export default function EmpresaQuotesModalView({ isModalOpen, setIsModalOpen, type, clients, quote, companyId, setTrigger }: iProps) {

    const [isModalGrid,] = useIsModalGrid();
    const [clientsDropDown, setClientsDropDown] = useState<iDropdownOption[]>();

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

        const optionsClients = handleTransformArrayToDropdownOptionsGuid(clients ?? [], 'clientId', ['fullName', 'phone', 'email']);
        setClientsDropDown(optionsClients);

        if (type === 'create') {
            setFormData(prev => ({
                ...prev,
                companyId: companyId
            }));

            return;
        }

        if (!isModalOpen || !quote) {
            return;
        }

        setFormData({
            quoteId: quote ? quote.quoteId : SYSTEM.EMPTY_GUID,
            companyId: companyId,
            clientId: quote ? quote.clientId : SYSTEM.EMPTY_GUID,
            title: quote && quote.title ? quote.title : '',
            observation: quote && quote.observation ? quote.observation : null,
            validUntil: quote && quote.validUntil ? handleFormatDateToInputValue(new Date(quote?.validUntil)) : SYSTEM.EMPTY_DATE,
            quoteStatus: quote ? quote.quoteStatus : '',
            items: quote && quote.items ? quote.items : []
        });
    }, [isModalOpen, type, clients, quote, companyId, setIsModalOpen, handleClose]);

    const quoteStatusEnum = useApiGetEnum({ enumName: 'QuoteStatusEnum' });

    const setClientIdOption = handleSetDropdownOption(formData, setFormData, handleGetPropName(formData, x => x.clientId ?? '')[1]) as Dispatch<SetStateAction<iDropdownOption[]>>;
    const setQuoteStatusOption = handleSetDropdownOption(formData, setFormData, handleGetPropName(formData, x => x.quoteStatus ?? '')[1]) as Dispatch<SetStateAction<iDropdownOption[]>>;

    async function handleSave() {
        if (!formData.companyId || !formData.clientId || !formData.title || !formData.quoteStatus || !formData.items || !formData.items?.length) {
            swal({ content: SYSTEM.WARN_FILL_OBLIGATORY_FIELDS, icon: 'warning' });
            return;
        }

        setEditing(false);
        setSaving(true);

        const data = handleLoopFormData(formData);
        const input = data.json as iQuote;

        // #region normalizar_props
        input.clientId = handleNormalizeGuidField(input.clientId ?? SYSTEM.EMPTY_GUID);

        input.items?.forEach(element => {
            if (element.quoteItemId) {
                element.quoteItemId = handleNormalizeGuidField(element.quoteItemId)
            }
        });

        // console.log(input);
        // #endregion

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
                            {type === 'create' ? (formData.title ? `Registrar novo orçamento: ${formData.title}` : 'Registrar novo orçamento') : <ContentLoaderText content={(`Editar orçamento: ${formData?.title}`)} />}
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
                    <div className={`${isModalGrid ? styles.grid : 'modal-layout-flex'}`}>
                        <Dropdown title='Cliente' options={clientsDropDown ?? []} selectedOption={clientsDropDown?.find(x => x.value.toString() === formData.clientId?.toString())} setSelectedOption={setClientIdOption} isDisabled={!editing} isObligatory={true} />
                        <InputMask title='Título' fieldName='title' formData={formData} setFormData={setFormData} isDisabled={!editing} isObligatory={true} />
                        <InputMask type='date' title='Válido até' fieldName='validUntil' formData={formData} setFormData={setFormData} isDisabled={!editing} />
                        <Dropdown title='Status do orçamento' options={quoteStatusEnum ?? []} selectedOption={quoteStatusEnum?.find(x => x.value === formData.quoteStatus) ?? undefined} setSelectedOption={setQuoteStatusOption} isDisabled={!editing} isObligatory={true} />

                        <div className={`${styles.div} ${styles.full}`}>
                            <label>Observações</label>
                            <textarea value={formData.observation ?? ''} className={styles.textarea} readOnly={!editing} rows={5} maxLength={512} onChange={(e) => setFormData((prev: typeof formData) => ({ ...prev, observation: e.target.value }))} />
                        </div>

                        <div className={`${styles.div} ${styles.full}`}>
                            <EmpresaQuotesItemsEditor
                                formData={formData}
                                setFormData={setFormData}
                                editing={editing}
                            />
                        </div>
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