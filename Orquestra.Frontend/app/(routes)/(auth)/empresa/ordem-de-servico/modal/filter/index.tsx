'use client';
import { iServiceOrder } from '@/app/api/consts/service-order';
import Button from '@/app/components/input/button';
import InputMask from '@/app/components/input/text';
import ModalGeneric from '@/app/components/modal/generic';
import styles from '@/app/components/modal/generic/index.module.scss';
import TagList from '@/app/components/tags/tag-list';
import { handleNormalizeFetchUrl, handleRemoveDuplicateQueryParams } from '@/app/functions/normalize.fetch-url';
import { handleClearFormData, handleLoopFormData } from '@/app/functions/set.formState';
import { useIsModalGrid } from '@/app/hooks/contexts/useGlobalContext';
import { Dispatch, SetStateAction } from 'react';

interface iProps {
    isModalOpen: boolean;
    setIsModalOpen: Dispatch<SetStateAction<boolean>>;
    modalFilterFormData: iServiceOrder;
    setModalFilterFormData: Dispatch<SetStateAction<iServiceOrder>>;
    apiUrlRequest: string;
    setApiUrlRequest: Dispatch<SetStateAction<string>>;
    setCurrentPage: Dispatch<SetStateAction<number>>;
}

export default function EmpresaServiceOrderModalFilters({
    isModalOpen,
    setIsModalOpen,
    modalFilterFormData,
    setModalFilterFormData,
    apiUrlRequest,
    setApiUrlRequest,
    setCurrentPage
}: iProps) {

    const [isModalGrid,] = useIsModalGrid();

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
                        {/* <Select title='Status' fieldName='serviceOrderStatus'  options={serviceOrderStatusEnum}  formData={modalFilterFormData} setFormData={setModalFilterFormData}/> */}
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