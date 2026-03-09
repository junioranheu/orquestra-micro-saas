import { iMe } from '@/app/api/consts/auth';
import { iClient } from '@/app/api/consts/client';
import { CONSTS_SCHEDULE, iSchedule } from '@/app/api/consts/schedule';
import { iUser } from '@/app/api/consts/user';
import { Fetch } from '@/app/api/fetch';
import { iEvent } from '@/app/components/calendar/complete';
import Icon from '@/app/components/icon';
import Button from '@/app/components/input/button';
import Dropdown, { iDropdownOption } from '@/app/components/input/drop-down';
import DropDownCliente from '@/app/components/input/drop-down-custom/cliente';
import InputMask from '@/app/components/input/text';
import ModalGeneric from '@/app/components/modal/generic';
import styles from '@/app/components/modal/generic/index.module.scss';
import TagList from '@/app/components/tags/tag-list';
import WhatsappWebShortcut from '@/app/components/whatsapp/whatsapp-web-shortcut';
import ROUTES from '@/app/consts/routes';
import SYSTEM from '@/app/consts/system';
import { DATE_STYLE, handleFormatDate, handleFormatDateToInputValue, handleFormatTimeToInputValue, handleIsBeforeTodayWithTime } from '@/app/functions/format.date';
import { handleToBrazilDate } from '@/app/functions/get.date.brazil';
import { handleGetFirstName } from '@/app/functions/get.formatUserName';
import handleGetPropName from '@/app/functions/get.propName';
import { handleNormalizeGuidArrayField, handleNormalizeGuidField } from '@/app/functions/normalize.guid';
import { handleClearFormData, handleLoopFormData, handleSetDropdownOption } from '@/app/functions/set.formState';
import swal from '@/app/functions/swal';
import toast from '@/app/functions/toast';
import { handleTransformArrayToDropdownOptionsGuid } from '@/app/functions/transform.arrayToDropdownOptions';
import useApiGetEnum from '@/app/hooks/api/useApiGetEnum';
import { useIsModalGrid } from '@/app/hooks/contexts/useGlobalContext';
import useWindowSize from '@/app/hooks/useWindowSize';
import { Guid } from 'guid-typescript';
import { useRouter } from 'next/navigation';
import { Dispatch, Fragment, SetStateAction, useCallback, useEffect, useState } from 'react';
import Swal from 'sweetalert2';

interface iProps {
    isOpen: boolean;
    setIsModalOpen: Dispatch<SetStateAction<boolean>>;
    type: 'edit' | 'create';
    me: iMe | undefined;
    event: iEvent | undefined;
    companyId: Guid;
    companyUsers: iUser[] | undefined;
    clients: iClient[] | undefined;
    handleGetSchedules: () => Promise<void>;
}

export default function ModalCalendarView({ isOpen, setIsModalOpen, type, me, event, companyId, companyUsers, clients, handleGetSchedules }: iProps) {

    const router = useRouter();
    const windowSize = useWindowSize();
    const [isModalGrid,] = useIsModalGrid();

    const [companyUsersDropDown, setCompanyUsersDropDown] = useState<iDropdownOption[]>();
    const [clientsDropDown, setClientsDropDown] = useState<iDropdownOption[]>();

    const [editing, setEditing] = useState<boolean>(false);
    const [saving, setSaving] = useState<boolean>(false);

    const paymentTypeEnum = useApiGetEnum({ enumName: 'PaymentTypeEnum' });
    const scheduleStatusEnum = useApiGetEnum({ enumName: 'ScheduleStatusEnum' });
    const scheduleTypeEnum = useApiGetEnum({ enumName: 'ScheduleTypeEnum' });

    const [formData, setFormData] = useState<iSchedule>({
        scheduleId: SYSTEM.EMPTY_GUID,
        dateStart: SYSTEM.EMPTY_DATE,
        dateEnd: SYSTEM.EMPTY_DATE,
        timeStart: '',
        timeEnd: '',
        paymentType: '',
        scheduleStatus: '',
        scheduleType: '',
        clientId: SYSTEM.EMPTY_GUID,
        client: undefined,
        companyId: SYSTEM.EMPTY_GUID,
        usersIds: [],
        isRestrictForSpecificUsers: false,
        customTitle: '',
        customUrl: '',
        observation: '',
        amountReceived: 0,
        observations: [],
        usersOutput: [],
        messageIntegrationWhatsapp: ''
    });

    const handleClose = useCallback(() => {
        setSaving(false);
        setEditing(false);
        setIsModalOpen(false);
        handleClearFormData(setFormData);
    }, [setIsModalOpen]);

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
                content: 'Nenhum colaborador foi registrado. Por favor, cadastre pelo menos um colaborador na sua empresa para prosseguir.',
                confirmBtnText: 'Cadastrar colaborador',
                confirmFunction: () => { router.push(ROUTES.EMPRESA_COLABORADORES) },
                icon: 'warning'
            });

            handleClose();
            return;
        }

        const optionsCompanyUsers = handleTransformArrayToDropdownOptionsGuid(companyUsers ?? [], 'userId', 'user.fullName');
        setCompanyUsersDropDown(optionsCompanyUsers);

        const optionsClients = handleTransformArrayToDropdownOptionsGuid(clients ?? [], 'clientId', ['fullName', 'phone', 'email']);
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
            scheduleType: event.schedule.scheduleType,
            clientId: event.schedule.clientId,
            client: event.schedule.client,
            companyId: event.schedule.companyId,
            usersIds: event.schedule.usersIds,
            isRestrictForSpecificUsers: event.schedule.isRestrictForSpecificUsers,
            customTitle: event.schedule.customTitle,
            customUrl: event.schedule.customUrl,
            observation: event.schedule.observation,
            amountReceived: event.schedule.amountReceived,
            observations: event.schedule.observations,
            messageIntegrationWhatsapp: event.schedule.messageIntegrationWhatsapp
        });
    }, [isOpen, type, event, companyUsers, clients, scheduleStatusEnum, router, setIsModalOpen, handleClose]);

    const setCompanyUsersIdOption = handleSetDropdownOption(formData, setFormData, handleGetPropName(formData, x => x.usersIds)[1]) as Dispatch<SetStateAction<iDropdownOption[]>>;
    const setClientIdOption = handleSetDropdownOption(formData, setFormData, handleGetPropName(formData, x => x.clientId)[1]) as Dispatch<SetStateAction<iDropdownOption[]>>;
    const setPaymentTypeOption = handleSetDropdownOption(formData, setFormData, handleGetPropName(formData, x => x.paymentType)[1]) as Dispatch<SetStateAction<iDropdownOption[]>>;
    const setScheduleStatusOption = handleSetDropdownOption(formData, setFormData, handleGetPropName(formData, x => x.scheduleStatus)[1]) as Dispatch<SetStateAction<iDropdownOption[]>>;
    const setScheduleTypeOption = handleSetDropdownOption(formData, setFormData, handleGetPropName(formData, x => x.scheduleType)[1]) as Dispatch<SetStateAction<iDropdownOption[]>>;

    async function handleSave() {
        if ((!formData.clientId || formData?.clientId === SYSTEM.EMPTY_GUID) || !formData.dateStart || !formData.timeStart || !formData.dateEnd || !formData.timeEnd || !formData.scheduleStatus || !formData.scheduleType) {
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

        const data = handleLoopFormData({ formData });
        const input = data.json as iSchedule;

        //#region Normalizar props
        input.usersIds = handleNormalizeGuidArrayField(input.usersIds);
        input.clientId = handleNormalizeGuidField(input.clientId);
        input.companyId = companyId;
        input.scheduleStatus = Number(input.scheduleStatus);
        input.scheduleType = Number(input.scheduleType);

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

    async function handleDisable(event: iEvent) {
        swal({
            content: 'Você tem certeza que deseja excluir este agendamento? Este processo é irreversível.',
            confirmBtnText: 'Sim, desejo excluir',
            mustConfirm: true,
            checkboxLabel: 'Sim, confirmo',
            confirmFunction: async () => {
                const input = { scheduleId: event.schedule.scheduleId };
                const schedule = await Fetch.put({ url: CONSTS_SCHEDULE.disable, body: input });

                if (schedule) {
                    toast({ content: 'Agendamento excluído com sucesso.' });
                    handleGetSchedules();
                    handleClose();
                    return;
                }

                toast({ content: 'Não foi possível excluir este agendamento.' });
            },
            cancelBtnText: 'Voltar',
            icon: 'question'
        });
    }

    useEffect(() => {
        setFormData(prev => ({
            ...prev,
            dateEnd: prev.dateStart
        }));
    }, [formData.dateStart]);

    if (!isOpen || !event) {
        return;
    }

    return (
        <ModalGeneric
            isOpen={isOpen}
            setIsModalOpen={setIsModalOpen}
            onRequestClose={handleClose}
            showCloseButton={false}
            allowCloseOutsideClick={false}
            width={windowSize.width <= 1366 ? '85%' : '55%'}
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
                                    <TagList
                                        tags={[
                                            { label: handleFormatDate(event.start, DATE_STYLE.DETALHADO_SEM_SEGUNDOS), color: handleIsBeforeTodayWithTime(event.start) ? 'var(--gray-dark)' : '' },
                                            { label: scheduleStatusEnum?.find(x => x.value.toString() === formData.scheduleStatus?.toString())?.label ?? '' },
                                            { label: scheduleTypeEnum?.find(x => x.value.toString() === formData.scheduleType?.toString())?.label ?? '' },
                                            { label: event.schedule?.paymentType },
                                            { label: '✖', color: 'transparent', handleFunction: () => handleClose(), title: 'Fechar' }
                                        ]}
                                    />
                                ) : (
                                    <TagList
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
                    <div className={`${isModalGrid ? styles.grid : 'modal-layout-flex'}`}>
                        <div className={`${styles.div} ${styles.full}`}>
                            <DropDownCliente editing={editing} clientsDropDown={clientsDropDown} setClientIdOption={setClientIdOption} clientId={formData.clientId ?? Guid.EMPTY} isObligatory={true} showNewClientButton={true} />
                        </div>

                        <Dropdown title='Tipo de agendamento' options={scheduleTypeEnum ?? []} selectedOption={scheduleTypeEnum?.find(x => x.value.toString() === formData.scheduleType?.toString())} setSelectedOption={setScheduleTypeOption} isDisabled={!editing} isObligatory={true} />
                        <Dropdown title='Status' options={scheduleStatusEnum ?? []} selectedOption={scheduleStatusEnum?.find(x => x.value.toString() === formData.scheduleStatus?.toString())} setSelectedOption={setScheduleStatusOption} isDisabled={!editing || (type === 'create')} isObligatory={true} />
                        <InputMask title='Data de início' type='date' fieldName='dateStart' formData={formData} setFormData={setFormData} isDisabled={!editing} isObligatory />
                        <InputMask title='Hora de início' type='time' fieldName='timeStart' formData={formData} setFormData={setFormData} isDisabled={!editing} isObligatory />
                        <InputMask title='Data e hora de encerramento' type='date' fieldName='dateEnd' formData={formData} setFormData={setFormData} isDisabled isObligatory />
                        <InputMask title='Hora de encerramento' type='time' fieldName='timeEnd' formData={formData} setFormData={setFormData} isDisabled={!editing} isObligatory />
                        <Dropdown title='Colaboradores da equipe' options={companyUsersDropDown ?? []} isMultiple={true} selectedOption={companyUsersDropDown?.filter(x => formData.usersIds?.some(id => id?.toString() === x.value?.toString())) || []} setSelectedOption={setCompanyUsersIdOption} isDisabled={!editing} />

                        {/* <div className={styles.div}>
                            <label>É um gendamento específico a um ou mais colaboradores?</label>
                            <input type='text' value={formData?.usersIds?.length > 0 ? `Sim, para ${formData?.usersIds?.length} colaborador${formData?.usersIds?.length > 1 ? 'es' : ''}` : 'Não — Liberado para qualquer colaborador'} readOnly={true} disabled={true} />
                        </div> */}

                        <InputMask title='Valor recebido' type='number' fieldName='amountReceived' formData={formData} setFormData={setFormData} isDisabled={!editing} />

                        <div className={`${styles.div} ${styles.full}`}>
                            <Dropdown title='Tipo de pagamento' options={paymentTypeEnum ?? []} selectedOption={paymentTypeEnum?.find(x => x.value.toLocaleString() === formData.paymentType?.toString())} setSelectedOption={setPaymentTypeOption} isDisabled={!editing} />
                        </div>

                        <InputMask title='Título do evento' fieldName='customTitle' formData={formData} setFormData={setFormData} isDisabled={!editing} />
                        <InputMask title='URL' fieldName='customUrl' formData={formData} setFormData={setFormData} isDisabled={!editing} />

                        <div className={`${styles.div} ${styles.full}`}>
                            <label>Observações da equipe</label>
                            <textarea className={styles.textarea} rows={3} value={formData.observation ?? ''} readOnly={!editing} onChange={(e) => setFormData((prev: typeof formData) => ({ ...prev, observation: e.target.value }))} />
                        </div>

                        {
                            type === 'edit' && (
                                <div className={`${styles.div} ${styles.full}`}>
                                    <label>Observações do sistema</label>
                                    <textarea className={styles.textarea} rows={3} value={(formData.observations ?? []).join('\n')} readOnly={true} />
                                </div>
                            )
                        }
                    </div>
                </main>

                <footer className={styles.modalFooter}>
                    <div className={styles.buttonsRow} style={{ marginRight: '1rem' }}>
                        <Button label='Fechar' handleFunction={() => handleClose()} styleType='transparent' />
                    </div>

                    {
                        type === 'create' ? (
                            <div className={styles.buttonsRow}>
                                <Button label={saving ? 'Salvando...' : 'Salvar'} handleFunction={() => handleSave()} isDisabled={saving} icon_feather={<Icon icon='save' />} />
                            </div>
                        ) : (
                            <div className={styles.buttonsRow}>
                                {me?.isUserAdmOfCurrentMainCompany && <Button label='Excluir' handleFunction={() => handleDisable(event)} styleType='red' icon_feather={<Icon icon='trash' />} />}
                                <WhatsappWebShortcut phone={formData.client?.phone} message={formData.messageIntegrationWhatsapp} clientId={formData.clientId} />
                                <Button label={`Detalhes • ${handleGetFirstName(formData.client?.fullName)}`} handleFunction={() => router.push(`${ROUTES.EMPRESA_CLIENTES}/${event?.schedule?.clientId}`)} styleType='transparent' icon_feather={<Icon icon='user-check' />} />

                                {
                                    // 1 = Marcado;
                                    event?.schedule?.scheduleStatus?.toString() === '1' && (
                                        !editing ? (
                                            <Fragment>
                                                <Button label='Editar' handleFunction={() => setEditing(true)} isDisabled={formData.scheduleStatus === 2} icon_feather={<Icon icon='edit' />} />
                                            </Fragment>
                                        ) : (
                                            <Fragment>
                                                <Button label='Cancelar edição' handleFunction={() => setEditing(false)} styleType='transparent' />
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