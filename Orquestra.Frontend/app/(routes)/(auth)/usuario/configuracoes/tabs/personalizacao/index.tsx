'use client';
import Button from '@/app/components/input/button';
import Mascot from '@/app/components/mascot';
import SYSTEM from '@/app/consts/system';
import { useIsModalGrid, useShowChatbot, useShowExpandedSidebar, useShowLogsDashboard } from '@/app/hooks/contexts/useGlobalContext';
import { handleApplyTheme, THEMES } from '@/app/hooks/useTheme';
import Tippy from '@tippyjs/react';
import { Fragment, ReactNode, useCallback, useEffect, useState } from 'react';
import styles from './index.module.scss';

export default function UsuarioConfiguracoesTabPersonalizacao() {
    return (
        <div className={styles.container}>
            <div className={styles.wrapper}>
                <InterfaceCustomizer />
                <FontSizeSelector />
                <ThemeSelector />
            </div>
        </div>
    )
}

interface iToggleContainerProps {
    toggleInfoContent: ReactNode;
    handleToggle: () => void;
    boolToggle: boolean;
}

function ToggleContainer({ toggleInfoContent, handleToggle, boolToggle }: iToggleContainerProps) {
    return (
        <div className={styles.toggleContainer}>
            <div className={styles.toggleInfo}>
                {toggleInfoContent}
            </div>

            <button onClick={handleToggle} className={`${styles.toggle} ${boolToggle ? styles.toggleActive : ''}`}>
                <span className={styles.toggleThumb} />
            </button>
        </div>
    )
}

function InterfaceCustomizer() {

    const [showMascot, setShowMascot] = useState<boolean>(true);
    const [showChatbot, setShowChatbot] = useShowChatbot();
    const [showExpandedSidebar, setShowExpandedSidebar] = useShowExpandedSidebar();
    const [isModalGrid, setIsModalGrid] = useIsModalGrid();
    const [showLogsDashboard, setShowLogsDashboard] = useShowLogsDashboard();

    const handleInitSettings = useCallback(() => {
        const valueMascot = localStorage.getItem(SYSTEM.LOCAL_STORAGE_SHOW_MASCOT);
        setShowMascot(valueMascot === null ? true : valueMascot === 'true');

        const valueChatbot = localStorage.getItem(SYSTEM.LOCAL_STORAGE_SHOW_CHATBOT);
        setShowChatbot(valueChatbot === null ? true : valueChatbot === 'true');

        const valueExpandedSidebar = localStorage.getItem(SYSTEM.LOCAL_STORAGE_SHOW_EXPANDED_SIDEBAR);
        setShowExpandedSidebar(valueExpandedSidebar === null ? true : valueExpandedSidebar === 'true');

        const valueModalGrid = localStorage.getItem(SYSTEM.LOCAL_STORAGE_IS_MODAL_GRID);
        setIsModalGrid(valueModalGrid === null ? true : valueModalGrid === 'true');

        const valueShowLogsDashboard = localStorage.getItem(SYSTEM.LOCAL_STORAGE_SHOW_LOGS_DASHBOARD);
        setShowLogsDashboard(valueShowLogsDashboard === null ? true : valueShowLogsDashboard === 'true');
    }, [setShowMascot, setShowChatbot, setShowExpandedSidebar, setIsModalGrid, setShowLogsDashboard]);

    useEffect(() => {
        handleInitSettings();
    }, [handleInitSettings]);

    function handleToggleMascot() {
        localStorage.setItem(SYSTEM.LOCAL_STORAGE_SHOW_MASCOT, (!showMascot).toString());
        setShowMascot((prev) => !prev);
    }

    function handleToggleChatbot() {
        localStorage.setItem(SYSTEM.LOCAL_STORAGE_SHOW_CHATBOT, (!showChatbot).toString());
        setShowChatbot((prev) => !prev);
    }

    function handleToggleExpandedSidebar() {
        localStorage.setItem(SYSTEM.LOCAL_STORAGE_SHOW_EXPANDED_SIDEBAR, (!showExpandedSidebar).toString());
        setShowExpandedSidebar((prev) => !prev);
    }

    function handleToggleIsModalGrid() {
        localStorage.setItem(SYSTEM.LOCAL_STORAGE_IS_MODAL_GRID, (!isModalGrid).toString());
        setIsModalGrid((prev) => !prev);
    }

    function handleToggleShowLogsDashboard() {
        localStorage.setItem(SYSTEM.LOCAL_STORAGE_SHOW_LOGS_DASHBOARD, (!showLogsDashboard).toString());
        setShowLogsDashboard((prev) => !prev);
    }

    return (
        <div className={styles.card}>
            <h2 className={styles.cardTitle}>Interface</h2>

            <ToggleContainer
                toggleInfoContent={
                    <Fragment>
                        <div className={styles.toggleText}>
                            <h3 className={styles.toggleTitle}>Exibir assistente virtual</h3>
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
                    </Fragment>
                }
                handleToggle={handleToggleMascot}
                boolToggle={showMascot}
            />

            <ToggleContainer
                toggleInfoContent={
                    <div className={styles.toggleText}>
                        <h3 className={styles.toggleTitle}>Exibir chatbot</h3>
                        <p className={styles.toggleDescription}>
                            {showChatbot ? 'O chatbot está ativado e visível todo o tempo.' : 'O chatbot está desativado.'}
                        </p>
                    </div>
                }
                handleToggle={handleToggleChatbot}
                boolToggle={showChatbot}
            />

            <ToggleContainer
                toggleInfoContent={
                    <div className={styles.toggleText}>
                        <h3 className={styles.toggleTitle}>Expandir menu lateral</h3>
                        <p className={styles.toggleDescription}>
                            {showExpandedSidebar ? 'O menu lateral está expandido. Aqui você vê todas as opções disponíveis de uma vez.' : 'O menu lateral está recolhido. Clique para expandir e ver mais opções de uma vez.'}
                        </p>
                    </div>
                }
                handleToggle={handleToggleExpandedSidebar}
                boolToggle={showExpandedSidebar}
            />

            <ToggleContainer
                toggleInfoContent={
                    <div className={styles.toggleText}>
                        <h3 className={styles.toggleTitle}>Campos de cadastro/edição ficam lado a lado</h3>
                        <p className={styles.toggleDescription}>
                            {isModalGrid ? 'Sempre que houver espaço, os campos de cadastro/edição aparecem lado a lado, ocupando duas colunas.' : 'Os campos de cadastro/edição aparecem um embaixo do outro.'}
                        </p>
                    </div>
                }
                handleToggle={handleToggleIsModalGrid}
                boolToggle={isModalGrid}
            />

            <ToggleContainer
                toggleInfoContent={
                    <div className={styles.toggleText}>
                        <h3 className={styles.toggleTitle}>Exibir painel de notificações no dashboard</h3>
                        <p className={styles.toggleDescription}>
                            {showLogsDashboard ? 'O painel está ativado e visível no dashboard.' : 'O painel está oculto.'}
                        </p>
                    </div>
                }
                handleToggle={handleToggleShowLogsDashboard}
                boolToggle={showLogsDashboard}
            />
        </div>
    )
}

function FontSizeSelector() {

    const MIN_FONT_SIZE = 75;
    const MAX_FONT_SIZE = 125;
    const DEFAULT_FONT_SIZE = 100;

    const [fontSize, setFontSize] = useState<number>(DEFAULT_FONT_SIZE);

    useEffect(() => {
        const stored = localStorage.getItem(SYSTEM.LOCAL_STORAGE_USER_FONT_SIZE);
        const size = stored ? Number(stored) : DEFAULT_FONT_SIZE;
        setFontSize(size);
        document.documentElement.style.setProperty('--user-font-size', `${size}%`);
        document.documentElement.style.fontSize = `${size}%`;
    }, []);

    function handleChangeFontSize(value: number) {
        setFontSize(value);
        localStorage.setItem(SYSTEM.LOCAL_STORAGE_USER_FONT_SIZE, value.toString());
        document.documentElement.style.setProperty('--user-font-size', `${value}%`);
        document.documentElement.style.fontSize = `${value}%`;
    }

    function handleResetFontSize() {
        setFontSize(DEFAULT_FONT_SIZE);
        localStorage.removeItem(SYSTEM.LOCAL_STORAGE_USER_FONT_SIZE);
        document.documentElement.style.setProperty('--user-font-size', `${DEFAULT_FONT_SIZE}%`);
        document.documentElement.style.fontSize = `${DEFAULT_FONT_SIZE}%`;
    }

    return (
        <div className={styles.card}>
            <h2 className={styles.cardTitle}>Tamanho da fonte</h2>

            <p className={styles.cardDescription}>
                Ajuste o tamanho do texto para melhor leitura.
            </p>

            <div className={styles.sliderLabels}>
                <span className={styles.sliderLabel}>{MIN_FONT_SIZE}%</span>
                <span className={styles.sliderCurrent}>Tamanho atual: {fontSize}%</span>
                <span className={styles.sliderLabel}>{MAX_FONT_SIZE}%</span>
            </div>

            <div className={styles.sliderContainer}>
                <input
                    type='range'
                    min={MIN_FONT_SIZE}
                    max={MAX_FONT_SIZE}
                    step='5'
                    value={fontSize}
                    onChange={(e) => handleChangeFontSize(Number(e.target.value))}
                    className={styles.slider}
                />
            </div>

            {
                fontSize !== DEFAULT_FONT_SIZE && (
                    <Button
                        label='Resetar ao padrão'
                        styleType='transparent'
                        handleFunction={() => handleResetFontSize()}
                        isDisabled={fontSize === DEFAULT_FONT_SIZE}
                        style={{ marginTop: '1rem' }}
                        classes={SYSTEM.ANIMATE}
                    />
                )
            }
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