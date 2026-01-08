'use client';
import { iServiceOrder } from '@/app/api/consts/service-order';
import Button from '@/app/components/input/button';
import Dropdown, { iDropdownOption } from '@/app/components/input/drop-down';
import InputMask from '@/app/components/input/text';
import ModalGeneric from '@/app/components/modal/generic';
import styles from '@/app/components/modal/generic/index.module.scss';
import TagList from '@/app/components/tags/tag-list';
import handleGetPropName from '@/app/functions/get.propName';
import handleNormalizeEmptyKeyToId from '@/app/functions/normalize.emptyKeyToId';
import { handleNormalizeFetchUrl, handleRemoveDuplicateQueryParams } from '@/app/functions/normalize.fetch-url';
import { handleClearFormData, handleLoopFormData, handleSetDropdownOption } from '@/app/functions/set.formState';
import { useIsModalGrid } from '@/app/hooks/contexts/useGlobalContext';
import { Guid } from 'guid-typescript';
import { Dispatch, SetStateAction } from 'react';

interface iProps {
    isModalOpen: boolean;
    setIsModalOpen: Dispatch<SetStateAction<boolean>>;
    modalFilterFormData: iServiceOrder;
    setModalFilterFormData: Dispatch<SetStateAction<iServiceOrder>>;
    apiUrlRequest: string;
    setApiUrlRequest: Dispatch<SetStateAction<string>>;
    setCurrentPage: Dispatch<SetStateAction<number>>;
    clientsDropDown: iDropdownOption<string | number | Guid>[] | undefined;
}

export default function EmpresaServiceOrderModalFilters({
    isModalOpen,
    setIsModalOpen,
    modalFilterFormData,
    setModalFilterFormData,
    apiUrlRequest,
    setApiUrlRequest,
    setCurrentPage,
    clientsDropDown,
}: iProps) {

    const [isModalGrid,] = useIsModalGrid();
    const setStatusOption = handleSetDropdownOption(modalFilterFormData, setModalFilterFormData, handleGetPropName(modalFilterFormData, x => x.serviceOrderStatus ?? '')[1]) as Dispatch<SetStateAction<iDropdownOption[]>>;
    const setClientIdOption = handleSetDropdownOption(modalFilterFormData, setModalFilterFormData, handleGetPropName(modalFilterFormData, x => x.clientId ?? '')[1]) as Dispatch<SetStateAction<iDropdownOption[]>>;

    function handleSubmit() {
        const normalizedFormData = handleNormalizeEmptyKeyToId(modalFilterFormData, setModalFilterFormData, 'clientId'); // Workaround bizarro para um bug bizarro...
        const data = handleLoopFormData(normalizedFormData, 'label');
        const url = handleNormalizeFetchUrl(apiUrlRequest, data);
        const urlNormalized = handleRemoveDuplicateQueryParams(url);

        setApiUrlRequest(urlNormalized);
        setCurrentPage(1);
        setIsModalOpen(false);
    }

    function handleClearFilters() {
        if (modalFilterFormData.clientId) {
            window.location.reload();
        }

        handleClearFormData(setModalFilterFormData);
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
                            Filtre suas ordens de serviço
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
                        <InputMask title='Título' fieldName='title' formData={modalFilterFormData} setFormData={setModalFilterFormData} />
                        <InputMask type='date' title='Data de Execução' fieldName='executionDate' formData={modalFilterFormData} setFormData={setModalFilterFormData} />
                        <Dropdown title='Cliente' options={clientsDropDown ?? []} selectedOption={clientsDropDown?.find(x => x.value.toString() === modalFilterFormData?.clientId?.toString())} setSelectedOption={setClientIdOption} />
                    </div>
                </main>

                <footer className={styles.modalFooter}>
                    <Button
                        label='Limpar filtros'
                        handleFunction={() => handleClearFilters()}
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