import iCompanySimpleOutput from '@/app/api/consts/company';
import ContentLoaderText from '@/app/components/content-loader/text';
import Button from '@/app/components/input/button';
import Dropdown, { iDropdownOption } from '@/app/components/input/drop-down';
import InputMask from '@/app/components/input/text';
import ModalGeneric from '@/app/components/modal/generic';
import styles from '@/app/components/modal/generic/index.module.scss';
import Tags from '@/app/components/tags';
import { COLORS } from '@/app/consts/colors';
import ROUTES from '@/app/consts/routes';
import SYSTEM from '@/app/consts/system';
import { handleFormatDateToInputValue } from '@/app/functions/format.date';
import handleGetPropName from '@/app/functions/get.propName';
import { handleClearFormData, handleSetDropdownOption } from '@/app/functions/set.formState';
import useApiGetCompanySituationEnum from '@/app/hooks/api/enums/useApiGetCompanySituationEnum';
import useWindowSize from '@/app/hooks/useWindowSize';
import { Dispatch, Fragment, SetStateAction, useCallback, useEffect, useState } from 'react';

interface iProps {
    isOpen: boolean;
    setModalIsOpen: Dispatch<SetStateAction<boolean>>;
    company: iCompanySimpleOutput | undefined;
}

export default function ModalEmpresaGerenciarView({ isOpen, setModalIsOpen, company }: iProps) {

    const windowSize = useWindowSize();

    const companyTypeEnum = useApiGetCompanySituationEnum({ enumName: 'CompanyTypeEnum' });
    const companySituationEnum = useApiGetCompanySituationEnum({ enumName: 'CompanySituationEnum' });

    const [editing, setEditing] = useState<boolean>(false);
    const [saving, setSaving] = useState<boolean>(false);

    const [formData, setFormData] = useState<iCompanySimpleOutput>({
        companyId: SYSTEM.EMPTY_GUID,
        name: '',
        email: '',
        companyType: '',
        companySituation: '',
        logoUrl: '',
        color: '',
        planType: '',
        planStartDate: SYSTEM.EMPTY_DATE,
        planEndDate: SYSTEM.EMPTY_DATE,
        modules: []
    });

    const handleClose = useCallback(() => {
        setSaving(false);
        setEditing(false);
        setModalIsOpen(false);
        handleClearFormData(setFormData);
    }, [setModalIsOpen]);

    useEffect(() => {
        if (!isOpen || !company) {
            return;
        }

        setFormData({
            companyId: company?.companyId!,
            name: company?.name ?? '',
            email: company?.email ?? '',
            companyType: company?.companyType ?? '',
            companySituation: company?.companySituation ?? '',
            logoUrl: company?.logoUrl ?? '',
            color: company?.color ?? '',
            planType: company?.planType ?? '',
            planStartDate: handleFormatDateToInputValue(company?.planStartDate ? new Date(company?.planStartDate) : new Date()),
            planEndDate: handleFormatDateToInputValue(company?.planEndDate ? new Date(company?.planEndDate) : new Date()),
            modules: company?.modules ?? []
        });
    }, [isOpen, company, setModalIsOpen, handleClose]);

    const setCompanyTypeOption = handleSetDropdownOption(formData, setFormData, handleGetPropName(formData, x => x.companyType)[1]) as Dispatch<SetStateAction<iDropdownOption[]>>;

    async function handleSave() {
        //     if (!formData.clientId || !formData.dateStart || !formData.timeStart || !formData.dateEnd || !formData.timeEnd || !formData.scheduleStatus) {
        //         swal({ content: 'Preencha todos os campos obrigatórios (*) antes de prosseguir com esta ação.', icon: 'warning' });
        //         return;
        //     }

        //     if (formData.dateStart) {
        //         const dateStart = new Date(formData.dateStart);

        //         if (dateStart.getHours() === 0 && dateStart.getMinutes() === 0) {
        //             const result = await Swal.fire({
        //                 text: 'Tem certeza de que deseja agendar para meia-noite?',
        //                 icon: 'warning',
        //                 showCancelButton: true,
        //                 confirmButtonText: 'Confirmar',
        //                 cancelButtonText: 'Voltar',
        //                 reverseButtons: true
        //             });

        //             if (!result.isConfirmed) {
        //                 return;
        //             }
        //         }
        //     }

        //     setEditing(false);
        //     setSaving(true);

        //     const data = handleLoopFormData(formData);
        //     const input = data.json as iSchedule;

        //     //#region Normalizar props
        //     // @ts-ignore;
        //     const scheduleStatusNormalized = CONSTS_SCHEDULE_STATUS_BACKEND?.find(x => x.value === CONSTS_SCHEDULE_STATUS?.find(y => y.label === formData.scheduleStatus?.label)?.value) ?? formData.scheduleStatus;
        //     // @ts-ignore; 
        //     formData.scheduleStatus = scheduleStatusNormalized;

        //     input.usersIds = handleNormalizeGuidArrayField(input.usersIds);
        //     input.clientId = handleNormalizeGuidField(input.clientId);
        //     input.companyId = companyId;

        //     input.dateStart = new Date(`${input.dateStart}T${input.timeStart}`);
        //     input.dateEnd = new Date(`${input.dateEnd}T${input.timeEnd}`);
        //     input.dateStart = handleToBrazilDate(input.dateStart);
        //     input.dateEnd = handleToBrazilDate(input.dateEnd);
        //     // console.log('input', input);
        //     // #endregion

        //     if (type === 'create') {
        //         const schedule = await Fetch.post({ url: CONSTS_SCHEDULE.post, body: input }) as iSchedule;

        //         if (schedule) {
        //             toast({ content: 'Agendamento criado com sucesso.' });
        //             handleGetSchedules();
        //             handleClose();
        //             return;
        //         }

        //         setEditing(true);
        //         setSaving(false);
        //         return;
        //     }

        //     const schedule = await Fetch.put({ url: CONSTS_SCHEDULE.put, body: input }) as iSchedule;

        //     if (schedule) {
        //         toast({ content: 'Agendamento atualizado com sucesso.' });
        //         handleGetSchedules();
        //         handleClose();
        //         return;
        //     }

        //     setEditing(true);
        //     setSaving(false);
        //     return;
    }

    if (!isOpen) {
        return;
    }

    return (
        <ModalGeneric
            isOpen={isOpen}
            setModalIsOpen={handleClose}
            showCloseButton={false}
            allowCloseOutsideClick={false}
            width={windowSize.width <= 1281 ? '85%' : '55%'}
            style={{ padding: 0, background: 'transparent' }}
        >
            <div className={styles.modalCard}>
                <header className={styles.modalHeader}>
                    <div className={styles.modalHeaderLeft}>
                        <h1 className={styles.inputTitle}><ContentLoaderText text={formData?.name} /></h1>
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
                    <div className={styles.grid}>
                        <InputMask title='Nome da empresa' fieldName='name' formData={formData} setFormData={setFormData} isDisabled={!editing} isObligatory={true} />
                        <InputMask title='E-mail' fieldName='email' formData={formData} setFormData={setFormData} isDisabled={!editing} isObligatory={true} />
                        <Dropdown title='Tipo' options={companyTypeEnum ?? []} selectedOption={companyTypeEnum?.find(x => x.value.toString() === formData.companyType?.toString())} setSelectedOption={setCompanyTypeOption} isDisabled={!editing} isObligatory={true} />
                        <Dropdown title='Situação' options={companySituationEnum ?? []} selectedOption={companySituationEnum?.find(x => x.value.toString() === formData.companySituation?.toString())} isDisabled={true} />
                        <InputMask title='Logo' fieldName='logoUrl' formData={formData} setFormData={setFormData} isDisabled={!editing} />
                        <Dropdown title='Cor de customização' options={COLORS ?? []} selectedOption={COLORS?.find(x => x.value.toString() === formData.color?.toString())} isDisabled={!editing} />
                        <InputMask title='Plano' fieldName='planType' formData={formData} setFormData={setFormData} isDisabled={!editing} />
                        <InputMask title='Início do plano' type='date' fieldName='planStartDate' formData={formData} setFormData={setFormData} isDisabled={true} />
                        <InputMask title='Fim do plano' type='date' fieldName='planEndDate' formData={formData} setFormData={setFormData} isDisabled={true} />
                        <InputMask title='Módulos' fieldName='modules' formData={formData} setFormData={setFormData} isDisabled={!editing} />
                    </div>
                </main>

                <footer className={styles.modalFooter}>
                    <div className={styles.buttonsRow}>
                        <Button label='Fechar' handleFunction={() => handleClose()} isStyleSimple={true} />
                    </div>

                    <div className={styles.buttonsRow}>
                        {
                            !editing ? (
                                <Fragment>
                                    <Button label='Visualizar membros' handleFunction={() => window.open(ROUTES.EMPRESA_MEMBROS, '_blank')} isStyleSimple={true} />
                                    <Button label='Visualizar clientes' handleFunction={() => window.open(ROUTES.EMPRESA_CLIENTES, '_blank')} isStyleSimple={true} />
                                    <Button label='Editar' handleFunction={() => setEditing(true)} />
                                </Fragment>
                            ) : (
                                <Fragment>
                                    <Button label='Cancelar edição' handleFunction={() => setEditing(false)} isStyleSimple={true} />
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