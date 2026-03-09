'use client';
import { CONSTS_SERVICE_ORDER, iServiceOrder } from '@/app/api/consts/service-order';
import { Fetch } from '@/app/api/fetch';
import ContentLoaderText from '@/app/components/content-loader/text';
import Dropdown, { iDropdownOption } from '@/app/components/input/drop-down';
import DropDownCliente from '@/app/components/input/drop-down-custom/cliente';
import InputMask from '@/app/components/input/text';
import ModalGeneric from '@/app/components/modal/generic';
import ModalGenericFooter from '@/app/components/modal/generic/footer/footer';
import styles from '@/app/components/modal/generic/index.module.scss';
import TagList from '@/app/components/tags/tag-list';
import SYSTEM from '@/app/consts/system';
import { handleFormatDateToInputValue } from '@/app/functions/format.date';
import handleGetPropName from '@/app/functions/get.propName';
import { handleNormalizeGuidField } from '@/app/functions/normalize.guid';
import { handleClearFormData, handleLoopFormData, handleSetDropdownOption } from '@/app/functions/set.formState';
import swal from '@/app/functions/swal';
import useApiGetEnum from '@/app/hooks/api/useApiGetEnum';
import { useIsModalGrid } from '@/app/hooks/contexts/useGlobalContext';
import { Guid } from 'guid-typescript';
import { Dispatch, SetStateAction, useCallback, useEffect, useState } from 'react';

interface iProps {
    isModalOpen: boolean;
    setIsModalOpen: Dispatch<SetStateAction<boolean>>;
    type: 'edit' | 'create';
    clientsDropDown: iDropdownOption<string | number | Guid>[] | undefined;
    serviceOrder: iServiceOrder | undefined;
    companyId: Guid | undefined;
    setTrigger: Dispatch<SetStateAction<Date>>;
}

export default function EmpresaServiceOrderModalView({ isModalOpen, setIsModalOpen, type, clientsDropDown, serviceOrder, companyId, setTrigger }: iProps) {

    const [isModalGrid,] = useIsModalGrid();

    const [editing, setEditing] = useState<boolean>(false);
    const [saving, setSaving] = useState<boolean>(false);

    const [formData, setFormData] = useState<iServiceOrder>({
        serviceOrderId: SYSTEM.EMPTY_GUID,
        companyId: SYSTEM.EMPTY_GUID,
        company: null,
        quoteId: null,
        quote: null,
        clientId: SYSTEM.EMPTY_GUID,
        client: null,
        title: '',
        observation: null,
        executionDate: null,
        serviceOrderStatus: null,
        createdDate: null
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
            setFormData(prev => ({
                ...prev,
                companyId: companyId
            }));

            return;
        }

        if (!isModalOpen || !serviceOrder) {
            return;
        }

        setFormData({
            serviceOrderId: serviceOrder?.serviceOrderId ?? SYSTEM.EMPTY_GUID,
            companyId: companyId,
            company: serviceOrder?.company ?? null,
            quoteId: serviceOrder?.quoteId ?? null,
            quote: serviceOrder?.quote ?? null,
            clientId: serviceOrder?.clientId ?? SYSTEM.EMPTY_GUID,
            client: serviceOrder?.client ?? null,
            title: serviceOrder?.title ?? '',
            observation: serviceOrder?.observation ?? null,
            executionDate: serviceOrder?.executionDate ? handleFormatDateToInputValue(new Date(serviceOrder.executionDate)) : null,
            serviceOrderStatus: serviceOrder?.serviceOrderStatus ?? null,
            createdDate: serviceOrder?.createdDate ?? null
        });
    }, [isModalOpen, type, serviceOrder, companyId, setIsModalOpen, handleClose]);

    const serviceOrderStatusEnum = useApiGetEnum({ enumName: 'ServiceOrderStatusEnum' });

    const setClientIdOption = handleSetDropdownOption(formData, setFormData, handleGetPropName(formData, x => x.clientId ?? '')[1]) as Dispatch<SetStateAction<iDropdownOption[]>>;
    const setServiceOrderStatusOption = handleSetDropdownOption(formData, setFormData, handleGetPropName(formData, x => x.serviceOrderStatus ?? '')[1]) as Dispatch<SetStateAction<iDropdownOption[]>>;

    async function handleSave() {
        if (!formData.companyId || !formData.clientId || !formData.title || !formData.serviceOrderStatus) {
            swal({ content: SYSTEM.WARN_FILL_OBLIGATORY_FIELDS, icon: 'warning' });
            return;
        }

        setEditing(false);
        setSaving(true);

        const data = handleLoopFormData({ formData });
        const input = data.json as iServiceOrder;

        // #region normalizar_props
        input.clientId = handleNormalizeGuidField(input.clientId ?? SYSTEM.EMPTY_GUID);

        // console.log(input);
        // #endregion

        if (type === 'create') {
            const output = await Fetch.post({ url: CONSTS_SERVICE_ORDER.post, body: input }) as iServiceOrder;

            if (output) {
                swal({
                    content: 'Ordem de serviço registrada com sucesso.',
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

        const output = await Fetch.put({ url: CONSTS_SERVICE_ORDER.put, body: input }) as iServiceOrder;

        if (output) {
            swal({
                content: 'Ordem de serviço atualizada com sucesso.',
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
                            {type === 'create' ? (formData.title ? `Registrar nova ordem de serviço: ${formData.title}` : 'Registrar nova ordem de serviço') : <ContentLoaderText content={(`Editar ordem de serviço: ${formData?.title}`)} />}
                        </h1>
                    </div>

                    <div className={styles.modalHeaderRight}>
                        <div className={styles.metaRow}>
                            <TagList
                                tags={[
                                    { label: '✖', color: 'transparent', handleFunction: () => handleClose(), title: 'Fechar' }
                                ]}
                            />
                        </div>
                    </div>
                </header>

                <main className={styles.modalContent}>
                    <div className={`${isModalGrid ? styles.grid : 'modal-layout-flex'}`}>
                        <DropDownCliente editing={editing} clientsDropDown={clientsDropDown} setClientIdOption={setClientIdOption} clientId={formData.clientId} isObligatory={true} showNewClientButton={true} />
                        <InputMask title='Título' fieldName='title' formData={formData} setFormData={setFormData} isDisabled={!editing} isObligatory={true} />
                        <InputMask type='date' title='Data de execução' fieldName='executionDate' formData={formData} setFormData={setFormData} isDisabled={!editing} />
                        <Dropdown title='Status da ordem de serviço' options={serviceOrderStatusEnum ?? []} selectedOption={serviceOrderStatusEnum?.find(x => x.value === formData.serviceOrderStatus) ?? undefined} setSelectedOption={setServiceOrderStatusOption} isDisabled={!editing} isObligatory={true} />

                        <div className={`${styles.div} ${styles.full}`}>
                            <label>Observações</label>
                            <textarea value={formData.observation ?? ''} className={styles.textarea} readOnly={!editing} rows={5} maxLength={512} onChange={(e) => setFormData((prev: typeof formData) => ({ ...prev, observation: e.target.value }))} />
                        </div>
                    </div>
                </main>

                <ModalGenericFooter
                    type={type}
                    saving={saving}
                    editing={editing}
                    handleClose={handleClose}
                    handleSave={handleSave}
                    setEditing={setEditing}
                />
            </div>
        </ModalGeneric>
    )
}