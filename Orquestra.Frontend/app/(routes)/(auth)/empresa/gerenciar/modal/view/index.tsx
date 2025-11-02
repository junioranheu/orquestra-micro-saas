import iCompanyOutput, { CONSTS_COMPANY } from '@/app/api/consts/company';
import { CONSTS_UTILITY } from '@/app/api/consts/utility';
import { Fetch } from '@/app/api/fetch';
import ContentLoaderText from '@/app/components/content-loader/text';
import Button from '@/app/components/input/button';
import Dropdown, { iDropdownOption } from '@/app/components/input/drop-down';
import InputImage from '@/app/components/input/image';
import InputMask from '@/app/components/input/text';
import ModalGeneric from '@/app/components/modal/generic';
import styles from '@/app/components/modal/generic/index.module.scss';
import Tags from '@/app/components/tags';
import ROUTES from '@/app/consts/routes';
import SYSTEM from '@/app/consts/system';
import handleConvertBase64ToFile from '@/app/functions/convert.base64ToFile';
import { handleFetchCEP } from '@/app/functions/fetch.CEP';
import { handleFormatDateToInputValue } from '@/app/functions/format.date';
import { handleGetOnlyNumbers } from '@/app/functions/format.string';
import handleGetPropName from '@/app/functions/get.propName';
import { handleClearFormData, handleLoopFormData, handleSetDropdownOption } from '@/app/functions/set.formState';
import swal from '@/app/functions/swal';
import useApiGetEnum from '@/app/hooks/api/useApiGetEnum';
import useApiRequestToSetterOnUrlChange from '@/app/hooks/api/useApiRequestToSetterOnUrlChange';
import useWindowSize from '@/app/hooks/useWindowSize';
import { Guid } from 'guid-typescript';
import { Dispatch, Fragment, KeyboardEvent, SetStateAction, useCallback, useEffect, useState } from 'react';

interface iProps {
    isModalOpen: boolean;
    setIsModalOpen: Dispatch<SetStateAction<boolean>>;
    type: 'edit' | 'create';
    company: iCompanyOutput | undefined;
}

export default function ModalEmpresaGerenciarView({ isModalOpen, setIsModalOpen, type, company }: iProps) {

    const windowSize = useWindowSize();

    const companyTypeEnum = useApiGetEnum({ enumName: 'CompanyTypeEnum' });
    const companySituationEnum = useApiGetEnum({ enumName: 'CompanySituationEnum' });
    const planTypeEnum = useApiGetEnum({ enumName: 'PlanTypeEnum' });

    const [countries, setCountries] = useState<string[] | undefined>([]);
    useApiRequestToSetterOnUrlChange<string[]>({ apiUrlRequest: CONSTS_UTILITY.getCountry, setter: setCountries });

    const [editing, setEditing] = useState<boolean>(false);
    const [saving, setSaving] = useState<boolean>(false);

    const [formData, setFormData] = useState<iCompanyOutput>({
        companyId: SYSTEM.EMPTY_GUID,
        name: '',
        email: '',
        phone: '',
        companyType: '',

        address: '',
        addressNumber: '',
        city: '',
        state: '',
        zipCode: '',
        country: '',

        logoFormFile: null,
        logoBase64: '',
        color: '',

        companySituation: '',
        planStartDate: SYSTEM.EMPTY_DATE,
        planEndDate: SYSTEM.EMPTY_DATE,
        planType: '',

        status: false
    });

    const handleClose = useCallback(() => {
        setSaving(false);
        setEditing(false);
        setIsModalOpen(false);
        handleClearFormData(setFormData);
    }, [setIsModalOpen]);

    useEffect(() => {
        handleClearFormData(setFormData);
        setSaving(false);
        setEditing(false);

        if (type === 'create') {
            setEditing(true);
            return;
        }

        if (!isModalOpen || !company) {
            return;
        }

        setFormData({
            companyId: company?.companyId,
            name: company?.name ?? '',
            email: company?.email ?? '',
            phone: company?.phone ?? '',
            companyType: company?.companyType ?? '',

            address: company?.address ?? '',
            addressNumber: company?.addressNumber ?? '',
            city: company?.city ?? '',
            state: company?.state ?? '',
            zipCode: company?.zipCode ?? '',
            country: company?.country ?? '',

            logoBase64: company?.logoBase64 ?? '',
            logoFormFile: company?.logoBase64 ? handleConvertBase64ToFile(company?.logoBase64, 'logo') : null,
            color: company?.color ?? '',

            companySituation: company?.companySituation ?? '',

            planStartDate: handleFormatDateToInputValue(company?.planStartDate ? new Date(company?.planStartDate) : new Date()),
            planEndDate: handleFormatDateToInputValue(company?.planEndDate ? new Date(company?.planEndDate) : new Date()),
            planType: company?.planType,
            status: company?.status
        });
    }, [isModalOpen, type, company, setIsModalOpen, handleClose]);

    const setCompanyTypeOption = handleSetDropdownOption(formData, setFormData, handleGetPropName(formData, x => x.companyType)[1]) as Dispatch<SetStateAction<iDropdownOption[]>>;
    const setCountryOption = handleSetDropdownOption(formData, setFormData, handleGetPropName(formData, x => x.country ?? '')[1]) as Dispatch<SetStateAction<iDropdownOption[]>>;

    async function handleSave() {
        if (!formData.name || !formData.email || !formData.phone || !formData.companyType) {
            swal({ content: SYSTEM.WARN_FILL_OBLIGATORY_FIELDS, icon: 'warning' });
            return;
        }

        setEditing(false);
        setSaving(true);

        const data = handleLoopFormData(formData);
        const input = data.json as iCompanyOutput;
        // console.log(input);

        const formDataInput = new FormData();
        formDataInput.append('CompanyId', input.companyId ? input.companyId.toString() : Guid.create().toString());
        formDataInput.append('Name', input.name);
        formDataInput.append('Email', input.email);

        if (input.phone) {
            const cleanedPhone = handleGetOnlyNumbers(input.phone);
            formDataInput.append('Phone', cleanedPhone);
        }

        formDataInput.append('CompanyType', input.companyType);
        if (input.address) formDataInput.append('Address', input.address);
        if (input.addressNumber) formDataInput.append('AddressNumber', input.addressNumber);
        if (input.city) formDataInput.append('City', input.city);
        if (input.state) formDataInput.append('State', input.state);

        if (input.zipCode) {
            const cleanedZipCode = handleGetOnlyNumbers(input.zipCode);
            formDataInput.append('ZipCode', cleanedZipCode);
        }

        if (input.country) formDataInput.append('Country', input.country);
        if (input.color) formDataInput.append('Color', input.color);
        if (input.logoFormFile && input.logoFormFile instanceof File) formDataInput.append('LogoFormFile', input.logoFormFile as Blob, input.logoFormFile.name);
        formDataInput.append('Status', input.status?.toString() ?? 'false');

        // console.log('formDataInput', formDataInput);

        if (type === 'create') {
            const company = await Fetch.post({ url: CONSTS_COMPANY.post, body: formDataInput, isFormData: true }) as iCompanyOutput;

            if (company) {
                swal({
                    content: 'Empresa registrada com sucesso. Além disso, ela foi definida como sua empresa principal.',
                    confirmFunction: () => window.location.reload(),
                    icon: 'success'
                });

                handleClose();
                return;
            }

            setEditing(true);
            setSaving(false);
            return;
        }

        const company = await Fetch.put({ url: CONSTS_COMPANY.put, body: formDataInput, isFormData: true }) as iCompanyOutput;

        if (company) {
            swal({
                content: 'Empresa atualizada com sucesso.',
                confirmFunction: () => window.location.reload(),
                icon: 'success'
            });

            handleClose();
            return;
        }

        setEditing(true);
        setSaving(false);
        return;
    }

    async function handleGetCEP(e: KeyboardEvent<HTMLInputElement>) {
        setFormData(prev => ({ ...prev, address: '', addressNumber: '', city: '', state: '', country: '' }));

        const cep = e.currentTarget.value;
        const cepRegex = /^\d{5}-\d{3}$/;

        if (!cepRegex.test(cep)) {
            return;
        }

        const data = await handleFetchCEP(cep);

        if (data) {
            setFormData(prev => ({ ...prev, ...data }));
        }
    }

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
            width={windowSize.width <= 1366 ? '85%' : '55%'}
            style={{ padding: 0, background: 'transparent' }}
        >
            <div className={styles.modalCard}>
                <header className={styles.modalHeader}>
                    <div className={styles.modalHeaderLeft}>
                        <h1 className={styles.inputTitle}>
                            {type === 'create' ? (formData.name ?? 'Nova empresa') : <ContentLoaderText content={formData?.name} />}
                        </h1>
                    </div>

                    <div className={styles.modalHeaderRight}>
                        <div className={styles.metaRow}>
                            <Tags
                                tags={[
                                    ...(type === 'edit' ? [{ label: company?.status ? 'Empresa verificada' : 'Empresa pendente de validação', title: company?.status ? 'Tudo certo! Essa empresa já foi verificada' : 'Parece que essa empresa ainda não foi verificada via e-mail', },] : []),
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

                        <InputMask title='CEP' fieldName='zipCode' formData={formData} setFormData={setFormData} isDisabled={!editing} mask='00000-000' handleOnChange={(e) => handleGetCEP(e)} />
                        <InputMask title='Rua' fieldName='address' formData={formData} setFormData={setFormData} isDisabled={!editing} />
                        <InputMask title='Número do endereço' fieldName='addressNumber' formData={formData} setFormData={setFormData} isDisabled={!editing} />
                        <InputMask title='Cidade' fieldName='city' formData={formData} setFormData={setFormData} isDisabled={!editing} />
                        <InputMask title='Estado' fieldName='state' formData={formData} setFormData={setFormData} isDisabled={!editing} />
                        <Dropdown title='País' options={(countries ?? []).map(country => ({ value: country, label: country }))} selectedOption={(countries ?? []).map(country => ({ value: country, label: country })).find(x => x.value === formData.country)} setSelectedOption={setCountryOption} isDisabled={!editing} />
                        <InputImage title='Logo' fieldName='logoFormFile' formData={formData} setFormData={setFormData} isDisabled={!editing} placeholder='Selecionar logo da empresa' />

                        {
                            type === 'edit' && (
                                <Fragment>
                                    <Dropdown title='Tipo do plano' options={planTypeEnum ?? []} selectedOption={planTypeEnum?.find(x => x.value.toString() === formData.planType?.toString())} isDisabled={true} />
                                    <Dropdown title='Situação' options={companySituationEnum ?? []} selectedOption={companySituationEnum?.find(x => x.value.toString() === formData.companySituation?.toString())} isDisabled={true} />

                                    {
                                        formData.companySituation?.toString() !== '1' && (
                                            <Fragment>
                                                <InputMask title='Início do plano' type='date' fieldName='planStartDate' formData={formData} setFormData={setFormData} isDisabled={true} />
                                                <InputMask title='Fim do plano' type='date' fieldName='planEndDate' formData={formData} setFormData={setFormData} isDisabled={true} />
                                            </Fragment>
                                        )
                                    }
                                </Fragment>
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
                                    !editing ? (
                                        <Fragment>
                                            <Button label='Plano e faturas' handleFunction={() => window.open(ROUTES.EMPRESA_USO_E_PLANO, '_blank')} isStyleSimple={true} />
                                            <Button label='Colaboradores' handleFunction={() => window.open(ROUTES.EMPRESA_COLABORADORES, '_blank')} isStyleSimple={true} />
                                            <Button label='Clientes' handleFunction={() => window.open(ROUTES.EMPRESA_CLIENTES, '_blank')} isStyleSimple={true} />
                                            <Button label='Habilitar edição' handleFunction={() => setEditing(true)} />
                                        </Fragment>
                                    ) : (
                                        <Fragment>
                                            <Button label='Cancelar edição' handleFunction={() => setEditing(false)} isStyleSimple={true} />
                                            <Button label={saving ? 'Salvando...' : 'Salvar'} handleFunction={() => handleSave()} isDisabled={saving} />
                                        </Fragment>
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