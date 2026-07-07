'use client';
import Button from '@/app/components/input/button';
import Dropdown, { iDropdownOption } from '@/app/components/input/drop-down';
import InputMask from '@/app/components/input/text';
import ModalGeneric from '@/app/components/modal/generic';
import styles from '@/app/components/modal/generic/index.module.scss';
import TagList from '@/app/components/tags/tag-list';
import handleGetPropName from '@/app/functions/get.propName';
import { handleNormalizeFetchUrl, handleRemoveDuplicateQueryParams } from '@/app/functions/normalize.fetch-url';
import { handleClearFormData, handleLoopFormData, handleSetDropdownOption } from '@/app/functions/set.formState';
import { useIsModalGrid } from '@/app/hooks/contexts/useGlobalContext';
import { Guid } from 'guid-typescript';
import { Dispatch, SetStateAction } from 'react';

interface iProps {
    isModalOpen: boolean;
    setIsModalOpen: Dispatch<SetStateAction<boolean>>;
    modalFilterFormData: iCompanyUserFormDataModalFilter;
    setModalFilterFormData: Dispatch<SetStateAction<iCompanyUserFormDataModalFilter>>;
    apiUrlRequest: string;
    setApiUrlRequest: Dispatch<SetStateAction<string>>;
    setCurrentPage: Dispatch<SetStateAction<number>>;
    companyUserRoleEnum: iDropdownOption<string | number | Guid>[] | undefined;
    moduleEnum: iDropdownOption<string | number | Guid>[] | undefined;
}

export interface iCompanyUserFormDataModalFilter {
    companyUserRole: string | null;
    modules: string | null;
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
    setCurrentPage,
    companyUserRoleEnum,
    moduleEnum
}: iProps) {

    const [isModalGrid,] = useIsModalGrid();
    const setCompanyUserRoleOption = handleSetDropdownOption(modalFilterFormData, setModalFilterFormData, handleGetPropName(modalFilterFormData, x => x.companyUserRole)[1]) as Dispatch<SetStateAction<iDropdownOption[]>>;
    const setModuleOption = handleSetDropdownOption(modalFilterFormData, setModalFilterFormData, handleGetPropName(modalFilterFormData, x => x.modules)[1]) as Dispatch<SetStateAction<iDropdownOption[]>>;

    function handleSubmit() {
        const data = handleLoopFormData({ formData: modalFilterFormData });
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
            showCloseButton={false}
            allowCloseOutsideClick={false}
            style={{ width: '50rem', padding: 0, background: 'transparent' }}
        >
            <div className={styles.modalCard}>
                <header className={styles.modalHeader}>
                    <div className={styles.modalHeaderLeft}>
                        <h1 className={styles.inputTitle}>
                            Filtre seus clientes
                        </h1>
                    </div>

                    <div className={styles.modalHeaderRight}>
                        <div className={styles.metaRow}>
                            <TagList
                                tags={[
                                    { label: '✖', color: 'transparent', handleFunction: () => setIsModalOpen(false), title: 'Fechar' }
                                ]}
                            />
                        </div>
                    </div>
                </header>

                <main className={styles.modalContent}>
                    <div className={`${isModalGrid ? styles.grid : 'modal-layout-flex'}`}>
                        <InputMask title='Nome' fieldName='fullName' formData={modalFilterFormData} setFormData={setModalFilterFormData} />
                        <InputMask title='E-mail' fieldName='email' formData={modalFilterFormData} setFormData={setModalFilterFormData} />
                        <Dropdown title='Tipo de colaborador' options={companyUserRoleEnum ?? []} selectedOption={modalFilterFormData.companyUserRole ?? undefined} setSelectedOption={setCompanyUserRoleOption} />
                        <Dropdown title='Módulos atribuídos' options={moduleEnum ?? []} selectedOption={modalFilterFormData.modules ?? undefined} setSelectedOption={setModuleOption} />
                    </div>
                </main>

                <footer className={styles.modalFooter}>
                    <Button
                        label='Limpar filtros'
                        handleFunction={() => handleClearFormData(setModalFilterFormData)}
                        styleType='transparent'
                        style={{ fontSize: '0.75rem' }}
                    />

                    <Button
                        label='Filtrar'
                        handleFunction={() => handleSubmit()}
                        style={{ fontSize: '0.75rem' }}
                    />
                </footer>
            </div>
        </ModalGeneric>
    )
}