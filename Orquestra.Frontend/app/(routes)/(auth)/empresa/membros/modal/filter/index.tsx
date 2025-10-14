'use client';
import Button from '@/app/components/input/button';
import Dropdown, { iDropdownOption } from '@/app/components/input/drop-down';
import InputMask from '@/app/components/input/text';
import ModalGeneric from '@/app/components/modal/generic';
import handleGetPropName from '@/app/functions/get.propName';
import { handleNormalizeFetchUrl, handleRemoveDuplicateQueryParams } from '@/app/functions/normalize.fetch-url';
import { handleClearFormData, handleLoopFormData, handleSetDropdownOption } from '@/app/functions/set.formState';
import useApiGetCompanySituationEnum from '@/app/hooks/api/enums/useApiGetCompanySituationEnum';
import { Dispatch, SetStateAction } from 'react';

interface iModalFilterParams {
    isModalOpen: boolean;
    setIsModalOpen: Dispatch<SetStateAction<boolean>>;
    modalFilterFormData: iCompanyUserFormDataModalFilter;
    setModalFilterFormData: Dispatch<SetStateAction<iCompanyUserFormDataModalFilter>>;
    apiUrlRequest: string;
    setApiUrlRequest: Dispatch<SetStateAction<string>>;
    setCurrentPage: Dispatch<SetStateAction<number>>;
}

export interface iCompanyUserFormDataModalFilter {
    companyUserRole: string | null;
    modules: string[] | null;
    fullName: string | null;
    email: string | null;
}

export default function EmpresaMembrosModalFilters({
    isModalOpen,
    setIsModalOpen,
    modalFilterFormData,
    setModalFilterFormData,
    apiUrlRequest,
    setApiUrlRequest,
    setCurrentPage
}: iModalFilterParams) {

    const companyUserRoleEnum = useApiGetCompanySituationEnum({ enumName: 'CompanyUserRoleEnum' });
    const moduleEnum = useApiGetCompanySituationEnum({ enumName: 'ModuleEnum' });

    const setCompanyUserRoleOption = handleSetDropdownOption(modalFilterFormData, setModalFilterFormData, handleGetPropName(modalFilterFormData, x => x.companyUserRole)[1]) as Dispatch<SetStateAction<iDropdownOption[]>>;
    const setModuleOption = handleSetDropdownOption(modalFilterFormData, setModalFilterFormData, handleGetPropName(modalFilterFormData, x => x.modules)[1]) as Dispatch<SetStateAction<iDropdownOption[]>>;

    function handleSubmit() {
        const data = handleLoopFormData(modalFilterFormData, 'value');
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
            title='Filtre seus membros'
            overlayColor={0.5}
            allowCloseOutsideClick={false}
            style={{ width: '50rem' }}
        >
            <div className='modal-layout-flex'>
                <InputMask title='Nome' fieldName='fullName' formData={modalFilterFormData} setFormData={setModalFilterFormData} />
                <InputMask title='E-mail' fieldName='email' formData={modalFilterFormData} setFormData={setModalFilterFormData} />
                {/* @ts-expect-error: dinâmico e pode não ter props compatíveis; */}
                <Dropdown title='Tipo' options={companyUserRoleEnum ?? []} selectedOption={companyUserRoleEnum?.find(x => x.value.toString() === modalFilterFormData.companyUserRole?.value.toString())} setSelectedOption={setCompanyUserRoleOption} />
                <Dropdown title='Módulos atribuídos' options={moduleEnum ?? []} selectedOption={moduleEnum?.find(x => x.value.toString() === modalFilterFormData.modules?.toString())} setSelectedOption={setModuleOption} multiple={true} />

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