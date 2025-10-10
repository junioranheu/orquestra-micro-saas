'use client';
import { iClientFormDataModalFilter } from '@/app/(routes)/(auth)/empresa/clientes/modal/filter';
import Button from '@/app/components/input/button';
import InputMask from '@/app/components/input/text';
import ModalGeneric from '@/app/components/modal/generic';
import { Dispatch, SetStateAction, useState } from 'react';

interface iModalFilterParams {
    isModalOpen: boolean;
    setIsModalOpen: Dispatch<SetStateAction<boolean>>;
}

export default function EmpresaClientesModalAdd({ isModalOpen, setIsModalOpen }: iModalFilterParams) {

    const [modalFilterFormData, setModalFilterFormData] = useState<iClientFormDataModalFilter>({
        fullName: null, email: null, CPF: null, address: null, dateOfBirth: null, notes: null, phone: null
    });

    function handleSubmit() {
        // const data = handleLoopFormData(modalFilterFormData, 'label');

        setIsModalOpen(false);
    }

    return (
        <ModalGeneric
            isOpen={isModalOpen}
            setModalIsOpen={setIsModalOpen}
            onRequestClose={() => setIsModalOpen(false)}
            showCloseButton={true}
            title='Cadastre um novo cliente'
            overlayColor={0.5}
            allowCloseOutsideClick={false}
            style={{ width: '50rem' }}
        >
            <div className='modal-layout-grid'>
                <InputMask title='Nome' fieldName='fullName' formData={modalFilterFormData} setFormData={setModalFilterFormData} />
                <InputMask title='CPF' fieldName='CPF' formData={modalFilterFormData} setFormData={setModalFilterFormData} />
                <InputMask title='E-mail' fieldName='email' formData={modalFilterFormData} setFormData={setModalFilterFormData} />
                <InputMask title='Telefone' fieldName='phone' formData={modalFilterFormData} setFormData={setModalFilterFormData} />
                <InputMask title='Endereço' fieldName='address' formData={modalFilterFormData} setFormData={setModalFilterFormData} />
                <InputMask type='date' title='Data de aniversário' fieldName='dateOfBirth' formData={modalFilterFormData} setFormData={setModalFilterFormData} />
                <InputMask title='Anotações' fieldName='notes' formData={modalFilterFormData} setFormData={setModalFilterFormData} />
                <div />
                <div />

                <Button
                    label='Cadastrar'
                    handleFunction={() => handleSubmit()}
                    style={{ fontSize: '0.75rem' }}
                />
            </div>
        </ModalGeneric>
    )
}