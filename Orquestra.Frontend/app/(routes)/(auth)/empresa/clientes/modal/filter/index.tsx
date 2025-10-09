'use client';
import Button from '@/app/components/input/button';
import InputMask from '@/app/components/input/text';
import ModalGeneric from '@/app/components/modal/generic';
import handleNormalizeFetchUrl from '@/app/functions/normalize.fetch-url';
import { handleClearFormData, handleLoopFormData } from '@/app/functions/set.formState';
import { Dispatch, SetStateAction } from 'react';

interface iModalFilterParams {
    isModalFilterOpen: boolean;
    setIsModalFilterOpen: Dispatch<SetStateAction<boolean>>;
    modalFilterFormData: iClientFormModalFilterData;
    setModalFilterFormData: Dispatch<SetStateAction<iClientFormModalFilterData>>;
    apiUrlRequest: string;
    setApiUrlRequest: Dispatch<SetStateAction<string>>;
    setCurrentPage: Dispatch<SetStateAction<number>>;
}

export interface iClientFormModalFilterData {
    fullName: string | null;
    email: string | null;
    CPF: string | null;
    address: string | null;
    dateOfBirth: string | null;
    phone: string | null;
    notes: string | null;
}

export default function EmpresaClientesModalFilters({
    isModalFilterOpen,
    setIsModalFilterOpen,
    modalFilterFormData,
    setModalFilterFormData,
    apiUrlRequest,
    setApiUrlRequest,
    setCurrentPage
}: iModalFilterParams) {

    function handleSubmit() {
        const data = handleLoopFormData(modalFilterFormData, 'label');
        const url = handleNormalizeFetchUrl(apiUrlRequest, data);

        setApiUrlRequest(url);
        setCurrentPage(1);
        setIsModalFilterOpen(false);
    }

    return (
        <ModalGeneric
            isOpen={isModalFilterOpen}
            setModalIsOpen={setIsModalFilterOpen}
            onRequestClose={() => setIsModalFilterOpen(false)}
            showCloseButton={true}
            title='Filtre seus clientes'
            overlayColor={0.5}
            allowCloseOutsideClick={false}
        >
            <div className='modal-layout-grid'>
                <InputMask title='Nome' fieldName='fullName' formData={modalFilterFormData} setFormData={setModalFilterFormData} />
                <InputMask title='E-mail' fieldName='email' formData={modalFilterFormData} setFormData={setModalFilterFormData} />
                <InputMask title='CPF' fieldName='CPF' formData={modalFilterFormData} setFormData={setModalFilterFormData} />
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