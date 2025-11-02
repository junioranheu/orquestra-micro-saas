'use client';
import Mascot from '@/app/components/mascot';
import SYSTEM from '@/app/consts/system';
import { handleApplyTheme, THEMES } from '@/app/hooks/useTheme';
import Tippy from '@tippyjs/react';
import { useEffect, useState } from 'react';
import styles from './index.module.scss';

export default function UsuarioConfiguracoesTabPersonalizacao() {
    return (
        <div className={styles.container}>
            <div className={styles.wrapper}>
                <MascotToggle />
                <FontSizeSelector />
                <ThemeSelector />
            </div>
        </div>
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
        setShowMascot((prev) => !prev);
    }

    return (
        <div className={styles.card}>
            <h2 className={styles.cardTitle}>Interface</h2>

            <div className={styles.toggleContainer}>
                <div className={styles.toggleInfo}>
                    <div className={styles.toggleText}>
                        <h3 className={styles.toggleTitle}>Exibir mascote de ajuda</h3>
                        <p className={styles.toggleDescription}>
                            Receba dicas e ajuda do {SYSTEM.MASCOT}.
                        </p>
                    </div>

                    {
                        showMascot && (
                            <Mascot
                                width={36}
                                isCentralized={false}
                                tippyContent={<div>Olá, eu sou o {SYSTEM.MASCOT}! 💚</div>}
                                tippyPlacement='right'
                                flipPeriodic={true}
                            />
                        )
                    }
                </div>

                <button onClick={handleToggleMascot} className={`${styles.toggle} ${showMascot ? styles.toggleActive : ''}`}>
                    <span className={styles.toggleThumb} />
                </button>
            </div>
        </div>
    )
}

function FontSizeSelector() {

    const [fontSize, setFontSize] = useState<number>(50);

    return (
        <div className={styles.card}>
            <h2 className={styles.cardTitle}>Tamanho da fonte</h2>

            <p className={styles.cardDescription}>
                Ajuste o tamanho do texto para melhor leitura.
            </p>

            <div className={styles.sliderContainer}>
                <span className={styles.sliderLabel}>Pequeno</span>

                <input
                    type='range'
                    min='0'
                    max='100'
                    value={fontSize}
                    onChange={(e) => setFontSize(Number(e.target.value))}
                    className={styles.slider}
                />

                <span className={`${styles.sliderLabel} ${styles.sliderLabelRight}`}>
                    Grande
                </span>
            </div>
        </div>
    )
}

function ThemeSelector() {

    const [selectedTheme, setSelectedTheme] = useState<string>('padrao');

    useEffect(() => {
        const saved = localStorage.getItem(SYSTEM.LOCAL_STORAGE_THEME) || 'padrao';
        setSelectedTheme(saved);
    }, []);

    function handleSelectTheme(themeId: string) {
        setSelectedTheme(themeId);
        localStorage.setItem(SYSTEM.LOCAL_STORAGE_THEME, themeId);
        handleApplyTheme(themeId);
    }

    return (
        <div className={styles.card}>
            <h2 className={styles.cardTitle}>Escolha seu tema preferido</h2>

            <div className={styles.themeContainer}>
                {
                    THEMES.map((theme) => (
                        <Tippy key={theme.id} content={theme.isUsable ? `Tema ${theme.label.toLocaleLowerCase()}.` : `O tema ${theme.label.toLocaleLowerCase()} está indisponível.`} placement='bottom'>
                            <button
                                onClick={() => theme.isUsable && handleSelectTheme(theme.id)}
                                className={`${styles.themeButton} ${theme.isUsable ? '' : styles.themeButtonDisabled}`}
                            >
                                <div className={`${styles.themeCircle} ${theme.className} ${selectedTheme === theme.id ? styles.themeSelected : ''}`} />
                                <span className={styles.themeLabel}>{theme.label}</span>
                            </button>
                        </Tippy>
                    ))
                }
            </div>
        </div>
    )
}