import iCompanyOutput, { CONSTS_COMPANY } from '@/app/api/consts/company';
import { Fetch } from '@/app/api/fetch';
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
import { handleClearFormData, handleLoopFormData, handleSetDropdownOption } from '@/app/functions/set.formState';
import swal from '@/app/functions/swal';
import useApiGetCompanySituationEnum from '@/app/hooks/api/enums/useApiGetCompanySituationEnum';
import useWindowSize from '@/app/hooks/useWindowSize';
import { Dispatch, Fragment, SetStateAction, useCallback, useEffect, useState } from 'react';

interface iProps {
    isOpen: boolean;
    setModalIsOpen: Dispatch<SetStateAction<boolean>>;
    company: iCompanyOutput | undefined;
}

export default function ModalEmpresaGerenciarView({ isOpen, setModalIsOpen, company }: iProps) {

    const windowSize = useWindowSize();

    const companyTypeEnum = useApiGetCompanySituationEnum({ enumName: 'CompanyTypeEnum' });
    const companySituationEnum = useApiGetCompanySituationEnum({ enumName: 'CompanySituationEnum' });

    const [editing, setEditing] = useState<boolean>(false);
    const [saving, setSaving] = useState<boolean>(false);

    const [formData, setFormData] = useState<iCompanyOutput>({
        companyId: SYSTEM.EMPTY_GUID,
        name: '',
        email: '',
        phone: '',
        companyType: '',

        streetAdress: '',
        city: '',
        state: '',
        zipCode: '',
        country: '',

        logo: [],
        color: '',

        companySituation: '',

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
            companyId: company?.companyId,
            name: company?.name ?? '',
            email: company?.email ?? '',
            phone: company?.phone ?? '',
            companyType: company?.companyType ?? '',

            streetAdress: company?.streetAdress ?? '',
            city: company?.city ?? '',
            state: company?.state ?? '',
            zipCode: company?.zipCode ?? '',
            country: company?.country ?? '',

            logo: company?.logo ?? [],
            color: company?.color ?? '',

            companySituation: company?.companySituation ?? '',

            planStartDate: handleFormatDateToInputValue(company?.planStartDate ? new Date(company?.planStartDate) : new Date()),
            planEndDate: handleFormatDateToInputValue(company?.planEndDate ? new Date(company?.planEndDate) : new Date()),
            modulesStr: company?.modulesStr ?? []
        });
    }, [isOpen, company, setModalIsOpen, handleClose]);

    const setCompanyTypeOption = handleSetDropdownOption(formData, setFormData, handleGetPropName(formData, x => x.companyType)[1]) as Dispatch<SetStateAction<iDropdownOption[]>>;
    const setColorOption = handleSetDropdownOption(formData, setFormData, handleGetPropName(formData, x => x.color ?? '')[1]) as Dispatch<SetStateAction<iDropdownOption[]>>;

    async function handleSave() {
        if (!formData.name || !formData.email || !formData.companyType) {
            swal({ content: SYSTEM.WARN_FILL_OBLIGATORY_FIELDS, icon: 'warning' });
            return;
        }

        setEditing(false);
        setSaving(true);

        const data = handleLoopFormData(formData);
        const input = data.json as iCompanyOutput;

        const company = await Fetch.put({ url: CONSTS_COMPANY.put, body: input }) as iCompanyOutput;

        if (company) {
            swal({
                content: 'Agendamento atualizado com sucesso.',
                confirmFunction: () => window.location.reload()
            });

            handleClose();
            return;
        }

        setEditing(true);
        setSaving(false);
        return;
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
                        <InputMask title='Telefone' fieldName='phone' formData={formData} setFormData={setFormData} isDisabled={!editing} isObligatory={true} mask='(00) 00000-0000' />
                        <Dropdown title='Tipo' options={companyTypeEnum ?? []} selectedOption={companyTypeEnum?.find(x => x.value.toString() === formData.companyType?.toString())} setSelectedOption={setCompanyTypeOption} isDisabled={!editing} isObligatory={true} />

                        <InputMask title='CEP' fieldName='zipCode' formData={formData} setFormData={setFormData} isDisabled={!editing} />
                        <InputMask title='Rua' fieldName='streetAdress' formData={formData} setFormData={setFormData} isDisabled={!editing} />
                        <InputMask title='Cidade' fieldName='city' formData={formData} setFormData={setFormData} isDisabled={!editing} />
                        <InputMask title='Estado' fieldName='state' formData={formData} setFormData={setFormData} isDisabled={!editing} />
                        <InputMask title='País' fieldName='country' formData={formData} setFormData={setFormData} isDisabled={!editing} />

                        <InputMask title='Logo (TEM QUE SER UM INPUT DE IMAGEM)' fieldName='logo' formData={formData} setFormData={setFormData} isDisabled={!editing} />
                        <Dropdown title='Cor de customização' options={COLORS ?? []} selectedOption={COLORS?.find(x => x.value.toString() === formData.color?.toString())} setSelectedOption={setColorOption} isDisabled={!editing} />

                        <Dropdown title='Situação' options={companySituationEnum ?? []} selectedOption={companySituationEnum?.find(x => x.value.toString() === formData.companySituation?.toString())} isDisabled={true} />
                        <InputMask title='Início do plano' type='date' fieldName='planStartDate' formData={formData} setFormData={setFormData} isDisabled={true} />
                        <InputMask title='Fim do plano' type='date' fieldName='planEndDate' formData={formData} setFormData={setFormData} isDisabled={true} />

                        <div className={styles.div}>
                            <label>Módulos</label>
                            <textarea className={styles.textarea} rows={3} value={formData.modulesStr?.join('\n') ?? ''} readOnly={true} />
                        </div>
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
                                    <Button label='Módulos' handleFunction={() => window.open(ROUTES.EMPRESA_USO_E_PLANO, '_blank')} isStyleSimple={true} />
                                    <Button label='Membros' handleFunction={() => window.open(ROUTES.EMPRESA_MEMBROS, '_blank')} isStyleSimple={true} />
                                    <Button label='Clientes' handleFunction={() => window.open(ROUTES.EMPRESA_CLIENTES, '_blank')} isStyleSimple={true} />
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