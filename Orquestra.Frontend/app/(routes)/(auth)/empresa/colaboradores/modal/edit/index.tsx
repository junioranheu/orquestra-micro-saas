'use client';
import { CONSTS_COMPANY_USER, iCompanyUser } from '@/app/api/consts/company-user';
import { Fetch } from '@/app/api/fetch';
import ContentLoaderText from '@/app/components/content-loader/text';
import Button from '@/app/components/input/button';
import Dropdown, { iDropdownOption } from '@/app/components/input/drop-down';
import ModalGeneric from '@/app/components/modal/generic';
import styles from '@/app/components/modal/generic/index.module.scss';
import Tags from '@/app/components/tags';
import SYSTEM from '@/app/consts/system';
import handleGetPropName from '@/app/functions/get.propName';
import { handleClearFormData, handleLoopFormData, handleSetDropdownOption } from '@/app/functions/set.formState';
import swal from '@/app/functions/swal';
import { Guid } from 'guid-typescript';
import { Dispatch, Fragment, SetStateAction, useCallback, useEffect, useState } from 'react';

interface iProps {
    isModalOpen: boolean;
    setIsModalOpen: Dispatch<SetStateAction<boolean>>;
    user: iCompanyUser | undefined;
    setTrigger: Dispatch<SetStateAction<Date>>;
    companyUserRoleEnum: iDropdownOption<string | number | Guid>[] | undefined;
    moduleEnum: iDropdownOption<string | number | Guid>[] | undefined;
}

interface iFormData {
    userModules: string[] | null;
    companyUserRole: string | null;
    userId: Guid | undefined;
    companyId: Guid | undefined;
}

export default function EmpresaMembrosModalEdit({
    isModalOpen,
    setIsModalOpen,
    user,
    setTrigger,
    companyUserRoleEnum,
    moduleEnum
}: iProps) {

    const [editing, setEditing] = useState<boolean>(false);
    const [saving, setSaving] = useState<boolean>(false);

    const [formData, setFormData] = useState<iFormData>({
        userModules: [],
        companyUserRole: null,
        userId: user?.userId,
        companyId: user?.companyId
    });

    const setCompanyUserRoleOption = handleSetDropdownOption(formData, setFormData, handleGetPropName(formData, x => x.companyUserRole)[1]) as Dispatch<SetStateAction<iDropdownOption[]>>;
    const setModuleOption = handleSetDropdownOption(formData, setFormData, handleGetPropName(formData, x => x.userModules)[1]) as Dispatch<SetStateAction<iDropdownOption[]>>;

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

        if (!isModalOpen) {
            return;
        }

        setFormData({
            userModules: user?.userModules as string[] ?? [],
            companyUserRole: user?.companyUserRole ?? null,
            userId: user?.userId,
            companyId: user?.companyId
        });
    }, [isModalOpen, user, setIsModalOpen, handleClose]);

    async function handleSave() {
        if (!formData.companyUserRole) {
            swal({ content: SYSTEM.WARN_FILL_OBLIGATORY_FIELDS, icon: 'warning' });
            return;
        }

        setEditing(false);
        setSaving(true);

        const data = handleLoopFormData(formData);
        const input = data.json as iFormData;

        // Normalizar;
        if (isNaN(Number(input.companyUserRole))) { // Se não for um número, é preciso buscar no array;
            input.companyUserRole = companyUserRoleEnum?.find(x => x.label === input.companyUserRole)?.value as string;
        }

        const output = await Fetch.put({ url: CONSTS_COMPANY_USER.put, body: input });

        if (output) {
            swal({
                content: 'Colaborador atualizado com sucesso.',
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

    const [isAdmSelected, setIsAdmSelected] = useState<boolean>(false);

    useEffect(() => {
        // @ts-expect-error: dynamic; 
        if (formData.companyUserRole?.label === 'Administrador' || formData.companyUserRole === 'Administrador') {
            setIsAdmSelected(true);
            setFormData(prev => ({ ...prev, userModules: [] }));
        } else {
            setIsAdmSelected(false);
        }
    }, [formData.companyUserRole]);

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
                            <ContentLoaderText content={(`Editar colaborador: ${user?.user?.fullName}`)} />
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
                    <div className='modal-layout-flex'>
                        <Dropdown
                            title='Tipo de colaborador'
                            options={companyUserRoleEnum ?? []}
                            selectedOption={companyUserRoleEnum?.find(x => x.label === formData.companyUserRole) ?? undefined}
                            setSelectedOption={setCompanyUserRoleOption}
                            isDisabled={!editing}
                        />

                        {
                            !isAdmSelected && (
                                <Dropdown
                                    title='Módulos atribuídos'
                                    options={moduleEnum ?? []}
                                    selectedOption={moduleEnum?.filter(x => formData.userModules?.map(String).includes(String(x.value))) ?? []}
                                    setSelectedOption={setModuleOption}
                                    isDisabled={!editing || isAdmSelected}
                                    isMultiple={true}
                                />
                            )
                        }
                    </div>
                </main>

                <footer className={styles.modalFooter}>
                    <div className={styles.buttonsRow}>
                        <Button label='Fechar' handleFunction={() => handleClose()} styleType='transparent' />
                    </div>

                    <div className={styles.buttonsRow}>
                        {
                            !editing ? (
                                <Fragment>
                                    <Button label='Editar' handleFunction={() => setEditing(true)} />
                                </Fragment>
                            ) : (
                                <Fragment>
                                    <Button label='Cancelar edição' handleFunction={() => setEditing(false)} styleType='transparent' />
                                    <Button label={saving ? 'Salvando...' : 'Salvar'} handleFunction={() => handleSave()} isDisabled={saving} />
                                </Fragment>
                            )
                        }
                    </div>
                </footer>
            </div>
        </ModalGeneric>
    )
}