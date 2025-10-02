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
import { handleClearFormData, handleLoopFormData, handleSetDropdownOption } from '@/app/functions/set.formState';
import swal from '@/app/functions/swal';
import toast from '@/app/functions/toast';
import { handleTransformArrayToDropdownOptionsGuid } from '@/app/functions/transform.arrayToDropdownOptions';
import useApiGetCompanySituationEnum from '@/app/hooks/api/enums/useApiGetCompanySituationEnum';
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

export default function ModalCalendarView({ isOpen, setModalIsOpen, type, event, companyId, companyUsers, clients, handleGetSchedules }: iProps) {

    const router = useRouter();
    const windowSize = useWindowSize();

    const [canEdit,] = useState<boolean>(true);
    const [companyUsersDropDown, setCompanyUsersDropDown] = useState<iDropdownOption[]>();
    const [clientsDropDown, setClientsDropDown] = useState<iDropdownOption[]>();

    const [editing, setEditing] = useState<boolean>(false);
    const [saving, setSaving] = useState<boolean>(false);

    const paymentTypeEnum = useApiGetCompanySituationEnum({ enumName: 'PaymentTypeEnum' });
    const scheduleStatusEnum = useApiGetCompanySituationEnum({ enumName: 'ScheduleStatusEnum' });

    const [formData, setFormData] = useState<iSchedule>({
        scheduleId: SYSTEM.EMPTY_GUID,
        dateStart: SYSTEM.EMPTY_DATE,
        dateEnd: SYSTEM.EMPTY_DATE,
        timeStart: '',
        timeEnd: '',
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
        observations: [],
        usersOutput: []
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

            const scheduleStatus = scheduleStatusEnum?.find(x => x.label === 'Marcado')?.value?.toString() ?? '';
            const dateStart = handleFormatDateToInputValue(event?.start ?? new Date());

            setFormData(prev => ({
                ...prev,
                scheduleStatus: scheduleStatus,
                dateStart: dateStart,
                dateEnd: dateStart // Igual ao start;
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
            dateStart: handleFormatDateToInputValue(event.start),
            dateEnd: handleFormatDateToInputValue(event.end),
            timeStart: handleFormatTimeToInputValue(event.start),
            timeEnd: handleFormatTimeToInputValue(event.end),
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
            observations: event.schedule.observations
        });
    }, [isOpen, type, event, companyUsers, clients, scheduleStatusEnum, router, setModalIsOpen, handleClose]);

    const setCompanyUsersIdOption = handleSetDropdownOption(formData, setFormData, handleGetPropName(formData, x => x.usersIds)[1]) as Dispatch<SetStateAction<iDropdownOption[]>>;
    const setClientIdOption = handleSetDropdownOption(formData, setFormData, handleGetPropName(formData, x => x.clientId)[1]) as Dispatch<SetStateAction<iDropdownOption[]>>;
    const setPaymentTypeOption = handleSetDropdownOption(formData, setFormData, handleGetPropName(formData, x => x.paymentType)[1]) as Dispatch<SetStateAction<iDropdownOption[]>>;
    const setScheduleStatusOption = handleSetDropdownOption(formData, setFormData, handleGetPropName(formData, x => x.scheduleStatus)[1]) as Dispatch<SetStateAction<iDropdownOption[]>>;

    async function handleSave() {
        if (!canEdit) {
            return;
        }

        if (!formData.clientId || !formData.dateStart || !formData.timeStart || !formData.dateEnd || !formData.timeEnd || !formData.scheduleStatus) {
            swal({ content: SYSTEM.WARN_FILL_OBLIGATORY_FIELDS, icon: 'warning' });
            return;
        }

        if (formData.dateStart) {
            const dateStart = new Date(formData.dateStart);

            if (dateStart.getHours() === 0 && dateStart.getMinutes() === 0) {
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
        input.usersIds = handleNormalizeGuidArrayField(input.usersIds);
        input.clientId = handleNormalizeGuidField(input.clientId);
        input.companyId = companyId;
        input.scheduleStatus = Number(input.scheduleStatus);

        input.dateStart = new Date(`${input.dateStart}T${input.timeStart}`);
        input.dateEnd = new Date(`${input.dateEnd}T${input.timeEnd}`);
        input.dateStart = handleToBrazilDate(input.dateStart);
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
                                            { label: scheduleStatusEnum?.find(x => x.value.toString() === formData.scheduleStatus?.toString())?.label ?? '' },
                                            { label: event.schedule?.paymentType },
                                            { label: '✖', color: 'transparent', handleFunction: () => handleClose(), title: 'Fechar' }
                                        ]}
                                    />
                                ) : (
                                    <Tags
                                        tags={[
                                            { label: '✖', color: 'transparent', handleFunction: () => handleClose(), title: 'Fechar' }
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
                        <Dropdown title='Status' options={scheduleStatusEnum ?? []} selectedOption={scheduleStatusEnum?.find(x => x.value.toString() === formData.scheduleStatus?.toString())} setSelectedOption={setScheduleStatusOption} isDisabled={!editing || (type === 'create')} isObligatory={true} />
                        <InputMask title='Data de início' type='date' fieldName='dateStart' formData={formData} setFormData={setFormData} isDisabled={!editing} isObligatory />
                        <InputMask title='Hora de início' type='time' fieldName='timeStart' formData={formData} setFormData={setFormData} isDisabled={!editing} isObligatory />
                        <InputMask title='Data e hora de encerramento' type='date' fieldName='dateEnd' formData={formData} setFormData={setFormData} isDisabled isObligatory />
                        <InputMask title='Hora de encerramento' type='time' fieldName='timeEnd' formData={formData} setFormData={setFormData} isDisabled={!editing} isObligatory />
                        <Dropdown title='Membros da equipe' options={companyUsersDropDown ?? []} multiple={true} selectedOption={companyUsersDropDown?.filter(x => formData.usersIds?.some(id => id?.toString() === x.value?.toString())) || []} setSelectedOption={setCompanyUsersIdOption} isDisabled={!editing} />

                        <div className={styles.div}>
                            <label>Este agendamento é específico a um ou mais membros?</label>
                            <input type='text' value={formData?.usersIds?.length > 0 ? `Sim, para ${formData?.usersIds?.length} membro${formData?.usersIds?.length > 1 ? 's' : ''}` : 'Não — Liberado para qualquer membro'} readOnly={true} disabled={true} />
                        </div>

                        <InputMask title='Valor recebido' type='number' fieldName='amountReceived' formData={formData} setFormData={setFormData} isDisabled={!editing} />
                        <Dropdown title='Tipo de pagamento' options={paymentTypeEnum ?? []} selectedOption={paymentTypeEnum?.find(x => x.value.toLocaleString() === formData.paymentType?.toString())} setSelectedOption={setPaymentTypeOption} isDisabled={!editing} />
                        <InputMask title='Título customizado' fieldName='customTitle' formData={formData} setFormData={setFormData} isDisabled={!editing} />
                        <InputMask title='URL' fieldName='customUrl' formData={formData} setFormData={setFormData} isDisabled={!editing} />

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