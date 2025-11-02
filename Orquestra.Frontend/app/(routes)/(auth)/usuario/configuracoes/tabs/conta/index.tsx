'use client';
import { iMe } from '@/app/api/consts/auth';
import Icon from '@/app/components/icon';
import Button from '@/app/components/input/button';
import SYSTEM from '@/app/consts/system';
import swal from '@/app/functions/swal';
import { useState } from 'react';
import styles from './index.module.scss';

interface iProps {
    me: iMe;
}

export default function UsuarioConfiguracoesTabConta({ me }: iProps) {
    return (
        <div className={styles.container}>
            <div className={styles.wrapper}>
                <PersonalInfoForm me={me} />
                <PasswordForm />
            </div>
        </div>
    )
}

function PersonalInfoForm({ me }: iProps) {

    const [personalInfo, setPersonalInfo] = useState({
        userName: me?.userName,
        email: me?.email
    });

    const handleChange = (field: string, value: string) => {
        setPersonalInfo(prev => ({ ...prev, [field]: value }));
    }

    async function handleSave() {
        alert('Informações pessoais salvas com sucesso!');
    }

    return (
        <div className={styles.card}>
            <div className={styles.cardHeader}>
                <h2 className={styles.cardTitle}>Informações pessoais</h2>
                <p className={styles.cardSubtitle}>
                    Atualize suas informações pessoais.
                </p>
            </div>

            <div className={styles.form}>
                <div className={styles.formRow}>
                    <div className={styles.formGroup}>
                        <label className={styles.label}>Nome completo</label>
                        <input
                            type='text'
                            value={personalInfo.userName}
                            onChange={(e) => handleChange('userName', e.target.value)}
                            className={styles.input}
                            placeholder='Digite seu nome completo'
                        />
                    </div>

                    <div className={styles.formGroup}>
                        <label className={styles.label}>E-mail</label>
                        <div className={styles.inputWithIcon}>
                            <input
                                type='email'
                                value={personalInfo.email}
                                onChange={(e) => handleChange('email', e.target.value)}
                                className={styles.input}
                                placeholder='seu@email.com'
                            />
                        </div>
                    </div>
                </div>

                <div className={styles.buttonGroup}>
                    <Button label='Salvar alterações' handleFunction={handleSave} />
                </div>
            </div>
        </div>
    )
}

function PasswordForm() {
    const [passwords, setPasswords] = useState({
        newPassword: '',
        newPasswordConfirmation: ''
    });

    const handleChange = (field: string, value: string) => {
        setPasswords(prev => ({ ...prev, [field]: value }));
    }

    async function handleSave() {
        if (!passwords.newPassword || !passwords.newPasswordConfirmation) {
            swal({
                content: SYSTEM.WARN_FILL_OBLIGATORY_FIELDS,
                icon: 'error'
            });
            return;
        }

        if (passwords.newPassword !== passwords.newPasswordConfirmation) {
            swal({
                content: 'As senhas não coincidem.',
                icon: 'error'
            });
            return;
        }

        alert('Senha alterada com sucesso!');
        setPasswords({ newPassword: '', newPasswordConfirmation: '' });
    }

    return (
        <div className={styles.card}>
            <div className={styles.cardHeader}>
                <h2 className={styles.cardTitle}>Alterar senha</h2>
                <p className={styles.cardSubtitle}>
                    Para sua segurança, escolha uma senha forte.
                </p>
            </div>

            <div className={styles.form}>
                <div className={styles.formRow}>
                    <div className={styles.formGroup}>
                        <label className={styles.label}>Nova senha</label>
                        <input
                            type='password'
                            value={passwords.newPassword}
                            onChange={(e) => handleChange('newPassword', e.target.value)}
                            className={styles.input}
                            placeholder='Digite a nova senha'
                        />
                    </div>

                    <div className={styles.formGroup}>
                        <label className={styles.label}>Confirmar nova senha</label>
                        <input
                            type='password'
                            value={passwords.newPasswordConfirmation}
                            onChange={(e) => handleChange('newPasswordConfirmation', e.target.value)}
                            className={styles.input}
                            placeholder='Confirme a nova senha'
                        />
                    </div>
                </div>

                <div className={styles.buttonGroup}>
                    <Button
                        label='Salvar nova senha'
                        handleFunction={handleSave}
                        icon_feather={<Icon icon='lock' size='small' />}
                    />
                </div>
            </div>
        </div>
    )
}