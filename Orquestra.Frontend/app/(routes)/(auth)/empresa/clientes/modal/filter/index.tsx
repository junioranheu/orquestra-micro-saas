'use client';
import Button from '@/app/components/input/button';
import InputMask from '@/app/components/input/text';
import ModalGeneric from '@/app/components/modal/generic';
import { handleNormalizeFetchUrl, handleRemoveDuplicateQueryParams } from '@/app/functions/normalize.fetch-url';
import { handleClearFormData, handleLoopFormData } from '@/app/functions/set.formState';
import { Dispatch, SetStateAction } from 'react';

interface iProps {
    isModalOpen: boolean;
    setIsModalOpen: Dispatch<SetStateAction<boolean>>;
    modalFilterFormData: iClientFormDataModalFilter;
    setModalFilterFormData: Dispatch<SetStateAction<iClientFormDataModalFilter>>;
    apiUrlRequest: string;
    setApiUrlRequest: Dispatch<SetStateAction<string>>;
    setCurrentPage: Dispatch<SetStateAction<number>>;
}

export interface iClientFormDataModalFilter {
    fullName: string | null;
    email: string | null;
    cpf: string | null;
    address: string | null;
    addressNumber: string | null;
    city: string | null;
    state: string | null;
    zipCode: string | null;
    country: string | null;
    dateOfBirth: Date | string | null;
    phone: string | null;
    notes: string | null;
}

export default function EmpresaClientesModalFilters({
    isModalOpen,
    setIsModalOpen,
    modalFilterFormData,
    setModalFilterFormData,
    apiUrlRequest,
    setApiUrlRequest,
    setCurrentPage
}: iProps) {

    function handleSubmit() {
        const data = handleLoopFormData(modalFilterFormData, 'label');
        const url = handleNormalizeFetchUrl(apiUrlRequest, data);
        const urlNormalized = handleRemoveDuplicateQueryParams(url);

        setApiUrlRequest(urlNormalized);
        setCurrentPage(1);
        setIsModalOpen(false);
    }

    return (
        <ModalGeneric
            isOpen={isModalOpen}
            setIsModalOpen={setIsModalOpen}
            onRequestClose={() => setIsModalOpen(false)}
            showCloseButton={true}
            title='Filtre seus clientes'
            overlayColor={0.5}
            allowCloseOutsideClick={false}
            style={{ width: '50rem' }}
        >
            <div className='modal-layout-grid'>
                <InputMask title='Nome' fieldName='fullName' formData={modalFilterFormData} setFormData={setModalFilterFormData} />
                <InputMask title='CPF' fieldName='cpf' formData={modalFilterFormData} setFormData={setModalFilterFormData} />
                <InputMask title='E-mail' fieldName='email' formData={modalFilterFormData} setFormData={setModalFilterFormData} />
                <InputMask title='Telefone' fieldName='phone' formData={modalFilterFormData} setFormData={setModalFilterFormData} />
                <InputMask title='Endereço' fieldName='address' formData={modalFilterFormData} setFormData={setModalFilterFormData} />
                <InputMask type='date' title='Data de aniversário' fieldName='dateOfBirth' formData={modalFilterFormData} setFormData={setModalFilterFormData} />
                <InputMask title='Anotações' fieldName='notes' formData={modalFilterFormData} setFormData={setModalFilterFormData} />
                <div />

                <Button
                    label='Limpar filtros'
                    handleFunction={() => handleClearFormData(setModalFilterFormData)}
                    isStyleSimple={true}
                    style={{ fontSize: '0.75rem' }}
                />

                <Button
                    label='Filtrar'
                    handleFunction={() => handleSubmit()}
                    style={{ fontSize: '0.75rem' }}
                />
            </div>
        </ModalGeneric>
    )
}