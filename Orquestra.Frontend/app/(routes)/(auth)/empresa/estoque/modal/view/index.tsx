'use client';
import { CONSTS_INVENTORY, iInventory } from '@/app/api/consts/inventory';
import { CONSTS_UTILITY } from '@/app/api/consts/utility';
import { Fetch } from '@/app/api/fetch';
import ContentLoaderText from '@/app/components/content-loader/text';
import Button from '@/app/components/input/button';
import InputImage from '@/app/components/input/image';
import InputMask from '@/app/components/input/text';
import ModalGeneric from '@/app/components/modal/generic';
import styles from '@/app/components/modal/generic/index.module.scss';
import Tags from '@/app/components/tags';
import SYSTEM from '@/app/consts/system';
import { handleClearFormData, handleLoopFormData } from '@/app/functions/set.formState';
import swal from '@/app/functions/swal';
import useApiRequestToSetterOnUrlChange from '@/app/hooks/api/useApiRequestToSetterOnUrlChange';
import { Guid } from 'guid-typescript';
import { Dispatch, Fragment, SetStateAction, useCallback, useEffect, useState } from 'react';

interface iProps {
    isModalOpen: boolean;
    setIsModalOpen: Dispatch<SetStateAction<boolean>>;
    type: 'edit' | 'create';
    item: iInventory | undefined;
    companyId: Guid | undefined;
    setTrigger: Dispatch<SetStateAction<Date>>;
}

export default function EmpresaEstoqueModalView({ isModalOpen, setIsModalOpen, type, item, companyId, setTrigger }: iProps) {

    const [editing, setEditing] = useState<boolean>(false);
    const [saving, setSaving] = useState<boolean>(false);

    const [formData, setFormData] = useState<iInventory>({
        inventoryId: SYSTEM.EMPTY_GUID,
        companyId: SYSTEM.EMPTY_GUID,
        name: null,
        description: '',
        quantity: 0,
        unitPrice: 0,
        imageFormFile: null,
        imageBase64: ''
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

        if (!isModalOpen || !item) {
            return;
        }

        setFormData({
            inventoryId: item ? item.inventoryId : Guid.create(),
            companyId: item ? item.companyId : Guid.create(),
            name: item ? item.name : null,
            description: item && item.description ? item.description : null,
            quantity: item && item.quantity ? item.quantity : 0,
            unitPrice: item && item.unitPrice ? item.unitPrice : 0,
            imageFormFile: item && item.imageFormFile ? item.imageFormFile : null,
            imageBase64: item && item.imageBase64 ? item.imageBase64 : ''
        });
    }, [isModalOpen, type, item, setIsModalOpen, handleClose]);

    async function handleSave() {
        if (!formData.name || !formData.quantity || !formData.unitPrice) {
            swal({ content: SYSTEM.WARN_FILL_OBLIGATORY_FIELDS, icon: 'warning' });
            return;
        }

        setEditing(false);
        setSaving(true);

        const data = handleLoopFormData(formData);
        const input = data.json as iInventory;
        const formDataInput = new FormData();

        formDataInput.append('InventoryId', input.inventoryId ? input.inventoryId.toString() : Guid.create().toString());
        formDataInput.append('CompanyId', input.companyId?.toString()!);
        formDataInput.append('Name', input.name ?? '');
        formDataInput.append('Description', input.description ?? '');
        formDataInput.append('Quantity', input?.quantity && input?.quantity > 0 ? input.quantity?.toString() : '0');
        formDataInput.append('UnitPrice', input?.unitPrice && input?.unitPrice > 0 ? input.unitPrice?.toString() : '0');

        if (input.imageFormFile && input.imageFormFile instanceof File) {
            formDataInput.append('ImageFormFile', input.imageFormFile as Blob, input.imageFormFile.name);
        }

        if (!companyId) {
            swal({ content: 'Erro interno: O ID da empresa está vazio. Tente novamnete, e se o erro persistir, contate o suporte.', icon: 'error' });
            return;
        }

        input.companyId = companyId;

        // console.log(input);

        if (type === 'create') {
            const output = await Fetch.post({ url: CONSTS_INVENTORY.post, body: formDataInput, isFormData: true });

            if (output) {
                swal({
                    content: 'Item registrado com sucesso.',
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

        const output = await Fetch.put({ url: CONSTS_INVENTORY.put, body: formDataInput, isFormData: true });

        if (output) {
            swal({
                content: 'Item atualizado com sucesso.',
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
                            {type === 'create' ? (formData.name ? `Novo item: ${formData.name}` : 'Cadastrar novo item') : <ContentLoaderText content={(`Editar item: ${formData?.name ?? item?.name}`)} />}
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
                        <InputMask title='Nome' fieldName='name' formData={formData} setFormData={setFormData} isDisabled={!editing} isObligatory={true} />
                        <InputMask title='Descrição' fieldName='description' formData={formData} setFormData={setFormData} isDisabled={!editing} />
                        <InputMask title='Quantidade' fieldName='quantity' formData={formData} setFormData={setFormData} isDisabled={!editing} isObligatory={true} />
                        <InputMask title='Preço por unidade' fieldName='unitPrice' formData={formData} setFormData={setFormData} isDisabled={!editing} isObligatory={true} />
                        <InputImage title='Imagem' fieldName='imageFormFile' formData={formData} setFormData={setFormData} isDisabled={!editing} placeholder='Selecionar imagem' />
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