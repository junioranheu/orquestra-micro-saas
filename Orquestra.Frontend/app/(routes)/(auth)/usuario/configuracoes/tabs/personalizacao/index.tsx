'use client';
import { iMe } from '@/app/api/consts/auth';
import FontScaler from '@/app/components/font-scaler';
import SYSTEM from '@/app/consts/system';
import { useEffect, useState } from 'react';
import styles from './index.module.scss';

interface iProps {
    me: iMe;
}

export default function UsuarioConfiguracoesTabPersonalizacao({ me }: iProps) {

    return (
        <section className={styles.main}>
            <h1>Personalização {me?.currentMainCompany?.name}</h1>

            <FontScaler />
            <MascotToggle />
        </section>
    )
}

function MascotToggle() {

    const [showMascot, setShowMascot] = useState<boolean>(true);

    useEffect(() => {
        const value = localStorage.getItem(SYSTEM.LOCAL_STORAGE_SHOW_MASCOT);
        setShowMascot(value === null ? true : value === 'true');
    }, []);

    function handleToggleMascot() {
        localStorage.setItem(SYSTEM.LOCAL_STORAGE_SHOW_MASCOT, (!showMascot).toString());
        setShowMascot(prev => !prev);
    }

    return (
        <div className={styles.container}>
            <p>O mascote {SYSTEM.MASCOT} está <strong>{showMascot ? 'ativo' : 'desligado'}</strong>.</p>

            <button onClick={handleToggleMascot}>
                {showMascot ? 'Desligar' : 'Ligar'} mascote
            </button>
        </div>
    )
}