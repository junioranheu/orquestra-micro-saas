import iClient from '@/app/api/consts/client';
import iSchedule, { CONSTS_SCHEDULE } from '@/app/api/consts/schedule';
import { iUser } from '@/app/api/consts/user';
import { Fetch } from '@/app/api/fetch';
import { iEvent } from '@/app/components/calendar/complete';
import Button from '@/app/components/input/button';
import Dropdown, { iDropdownOption } from '@/app/components/input/drop-down';
import InputMask from '@/app/components/input/text';
import ModalGeneric from '@/app/components/modal/generic';
import styles from '@/app/components/modal/generic/index.module.scss';
import Tags from '@/app/components/tags';
import ROUTES from '@/app/consts/routes';
import SYSTEM from '@/app/consts/system';
import { DATE_STYLE, handleFormatDate, handleFormatDateToInputValue, handleFormatTimeToInputValue, handleIsBeforeTodayWithTime } from '@/app/functions/format.date';
import { handleToBrazilDate } from '@/app/functions/get.date.brazil';
import handleGetPropName from '@/app/functions/get.propName';
import { handleNormalizeGuidArrayField, handleNormalizeGuidField } from '@/app/functions/normalize.guid';
import { handleClearFormData, handleInputFormStateChange, handleLoopFormData, handleSetDropdownOption } from '@/app/functions/set.formState';
import swal from '@/app/functions/swal';
import toast from '@/app/functions/toast';
import { handleTransformArrayToDropdownOptionsGuid } from '@/app/functions/transform.arrayToDropdownOptions';
import useWindowSize from '@/app/hooks/useWindowSize';
import { Guid } from 'guid-typescript';
import { useRouter } from 'next/navigation';
import { Dispatch, Fragment, SetStateAction, useCallback, useEffect, useState } from 'react';
import Swal from 'sweetalert2';

interface iProps {
    isOpen: boolean;
    setModalIsOpen: Dispatch<SetStateAction<boolean>>;
    type: 'edit' | 'create' | undefined;
    event: iEvent | undefined;
    companyId: Guid;
    companyUsers: iUser[] | undefined;
    clients: iClient[] | undefined;
    handleGetSchedules: () => Promise<void>;
}

export const CONSTS_PAYMENT_TYPE = [
    { value: 1, label: 'Dinheiro' },
    { value: 2, label: 'Credito' },
    { value: 3, label: 'Debito' },
    { value: 4, label: 'Pix' },
    { value: 5, label: 'TED' },
    { value: 6, label: 'Outro' }
] as iDropdownOption[];

export const CONSTS_SCHEDULE_STATUS = [
    { value: 1, label: 'Marcado' },
    { value: 2, label: 'Remarcado' },
    { value: 3, label: 'Concluído' },
    { value: 4, label: 'Cancelado' }
] as iDropdownOption[];

export const CONSTS_SCHEDULE_STATUS_BACKEND = [
    { value: 1, label: 'Scheduled' },
    { value: 2, label: 'Rescheduled' },
    { value: 3, label: 'Completed' },
    { value: 4, label: 'Canceled' }
] as iDropdownOption[];

export default function ModalCalendarView({ isOpen, setModalIsOpen, type, event, companyId, companyUsers, clients, handleGetSchedules }: iProps) {

    const router = useRouter();
    const windowSize = useWindowSize();

    const [canEdit,] = useState<boolean>(true);
    const [clientsDropDown, setClientsDropDown] = useState<iDropdownOption[]>();
    const [companyUsersDropDown, setCompanyUsersDropDown] = useState<iDropdownOption[]>();

    const [editing, setEditing] = useState<boolean>(false);
    const [saving, setSaving] = useState<boolean>(false);

    const [formData, setFormData] = useState<iSchedule>({
        scheduleId: SYSTEM.EMPTY_GUID,
        date: SYSTEM.EMPTY_DATE,
        durationMinutes: 0,
        paymentType: '',
        scheduleStatus: '',
        clientId: SYSTEM.EMPTY_GUID,
        companyId: SYSTEM.EMPTY_GUID,
        usersIds: [],
        isRestrictForSpecificUsers: false,
        customTitle: '',
        customUrl: '',
        observation: '',
        amountReceived: 0,
        dateEnd: SYSTEM.EMPTY_DATE,
        observations: [],
        usersOutput: [],
        timeStart: '',
        timeEnd: ''
    });

    const handleClose = useCallback(() => {
        setSaving(false);
        setEditing(false);
        setModalIsOpen(false);
        handleClearFormData(setFormData);
    }, [setModalIsOpen]);

    useEffect(() => {
        if (!isOpen) {
            return;
        }

        if (!clients || !clients.length) {
            swal({
                content: 'Nenhum cliente foi registrado. Por favor, cadastre pelo menos um cliente para prosseguir.',
                confirmBtnText: 'Cadastrar cliente',
                confirmFunction: () => { router.push(ROUTES.EMPRESA_CLIENTES) },
                icon: 'warning'
            });

            handleClose();
            return;
        }

        if (!companyUsers || !companyUsers.length) {
            swal({
                content: 'Nenhum membro foi registrado. Por favor, cadastre pelo menos um membro na sua empresa para prosseguir.',
                confirmBtnText: 'Cadastrar membro',
                confirmFunction: () => { router.push(ROUTES.EMPRESA_MEMBROS) },
                icon: 'warning'
            });

            handleClose();
            return;
        }

        const optionsCompanyUsers = handleTransformArrayToDropdownOptionsGuid(companyUsers ?? [], 'userId', 'user.fullName');
        setCompanyUsersDropDown(optionsCompanyUsers);

        const optionsClients = handleTransformArrayToDropdownOptionsGuid(clients ?? [], 'clientId', 'fullName');
        setClientsDropDown(optionsClients);

        if (type === 'create') {
            handleClearFormData(setFormData);

            setFormData(prev => ({
                ...prev,
                date: handleFormatDateToInputValue(event?.start ?? new Date())
            }));

            setEditing(true);
            return;
        }

        if (!event || !event.schedule || !event.schedule.client) {
            console.error(event);
            swal({ content: 'Houve uma falha ao abrir esse agendamento. Verifique o inspecionador de elementos.' });
            handleClose();
            return;
        }

        setFormData({
            scheduleId: event.schedule.scheduleId,
            date: handleFormatDateToInputValue(event.start),
            paymentType: event.schedule.paymentType,
            scheduleStatus: event.schedule.scheduleStatus,
            clientId: event.schedule.clientId,
            companyId: event.schedule.companyId,
            usersIds: event.schedule.usersIds,
            isRestrictForSpecificUsers: event.schedule.isRestrictForSpecificUsers,
            customTitle: event.schedule.customTitle,
            customUrl: event.schedule.customUrl,
            observation: event.schedule.observation,
            amountReceived: event.schedule.amountReceived,
            dateEnd: handleFormatDateToInputValue(event.end),
            observations: event.schedule.observations,
            timeStart: handleFormatTimeToInputValue(event.start),
            timeEnd: handleFormatTimeToInputValue(event.end)
        });
    }, [isOpen, type, event, companyUsers, clients, router, setModalIsOpen, handleClose]);

    const setCompanyUsersIdOption = handleSetDropdownOption(formData, setFormData, handleGetPropName(formData, x => x.usersIds)[1]) as Dispatch<SetStateAction<iDropdownOption[]>>;
    const setClientIdOption = handleSetDropdownOption(formData, setFormData, handleGetPropName(formData, x => x.clientId)[1]) as Dispatch<SetStateAction<iDropdownOption[]>>;
    const setPaymentTypeOption = handleSetDropdownOption(formData, setFormData, handleGetPropName(formData, x => x.paymentType)[1]) as Dispatch<SetStateAction<iDropdownOption[]>>;
    const setScheduleStatusOption = handleSetDropdownOption(formData, setFormData, handleGetPropName(formData, x => x.scheduleStatus)[1]) as Dispatch<SetStateAction<iDropdownOption[]>>;

    async function handleSave() {
        if (!canEdit) {
            return;
        }

        if (!formData.clientId || !formData.date || !formData.timeStart || !formData.dateEnd || !formData.timeEnd || !formData.scheduleStatus) {
            swal({ content: 'Preencha todos os campos obrigatórios (*) antes de prosseguir com esta ação.', icon: 'warning' });
            return;
        }

        if (formData.date) {
            const date = new Date(formData.date);

            if (date.getHours() === 0 && date.getMinutes() === 0) {
                const result = await Swal.fire({
                    text: 'Tem certeza de que deseja agendar para meia-noite?',
                    icon: 'warning',
                    showCancelButton: true,
                    confirmButtonText: 'Confirmar',
                    cancelButtonText: 'Voltar',
                    reverseButtons: true
                });

                if (!result.isConfirmed) {
                    return;
                }
            }
        }

        setEditing(false);
        setSaving(true);

        const data = handleLoopFormData(formData);
        const input = data.json as iSchedule;

        //#region Normalizar props
        // @ts-ignore;
        const scheduleStatusNormalized = CONSTS_SCHEDULE_STATUS_BACKEND?.find(x => x.value === CONSTS_SCHEDULE_STATUS?.find(y => y.label === formData.scheduleStatus?.label)?.value) ?? formData.scheduleStatus;
        // @ts-ignore; 
        formData.scheduleStatus = scheduleStatusNormalized;

        input.usersIds = handleNormalizeGuidArrayField(input.usersIds);
        input.clientId = handleNormalizeGuidField(input.clientId);
        input.companyId = companyId;

        input.date = new Date(`${input.date}T${input.timeStart}`);
        input.dateEnd = new Date(`${input.dateEnd}T${input.timeEnd}`);
        input.date = handleToBrazilDate(input.date);
        input.dateEnd = handleToBrazilDate(input.dateEnd);
        // console.log('input', input);
        // #endregion

        if (type === 'create') {
            const schedule = await Fetch.post({ url: CONSTS_SCHEDULE.post, body: input }) as iSchedule;

            if (schedule) {
                toast({ content: 'Agendamento criado com sucesso.' });
                handleGetSchedules();
                handleClose();
                return;
            }

            setEditing(true);
            setSaving(false);
            return;
        }

        const schedule = await Fetch.put({ url: CONSTS_SCHEDULE.put, body: input }) as iSchedule;

        if (schedule) {
            toast({ content: 'Agendamento atualizado com sucesso.' });
            handleGetSchedules();
            handleClose();
            return;
        }

        setEditing(true);
        setSaving(false);
        return;
    }

    // Forçar a copia da data inicial para a prop de data final;
    useEffect(() => {
        setFormData(prev => ({
            ...prev,
            dateEnd: formData.date
        }));
    }, [formData.date]);

    if (!isOpen || !event) {
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
                        <h1 className={styles.inputTitle}>{formData?.customTitle ? formData?.customTitle : event?.title}</h1>
                    </div>

                    <div className={styles.modalHeaderRight}>
                        <div className={styles.metaRow}>
                            {
                                type === 'edit' ? (
                                    <Tags
                                        tags={[
                                            { label: handleFormatDate(event.start, DATE_STYLE.DETALHADO_SEM_SEGUNDOS), color: handleIsBeforeTodayWithTime(event.start) ? 'var(--gray-dark)' : '' },
                                            { label: CONSTS_SCHEDULE_STATUS?.find(x => x.value === CONSTS_SCHEDULE_STATUS_BACKEND?.find(y => y.label === event.schedule?.scheduleStatus)?.value)?.label ?? '' },
                                            { label: event.schedule?.paymentType },
                                            { label: '✖', color: 'transparent', handleFunction: () => handleClose(), title: 'Fechar' },
                                        ]}
                                    />
                                ) : (
                                    <Tags
                                        tags={[
                                            { label: '✖', color: 'transparent', handleFunction: () => handleClose(), title: 'Fechar' },
                                        ]}
                                    />
                                )
                            }
                        </div>
                    </div>
                </header>

                <main className={styles.modalContent}>
                    <div className={styles.grid}>
                        <Dropdown title='Cliente' options={clientsDropDown ?? []} selectedOption={clientsDropDown?.find(x => x.value.toString() === formData.clientId?.toString())} setSelectedOption={setClientIdOption} isDisabled={!editing} isObligatory={true} />
                        <Dropdown title='Status' options={CONSTS_SCHEDULE_STATUS} selectedOption={CONSTS_SCHEDULE_STATUS?.find(x => x.value === CONSTS_SCHEDULE_STATUS_BACKEND?.find(y => y.label === formData.scheduleStatus)?.value)} setSelectedOption={setScheduleStatusOption} isDisabled={!editing} isObligatory={true} />
                        <InputMask title='Data de início' type='date' objectFormData={handleGetPropName(formData, x => x.date ?? '')} isDisabled={!editing} handleChange={(e) => handleInputFormStateChange(e, setFormData)} isObligatory={true} />
                        <InputMask title='Hora de início' type='time' objectFormData={handleGetPropName(formData, x => x.timeStart ?? '')} isDisabled={!editing} handleChange={(e) => handleInputFormStateChange(e, setFormData)} isObligatory={true} />
                        <InputMask title='Data e hora de encerramento' type='date' objectFormData={handleGetPropName(formData, x => x.dateEnd ?? '')} isDisabled={true} handleChange={(e) => handleInputFormStateChange(e, setFormData)} isObligatory={true} />
                        <InputMask title='Hora de encerramento' type='time' objectFormData={handleGetPropName(formData, x => x.timeEnd ?? '')} isDisabled={!editing} handleChange={(e) => handleInputFormStateChange(e, setFormData)} isObligatory={true} />
                        <Dropdown title='Membros da equipe' options={companyUsersDropDown ?? []} multiple={true} selectedOption={companyUsersDropDown?.filter(x => formData.usersIds?.some(id => id?.toString() === x.value?.toString())) || []} setSelectedOption={setCompanyUsersIdOption} isDisabled={!editing} />

                        <div className={styles.div}>
                            <label>Este agendamento é específico a um ou mais membros?</label>
                            <input type='text' value={formData?.usersIds?.length > 0 ? `Sim, para ${formData?.usersIds?.length} membro${formData?.usersIds?.length > 1 ? 's' : ''}` : 'Não — Liberado para qualquer membro'} readOnly={true} disabled={true} />
                        </div>

                        <InputMask title='Valor recebido' type='number' objectFormData={handleGetPropName(formData, x => x.amountReceived ?? '')} isDisabled={!editing} handleChange={(e) => handleInputFormStateChange(e, setFormData)} />
                        <Dropdown title='Tipo de pagamento' options={CONSTS_PAYMENT_TYPE} selectedOption={CONSTS_PAYMENT_TYPE.find(x => x.label === formData.paymentType)!} setSelectedOption={setPaymentTypeOption} isDisabled={!editing} />
                        <InputMask title='Título customizado' objectFormData={handleGetPropName(formData, x => x.customTitle ?? '')} isDisabled={!editing} handleChange={(e) => handleInputFormStateChange(e, setFormData)} />
                        <InputMask title='URL' objectFormData={handleGetPropName(formData, x => x.customUrl ?? '')} isDisabled={!editing} handleChange={(e) => handleInputFormStateChange(e, setFormData)} />

                        <div className={styles.div}>
                            <label>Observações da equipe</label>
                            <textarea className={styles.textarea} rows={3} value={formData.observation ?? ''} readOnly={!editing} onChange={(e) => setFormData((prev: typeof formData) => ({ ...prev, observation: e.target.value }))} />
                        </div>

                        {
                            type === 'edit' && (
                                <div className={styles.div}>
                                    <label>Observações do sistema</label>
                                    <textarea className={styles.textarea} rows={3} value={formData.observations} readOnly={true} />
                                </div>
                            )
                        }
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
                                    canEdit && (
                                        !editing ? (
                                            <Fragment>
                                                <Button label='Editar' handleFunction={() => setEditing(true)} />
                                            </Fragment>
                                        ) : (
                                            <Fragment>
                                                <Button label='Cancelar edição' handleFunction={() => setEditing(false)} isStyleSimple={true} />
                                                <Button label={saving ? 'Salvando...' : 'Salvar'} handleFunction={() => handleSave()} isDisabled={saving} />
                                            </Fragment>
                                        )
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